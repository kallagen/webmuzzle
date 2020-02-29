using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TSensor.FillValues
{
    public class Entity
    {
        public string Label { get; set; }
        public DateTime Time { get; set; }
        public List<decimal> ValueDict { get; set; } = new List<decimal>();

        public Entity() { }
        public Entity(Dictionary<int, string> cells, int valueCount, int row)
        {
            Label = cells[1];

            try
            {
                Time = DateTime.ParseExact(cells[2], new[] { "HH:mm:ss", "H:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch
            {
                throw new Exception($"Неправильный формат времени \"{cells[2]}\", строка {row}, колонка 2");
            }

            foreach(var cell in cells.Skip(2))
            {
                try
                {
                    ValueDict.Add(decimal.Parse(cell.Value));
                }
                catch
                {
                    throw new Exception($"Неправильный формат значения \"{cell.Value}\", строка {row}, колонка {cell.Key}");
                }
            }

            if (ValueDict.Count != valueCount)
            {
                throw new Exception($"Количество значений не совпадает с количеством колонок в шапке, строка {row}");
            }
        }

        public IEnumerable<string> ToStringDict()
        {
            return new[]
            {
                Label, Time.ToString("HH:mm:ss")
            }.Concat(ValueDict.Select(p => p.ToString()));
        }

        private static List<decimal> Delta(Entity first, Entity second, int parts)
        {
            return Enumerable.Range(0, first.ValueDict.Count).Select(idx =>
                (second.ValueDict[idx] - first.ValueDict[idx]) / parts).ToList();
        }

        private static Entity AddDelta(Entity first, List<decimal> delta, int parts)
        {
            return new Entity
            {
                Label = first.Label,
                Time = first.Time.AddSeconds(parts),
                ValueDict = Enumerable.Range(0, first.ValueDict.Count)
                    .Select(idx => Math.Round(first.ValueDict[idx] + delta[idx] * parts, 3)).ToList()
            };
        }

        public static IEnumerable<Entity> AvgList(Entity first, Entity second)
        {
            int totalSeconds = (int)(second.Time - first.Time).TotalSeconds;
            var delta = Delta(first, second, totalSeconds);

            return Enumerable.Range(1, totalSeconds - 1).Select(s => AddDelta(first, delta, s));
        }
    }
}