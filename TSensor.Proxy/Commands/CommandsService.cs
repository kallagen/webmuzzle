using System.ComponentModel;
using TSensor.Proxy.Logger;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TSensor.Proxy.Commands
{
    public class CommandsService
    {

        private readonly BackgroundWorker worker;

        private readonly Config _config;
        private readonly ILogger _logger;
        private readonly CommandsRepository _repository;

        //true --> закрыто, команда еще выполняется 
        public static bool Lock = false;
        private static bool IsThereNoNewTasks = false;

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
            try
            {
                var command = await GetCommandAsync();
                if (command == null)
                {
                    IsThereNoNewTasks = false;
                }
                else
                {
                    try
                    {
                        await doWork(command);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"While eval command got: {ex.Message}", isError: true);
                    }
                }

            }
            catch (Exception ex)
            {
                //TODO если ошибка содержит 404 то выставить флаг говорящий о том что задачи закончились
                if (ex.Message.Contains("404"))
                {
                    IsThereNoNewTasks = true;
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
        
        private async Task SendStatusAsync(bool failed, string commandGuid)
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
                        { "failReason", "tempFailedReason" }
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
            
            // var result = await Http.Http.PostAsync(failed? _config.ApiUrlSendCommandFailed: _config.ApiUrlSendCommandComplete,
            //     new Dictionary<string, string>
            //     {
            //         { "deviceGuid", _config.DeviceGuid },
            //         { "commandGuid", commandGuid }
            //     });
            //
            // if (result.Exception != null)
            // {
            //     _logger.Log($"Command result error sending", isError: true);
            //     _logger.Log(result.Exception.Message, isError: true);
            // }
            // else
            // {
            //     _logger.Log($"Command result successfully sended({result.Content})", elapsed: elapsed);
            // }
        }

        private async Task doWork(ControllerCommand command)
        {
            try
            {
                Lock = true;

                if (command.Command == "RESET")
                {
                    CommandEvaluator.Eval(command.Command, out var error);
                }
                else
                {
                    // ПАРСИНГ ЗНАЧЕНИЙ ИЗ command.Command
                    CommandEvaluator.Eval(command.Command, out var error);
                    await SendStatusAsync(error != null, command.Guid.ToString());

                }

                Lock = false;
            }
            catch (Exception e)
            {
                _logger.Log($"Error while send status {e.Message}", isError: true);
            }
        }

        private void RestartController()
        {
            //в случае если командой был рестарт выставить в конфиге флаг и отправить на сервак статус рестартинг
            //тут же нон стоп чекать стейт контроллера, и как только включится отпарвить статус enabled
        
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO тут нужно сообщать о том что задача выполнена помечая в таблице, до слипа,
            //и при этом выключать новые запросы, хотя стоп, они и так сами офаются 
            
            //если до этого все норм то спим промежуток до попытки запроса следующей команды
            
            
            Thread.Sleep(IsThereNoNewTasks?60*1000:_config.CommandGetInterval.Value);
            _logger.Log("sas");
            worker.RunWorkerAsync();
        }
        
        
        public void Run()
        {
            worker.RunWorkerAsync();
        }

    }
    
}