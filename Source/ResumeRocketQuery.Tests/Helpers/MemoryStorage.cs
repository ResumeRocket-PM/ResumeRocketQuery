using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Module.Memory
{
    public interface IMemoryStorage
    {
        Task<int> InsertAsync<T>(T reference, Action<T, int> setIdentity);
        Task<List<T>> SelectAsync<T>();
        Task DeleteAsync<T>(Func<T, bool> deleteCondition);
        Task<int> UpdateAsync<T>(Func<T, bool> updateCondition, Action<T> updateAction);
    }

    public class MemoryStorage : IMemoryStorage
    {
        private readonly ConcurrentDictionary<Type, List<object>> _memoryStore;

        public MemoryStorage()
        {
            _memoryStore = new ConcurrentDictionary<Type, List<object>>();
        }

        private int Insert<T>(T reference, Action<T, int> setIdentity)
        {
            var id = GenerateIdentifier<T>();

            var storedValue = Clone(reference);

            setIdentity(storedValue, id);

            if (!_memoryStore.ContainsKey(typeof(T)))
            {
                _memoryStore[typeof(T)] = new List<object>
                {
                    storedValue
                };
            }
            else
            {
                _memoryStore[typeof(T)].Add(storedValue);
            }

            return id;
        }

        private List<T> Select<T>()
        {
            List<T> result = new List<T>();

            if (_memoryStore.ContainsKey(typeof(T)))
            {
                var storage = _memoryStore[typeof(T)];

                result = storage.Select(x => (T)x).ToList();
            }

            return result;
        }

        private void Delete<T>(Func<T, bool> deleteCondition)
        {
            List<T> result = new List<T>();

            if (_memoryStore.ContainsKey(typeof(T)))
            {
                _memoryStore[typeof(T)].RemoveAll(x => deleteCondition((T)x));
            }
        }

        private int Update<T>(Func<T, bool> updateCondition, Action<T> updateAction)
        {
            int result = 0;

            if (_memoryStore.ContainsKey(typeof(T)))
            {
                var dataCloneStorages = new List<object>();

                foreach (T row in _memoryStore[typeof(T)])
                {
                    var storage = Clone<T>(row);

                    dataCloneStorages.Add(storage);
                }

                foreach (T clone in dataCloneStorages.Where(x => updateCondition((T)x)))
                {
                    updateAction(clone);

                    result++;
                }

                _memoryStore[typeof(T)] = dataCloneStorages;
            }

            return result;
        }

        public Task<int> InsertAsync<T>(T reference, Action<T, int> setIdentity)
        {
            return Task.FromResult(Insert<T>(reference, setIdentity));
        }

        public Task<List<T>> SelectAsync<T>()
        {
            return Task.FromResult(Select<T>());
        }

        public Task DeleteAsync<T>(Func<T, bool> deleteCondition)
        {
            Delete<T>(deleteCondition);

            return Task.CompletedTask;
        }

        public Task<int> UpdateAsync<T>(Func<T, bool> updateCondition, Action<T> updateAction)
        {
            return Task.FromResult(Update<T>(updateCondition, updateAction));
        }

        private T Clone<T>(T source)
        {
            if (object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);

            return result;
        }

        private int GenerateIdentifier<T>()
        {
            int result = 1;

            if (_memoryStore.ContainsKey(typeof(T)))
            {
                result = _memoryStore[typeof(T)].Count + 1;
            }

            return result;
        }
    }
}