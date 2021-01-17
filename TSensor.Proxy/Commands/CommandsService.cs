#nullable enable
using System.ComponentModel;
using TSensor.Proxy.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TSensor.Proxy.Com;

namespace TSensor.Proxy.Commands
{
    public class CommandsService
    {
        private readonly BackgroundWorker worker;

        private readonly Config _config;
        private readonly ILogger _logger;
        private readonly CommandsRepository _repository;

        public static bool Flag1 = false;
        public static bool IsThereCommandRunning = false;
        public static bool IsThereAnyCommandDone = false;
        
        public CommandsService(Config config, ILogger logger, CommandsRepository repository)
        {
            _config = config;
            _logger = logger;
            _repository = repository;
            
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Если таблица портов уже заполнена, и сейчас не ранится никаких других коммад
            if (PortListener.FlagPortTableReady && !IsThereCommandRunning)
            {
                try
                {
                    IsThereCommandRunning = true;
                    var command = await GetCommandAsync();
                    if (command == null)
                    {
                        Flag1 = false;
                    }
                    else
                    {
                        Flag1 = true;

                        _logger.Log($"New command to eval:{command.Command}, Flag1 setted to true, waiting for Flag2");
                        if (PortListener.Flag2)
                        {
                            try
                            {
                                if (!_config.IsTcpInputMode)
                                {
                                    bool? failed = null;
                                    var result = await doWork(command);
                                    if (result.failed && result.failedReason != null)
                                    {
                                        await SendStatusAsync(failed: true, commandGuid: command.Guid,
                                            result.failedReason);
                                    }
                                    else if (!result.failed && result.failedReason == null)
                                    {
                                        await SendStatusAsync(failed: false, commandGuid: command.Guid);
                                    }
                                }
                                else
                                {
                                    const string failedReason =
                                        "The controller input mode is TCP! there are no possible to eval command in that mode.";
                                    _logger.Log(failedReason, isError: true);
                                    await SendStatusAsync(failed: true, commandGuid: command.Guid,
                                        failedReason: failedReason);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Log($"While eval command got error: {ex.Message}", isError: true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Команды закончились
                    if (ex.Message.Contains("404"))
                    {
                        Flag1 = false;
                        CommandEvalResult.sended = true; // поддельный сенд чтобы Worker_RunWorkerCompleted не ругался
                    }
                    else
                        _logger.Log($"While getting command got error: {ex.Message}", isError: true);
                }
                finally
                {
                    IsThereCommandRunning = false;
                    IsThereAnyCommandDone = true;
                }
            }
        }

        private async Task<ControllerCommand> GetCommandAsync()
        {
            using var elapsed = Elapsed.Create; //засекание времени
            var deviceGuid = _config.DeviceGuid;
            
            var command = await _repository.GetLastCommand();
            _logger.Log($"Got command {command.Command}", elapsed: elapsed);
            return command;
        }
        
        private async Task SendStatusAsync(bool failed, string commandGuid, string? failedReason = null)
        {
            using var elapsed = Elapsed.Create;

            if (!failed)
            {
                var result = await Http.Http.PostAsync(_config.ApiUrlSendCommandComplete,
                    new Dictionary<string, string>
                    {
                        { "deviceGuid", _config.DeviceGuid },
                        { "commandGuid", commandGuid }
                    });

                if (result.Exception != null)
                {
                    _logger.Log($"Command result error sending", isError: true);
                    _logger.Log(result.Exception.Message, isError: true);
                }
                else
                {
                    _logger.Log($"Command result successfully sended({result.Content})", elapsed: elapsed);
                }
            }
            else
            {
                var result = await Http.Http.PostAsync(_config.ApiUrlSendCommandFailed,
                    new Dictionary<string, string>
                    {
                        { "deviceGuid", _config.DeviceGuid },
                        { "commandGuid", commandGuid },
                        { "failReason", failedReason ?? "Ошибка, failed reason == null, failed == true" }
                    });

                if (result.Exception != null)
                {
                    _logger.Log($"Command result error sending", isError: true);
                    _logger.Log(result.Exception.Message, isError: true);
                }
                else
                {
                    _logger.Log($"Command result successfully sended({result.Content})", elapsed: elapsed);
                }
            }
        }

        /// <summary>
        /// Инкапсулирует результат для выставление его в базу данных
        /// </summary>
        public class CommandEvalResult
        {
            public string? failedReason = null;
            public bool failed = false;
            //Если выставлено, значит сообщение о выполненной команде было отправлено
            public static bool sended = false;
            public CommandEvalResult(bool failed, string? failedReason = null)
            {
                this.failed = failed;
                this.failedReason = failedReason;
                sended = true;
            }
        }

        /// <summary>
        /// Выполняет команду посылая её на порт контроллеру
        /// </summary>
        /// <param name="command">ControllerCommand</param>
        /// <returns>Возвращает результат в виде класса CommandEvalResult</returns>
        private async Task<CommandEvalResult> doWork(ControllerCommand command)
        {
            try
            {
                //ПАРСИНГ
                ParsedCommand parcedCommand = null;
                try
                {
                    parcedCommand = ParceSingleRegisterWriteCommand(command.Command);
                }
                catch (ArgumentException e)
                {
                    _logger.Log($"Parsed error: {e}", isError: true);
                    return new CommandEvalResult(true, $"Parsed error: {e}");
                }
                
                var comPortForCmdEval = ComPortsRepository.IzkNumbersToPortNames[parcedCommand.izkNumber]; 

                
                if (parcedCommand != null)
                {
                    if (command.Command == "RESET")
                    {
                        _logger.Log($"Reset command detected, evaluating");
                        var evalResult = CommandEvaluator.Eval(comPortForCmdEval,
                            new ParsedCommand() { izkNumber = parcedCommand.izkNumber, value = 65535, startAddress = 0});
                        return evalResult;
                    }
                    else
                    {
                        var evalResult = CommandEvaluator.Eval(comPortForCmdEval, parcedCommand);
                        await SendStatusAsync(evalResult.failed, evalResult.failedReason);
                        return evalResult;
                    }
                }
                else
                {
                    _logger.Log($"ParceSingleRegisterWriteCommand error", isError: true);
                    return new CommandEvalResult(true, "ParceSingleRegisterWriteCommand error");
                } //TODO gavr тут експрешон ис олвейс тру на 190
                
            }
            catch (Exception e)
            {
                _logger.Log($"Error while send status {e.Message}", isError: true);
                return new CommandEvalResult(true, $"Error while send status {e.Message}");
            }
        }

        static ParsedCommand ParceSingleRegisterWriteCommand(string command)
        {
            if (command.Length <= 15)
            {
                var cmd = command.Trim();
                var cmdWithoutSemi = cmd.Replace(":", "");
                var blockAddress2 = cmdWithoutSemi.Substring(0, 2);
                var commandTypeRegister2 = cmdWithoutSemi.Substring(2, 2);
                var retisterAddress4 = cmdWithoutSemi.Substring(4, 4);
                var value4 = cmdWithoutSemi.Substring(8, 4);

                IFormatProvider format = null;
                var hexStyle = NumberStyles.HexNumber;

                if (byte.TryParse(blockAddress2, hexStyle, format, out var byteIzkAddress))
                {
                    if (ushort.TryParse(value4, hexStyle, format, out var ushortValue4))
                    {
                        if (ushort.TryParse(retisterAddress4, hexStyle, format, out var ushortRetisterAddress4))
                        {
                            var parsedCommand = new ParsedCommand()
                                {izkNumber = byteIzkAddress, value = ushortValue4, startAddress = ushortRetisterAddress4};
                            return parsedCommand;
                        }
                        else
                            throw new ArgumentException($"Невозможно распарсить адрес регистра: {retisterAddress4}");
                    }
                    else
                        throw new ArgumentException($"Невозможно распарсить записываемое значение: {value4}");
                }
                else
                    throw new ArgumentException($"Невозможно распарсить адрес ИЗК(первые 2 значения): {blockAddress2}");
            }
            else
                throw new ArgumentException($"Длинна команды на запись одного регистра должна быть <= 15");
            
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsThereAnyCommandDone)
            {
                if (CommandEvalResult.sended)
                {
                    CommandEvalResult.sended = false;
                }
                else
                {
                    _logger.Log(
                        "ВНИМАНИЕ Сообщение о выполнении команды не было отправлено! " +
                        "Комана останется в базе со статусом 0 и будет приходить снова и снова\n" +
                        "Это значит что в прокси присутствует путь в конце которого не отправляется результат",
                        isError: true);
                }
            }
            Thread.Sleep(_config.CommandGetInterval);
            worker.RunWorkerAsync();
        }

        public void Run()
        {
            worker.RunWorkerAsync();
        }

    }
    
    public class ParsedCommand
    {
        public byte izkNumber;
        public ushort startAddress;
        public ushort value;
    }
    
}