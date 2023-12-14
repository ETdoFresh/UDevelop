using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GameEditor.Databases
{
    public class DatabaseTickUtility
    {
        public static async UniTask<object> GetValueAtUtcTickAsync(string path, long tick)
        {
            var value = await Database.GetValueAsync(path);
            return value == null ? null : GetValueAtUtcTick(JObject.FromObject(value), tick);
        }

        public static async UniTask<object> GetValueAtUtcNowAsync(string path) =>
            await GetValueAtUtcTickAsync(path, DateTime.UtcNow.Ticks);

        public static object GetValueAtUtcTick(JObject jObject, long tick)
        {
            if (jObject == null) return null;
            var properties = jObject.Properties().ToArray();
            var bestTick = 0L;
            var bestValue = (JToken)null;
            var minTick = long.MaxValue;
            var minValue = (JToken)null;
            foreach (var property in properties)
            {
                var currentTick = long.Parse(property.Name);
                if (currentTick > bestTick && currentTick <= tick)
                {
                    bestTick = currentTick;
                    bestValue = property.Value;
                }
                if (currentTick < minTick)
                {
                    minTick = currentTick;
                    minValue = property.Value;
                }
            }
            return bestValue ?? minValue;
        }

        public static object GetValueAtUtcNow(JObject jObject) =>
            GetValueAtUtcTick(jObject, DateTime.UtcNow.Ticks);
    }
}