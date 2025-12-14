using System.Collections.Generic;
using UnityEngine;

namespace Databases.Base
{
    /// <summary>
    /// Base Class for storing game data through scriptable objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DatabaseBase<T> : ScriptableObject
    {
        [SerializeField] protected List<T> entries = new();
        
        private Dictionary<string, T> _lookup;
        
        /// <summary>
        /// Get the Database Specific Key
        /// </summary>
        /// <param name="entry">Type to get the key from</param>
        /// <returns>The Key for the Database Entry</returns>
        protected abstract string GetKey(T entry);
        
        /// <summary>
        /// Create the Database Lookup based off the entries content
        /// </summary>
        private void BuildLookup()
        {
            if (entries == null || entries.Count == 0)
            {
                Debug.LogWarning($"Database {name} is empty");
                return;
            }
            
            _lookup = new Dictionary<string, T>();

            foreach (var entry in entries)
            {
                var key = GetKey(entry);
                _lookup[key] = entry;
            }
        }
        
        protected virtual void OnEnable()
        {
            BuildLookup();
        }
        
        /// <summary>
        /// Try to fetch an entry from the database
        /// </summary>
        /// <param name="id">ID of the entry</param>
        /// <returns>Data attached to the ID</returns>
        public T TryGet(string id)
        {
            if (_lookup == null) BuildLookup();
            
            return _lookup.GetValueOrDefault(id);
        }
    }
}
