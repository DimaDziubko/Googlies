using System.Collections.Generic;

namespace _Game.Core.Data
{
    public class DataPool<TType, TData>
    {
        private readonly Dictionary<TType, TData> _data = new Dictionary<TType, TData>();
        public void Add(TType type, TData data)
        {
            _data[type] = data;
        }

        public TData ForType(TType type)
        {
            if (_data.TryGetValue(type, out var data))
            {
                return data;
            }

            return default;
        }

        //Debug
        public Dictionary<TType, TData> GetData()
        {
            return _data;
        }

        public void Cleanup()
        {
            _data.Clear();
        }
    }
}