using System.Collections;
using System.Collections.Generic;

namespace Xambon.PreLoader
{
    public class PreLoadParameters : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _internalParameters = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Prism.Navigation.NavigationParameters" /> class.
        /// </summary>
        public PreLoadParameters()
        {

        }


        public void Add(string key, object value)
        {
            _internalParameters.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _internalParameters.ContainsKey(key);
        }

        public T GetValue<T>(string key) where T : class
        {
            if (_internalParameters.TryGetValue(key, out var tryValue))
            {
                if (tryValue is T value)
                {
                    return value;
                }
            }
            return default(T);
        }


        public int Count()
        {
            return _internalParameters.Count;
        }
        public IEnumerable<string> Keys()
        {
            return _internalParameters.Keys;

        }
       
       
        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            if (_internalParameters.TryGetValue(key, out var valueObj))
            {
                if (valueObj is T valueInternal)
                {
                    value = (T)valueObj;
                    return true;
                }
            }
            return false;
        }

        public object Item(string key)
        {
            return _internalParameters[key];
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _internalParameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           return _internalParameters.GetEnumerator();
        }
    }
}
