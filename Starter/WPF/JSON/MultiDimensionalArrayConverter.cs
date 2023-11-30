using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Json
{
    class MultiDimensionalArrayConverter : JsonConverter<byte[,]>
    {
        public override byte[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int[][] jagged = JsonSerializer.Deserialize<int[][]>(reader.GetString()!)!;
            byte[,] res = new byte[jagged.Length, jagged[0].Length];
            for (int i = 0; i < jagged.Length; i++)
                for (int j = 0; j < jagged[i].Length; j++)
                    res[i, j] = (byte)jagged[i][j];
            return res;
        }

        public override void Write(Utf8JsonWriter writer, byte[,] value, JsonSerializerOptions options)
        {
            int[][] jagged = new int[value.GetLength(0)][];
            for (int i = 0; i < value.GetLength(0); i++)
            {
                jagged[i] = new int[value.GetLength(1)];
                for (int j = 0; j < value.GetLength(1); j++)
                    jagged[i][j] = value[i, j];
            }
            string json = JsonSerializer.Serialize(jagged);
            writer.WriteStringValue(json);
        }
    }
}
