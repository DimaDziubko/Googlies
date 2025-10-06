using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Utils
{
    public class JSONUtils
    {
        public static T ConvertToObj<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    Debug.LogWarning("[JSON DESERIALIZING ERROR]" + args.ErrorContext.Error.Message + "; JSON: " + json);
                    args.ErrorContext.Handled = true;
                },
                //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            T obj = JsonConvert.DeserializeObject<T>(json, settings);
            return obj;
        }

        public static string ConvertToString(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    Debug.LogWarning("[JSON SERIALIZING ERROR]" + args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                },

                TypeNameHandling = TypeNameHandling.None
            };

            string json = JsonConvert.SerializeObject(obj, settings);
            return json;
        }
        public static bool TryParseJson<T>(string json, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(json, settings);
            return success;
        }

        public static List<string> InvalidJsonElements;
        public static List<T> ToList<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    Debug.LogWarning("[JSON DESERIALIZING ERROR TO LIST]" + args.ErrorContext.Error.Message + "; JSON: " + json);
                    args.ErrorContext.Handled = true;
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };

            List<T> objectsList = JsonConvert.DeserializeObject<List<T>>(json, settings);
            return objectsList;
        }
    }
}
