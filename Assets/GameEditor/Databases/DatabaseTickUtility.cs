using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GameEditor.Databases
{
    public class DatabaseTickUtility
    {
        public static async UniTask<T> GetValueAtUtcTickAsync<T>(string path, long tick)
        {
            var value = await Database.GetValueAsync(path);
            return value == null ? default : GetValueAtUtcTick<T>(JObject.FromObject(value), tick);
        }

        public static async UniTask<T> GetValueAtUtcNowAsync<T>(string path) =>
            await GetValueAtUtcTickAsync<T>(path, DateTime.UtcNow.Ticks);
        
        public static async UniTask<long> GetClosestTickWithoutGoingOverAsync(string path, long tick)
        {
            var value = await Database.GetValueAsync(path);
            return value == null ? 0L : GetClosestTickWithoutGoingOver(JObject.FromObject(value), tick);
        }
        
        public static async UniTask<long> GetClosestTickWithoutGoingOverNowAsync(string path) =>
            await GetClosestTickWithoutGoingOverAsync(path, DateTime.UtcNow.Ticks);

        public static T GetValueAtUtcTick<T>(JObject jObject, long tick)
        {
            if (jObject == null) return default;
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
            return bestValue != null ? bestValue.ToObject<T>() : minValue != null ? minValue.ToObject<T>() : default;
        }

        public static T GetValueAtUtcNow<T>(JObject jObject) =>
            GetValueAtUtcTick<T>(jObject, DateTime.UtcNow.Ticks);
        
        public static long GetClosestTickWithoutGoingOver(JObject jObject, long tick)
        {
            if (jObject == null) return 0L;
            var properties = jObject.Properties().ToArray();
            var bestTick = 0L;
            var minTick = long.MaxValue;
            foreach (var property in properties)
            {
                var currentTick = long.Parse(property.Name);
                if (currentTick > bestTick && currentTick <= tick)
                    bestTick = currentTick;
                if (currentTick < minTick)
                    minTick = currentTick;
            }
            return bestTick == 0L ? minTick : bestTick;
        }
        
        public static long GetClosestTickWithoutGoingOverNow(JObject jObject) =>
            GetClosestTickWithoutGoingOver(jObject, DateTime.UtcNow.Ticks);
    }
}