using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Semigroups
{
    internal class MatrixConverter : JsonConverter<Matrix>
    {
        public override Matrix ReadJson(JsonReader reader, Type objectType, Matrix existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            var lastScope = value.LastIndexOf(')');
            value = value.Substring(0, lastScope + 1);
            var trimed = value.Trim('(', ')');

            var resList = new List<List<int>>();
            var lines = trimed.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                resList.Add(new List<int>());
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < values.Length; j++)
                {
                    resList[i].Add(int.Parse(values[j]));
                }
            }

            var result = new int[resList.Count, resList.Count > 0 ? resList[0].Count : 0];

            for (int i = 0; i < resList.Count; i++)
            {
                for (int j = 0; j < resList[i].Count; j++)
                {
                    result[i, j] = resList[i][j];
                }
            }

            return new Matrix(result);
        }

        public override void WriteJson(JsonWriter writer, Matrix value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
