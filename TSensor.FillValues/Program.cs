using System;
using System.Collections.Generic;
using System.Linq;

namespace TSensor.FillValues
{
    class Program
    {
        private static LoggerService ExceptionLogger = new LoggerService("exception.log");

        private const string PRESS_ANY_KEY_MESSAGE = "Нажмите любую кнопку...";
        private static void Exit(string message, Exception exception = null)
        {
            if (exception != null)
            {
                ExceptionLogger.Write(exception);
            }

            Console.WriteLine(message);
            Console.WriteLine(PRESS_ANY_KEY_MESSAGE);
            Console.ReadKey();
            Environment.Exit(0);
        }

        private const string DIALOG_FILTER = "Excel 2003 и старше(*.xlsx)|*.xlsx";

        [STAThread]
        public static void Main(string[] args)
        {
            var dialogService = new DialogService();
            var excelService = new ExcelService();

            Dictionary<int, Dictionary<int, string>> inputRawData = new Dictionary<int, Dictionary<int, string>>();

            using (var openStream = dialogService.Open(DIALOG_FILTER))
            {
                if (openStream == null)
                {
                    Exit("Файл не выбран");
                }

                try
                {
                    inputRawData = excelService.ReadToEnd(openStream);
                }
                catch (Exception e)
                {
                    Exit("Во время чтения файла произошла ошибка", e);
                }
            }

            if (inputRawData.Count() <= 1)
            {
                Exit("Файл не содержит данных");
            }

            List<Entity> values = new List<Entity>();
            int rowValueCount = inputRawData.First().Value.Count() - 2;

            try
            {
                foreach (var row in inputRawData.Skip(1))
                {
                    values.Add(new Entity(row.Value, rowValueCount, row.Key));
                }
            }
            catch(Exception e) 
            {
                Exit($"При обработке данных произошла ошибка: {e.Message}");
            }

            var resultData = new List<IEnumerable<string>>
            {
                inputRawData.Take(1).SelectMany(p => p.Value, (p, k) => k.Value)
            };

            Entity previous = null;
            foreach (var current in values)
            {
                if (previous == null)
                {
                    previous = current;
                    resultData.Add(current.ToStringDict());
                }
                else
                {
                    if ((current.Time - previous.Time).TotalSeconds > 1)
                    {
                        resultData.AddRange(Entity.AvgList(previous, current).Select(p => p.ToStringDict()));
                    }

                    resultData.Add(current.ToStringDict());
                    previous = current;
                }
            }

            using (var saveStream = dialogService.Save(DIALOG_FILTER))
            {
                if (saveStream == null)
                {
                    Exit("Файл не выбран");
                }

                excelService.SaveToStream(resultData, saveStream);
            }

            Exit("Файл успешно сохранен");
        }
    }
}