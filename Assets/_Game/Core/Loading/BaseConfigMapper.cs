using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class BaseConfigMapper
    {
        protected readonly IMyLogger _logger;

        protected BaseConfigMapper(IMyLogger logger)
        {
            _logger = logger;
        }
        protected List<T> ParseConfigList<T>(JObject jsonData, string configKey) where T : class
        {
            var tokens = jsonData[configKey];
            if (tokens == null)
            {
                _logger.LogError($"{typeof(T).Name} config is null for key {configKey}");
                return null;
            }

            return tokens.Select(token => token.ToObject<T>()).ToList();
        }
    }
}
