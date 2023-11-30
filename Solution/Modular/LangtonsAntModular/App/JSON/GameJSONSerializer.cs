using System;
using System.Text.Json;
using LangtonsAnt;

namespace Json
{
    public static class GameJSONSerializer
    {
        public static string ToJson(IGame game)
        {
            string json;
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                    new MultiDimensionalArrayConverter(),
                    new InterfaceConverterFactory(typeof(GeneralizedAnt), typeof(IAnt)),
                }
            };

            try
            {
                json = JsonSerializer.Serialize((Game)game, serializeOptions);
            }
            catch (Exception ex)
            {
                throw new JSONSerializationException("Could not save game state to JSON", ex);
            }
            return json;
        }

        public static Game FromJson(string jsonString)
        {
            Game game;
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                    new MultiDimensionalArrayConverter(),
                    new InterfaceConverterFactory(typeof(GeneralizedAnt), typeof(IAnt)),
                }
            };

            try
            {
                game = JsonSerializer.Deserialize<Game>(jsonString, serializeOptions) ?? throw new Exception("Game deserialized from JSON is Null");
            }
            catch (Exception ex)
            {
                throw new JSONSerializationException("Could not load game state from JSON", ex);
            }
            return game;
        }
    }
}
