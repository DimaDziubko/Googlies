using System.Linq;
using DevToDev.Analytics;

namespace _Game.Utils._Dtd
{
    public static class DtdParamsExtensions
    {
        public static string ToLogString(this DTDCustomEventParameters parameters)
        {
            return string.Join(", ", parameters.GetAllParameters().Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
}