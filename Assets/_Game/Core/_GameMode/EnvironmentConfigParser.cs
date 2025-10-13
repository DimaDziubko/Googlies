using _Game.Core.Configs.Build;
using System;

namespace _Game.Core._GameMode
{
    public static class EnvironmentConfigParser
    {
        public static EnvironmentConfig Parse(string configContent)
        {
            var config = new EnvironmentConfig();

            try
            {
                var lines = configContent.Split('\n');

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Skip comments and empty lines
                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                        continue;

                    // Skip section headers
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                        continue;

                    // Parse key-value pairs
                    if (trimmedLine.Contains("="))
                    {
                        var parts = trimmedLine.Split('=');
                        if (parts.Length >= 2)
                        {
                            var key = parts[0].Trim().ToLower();
                            var value = parts[1].Trim();

                            if (key == "environment_id")
                            {
                                if (int.TryParse(value, out int environmentId))
                                {
                                    if (Enum.IsDefined(typeof(EnvironmentType), environmentId))
                                    {
                                        config.ActiveEnvironment = (EnvironmentType)environmentId;
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"Invalid environment_id: {environmentId}. Valid values: 0 (Development), 1 (Staging), 2 (Production)");
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException($"Cannot parse environment_id value: {value}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error parsing environment config: {e.Message}", e);
            }

            return config;
        }
    }
}