using System;
using System.Collections.Generic;
using System.Linq;

namespace Trident.Testing
{
    public class InMemoryDataStore
    {
        readonly IDictionary<Type, IList<dynamic>> _dataStore = new Dictionary<Type, IList<dynamic>>(); 

        public IQueryable<T> Get<T>() where T : class, new()
        {
            if (_dataStore.ContainsKey(typeof(T)))
                return _dataStore[typeof(T)].Select(x => (T)x).AsQueryable();

            return new List<T>().AsQueryable();
        }

        public void Add<T>(T newRecord) where T : class, new()
        {
            if (_dataStore.ContainsKey(typeof(T)))
                _dataStore[typeof(T)].Add(newRecord);

            else
                _dataStore.Add(typeof(T), new List<dynamic> { newRecord });

        }

        public void Delete<T>(T objectToDelete) where T : class, new()
        {
            if (_dataStore.ContainsKey(typeof(T)))
                _dataStore[typeof(T)].Remove(objectToDelete);

        }

        public void Clear()
        {
            _dataStore.Clear();
        }
    }
}