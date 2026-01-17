using System;
using System.Collections.Generic;
using UnityEngine;

namespace Databases
{
     /// <summary>
    /// Base Class for storing game data through scriptable objects
    /// </summary>
    /// <typeparam name="T">Data stored in Database</typeparam>
    public abstract class DatabaseBase<T> : ScriptableObject
    {
        [SerializeField] private T[] entries;
        
        private Dictionary<string, T> _lookup;
        private bool _isLookupBuilt;
        
        public T[] Entries => entries;
        
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
            if (_isLookupBuilt) return;
            
            _lookup = new Dictionary<string, T>(entries.Length, StringComparer.Ordinal);
            
            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                var key = NormalizeKey(GetKey(entry));
                _lookup[key] = entry;
            }
            
            _isLookupBuilt = true;
        }
        
        protected virtual void OnEnable()
        {
            BuildLookup();
        }
        
        protected virtual void OnDisable()
        {
            _isLookupBuilt = false;
            _lookup = null; 
        }
        
        /// <summary>
        /// Normalizes a key for consistent lookup. Uses ordinal comparison for better performance.
        /// </summary>
        /// <param name="id">The raw key to normalize</param>
        /// <returns>Normalized key</returns>
        private static string NormalizeKey(string id) => id.Trim().ToLowerInvariant();
        
        /// <summary>
        /// Try to fetch an entry from the database
        /// </summary>
        /// <param name="id">ID of the entry</param>
        /// <param name="entry">Output parameter for data</param>
        /// <returns>True if entry was found</returns>
        public bool TryGet(string id, out T entry)
        {
            if (!_isLookupBuilt) BuildLookup();
            
            return _lookup.TryGetValue(NormalizeKey(id), out entry);
        }
        
        /// <summary>
        /// Get entry without output parameter for cleaner syntax when you know it exists.
        /// Throws KeyNotFoundException if entry is not found.
        /// </summary>
        public T Get(string id)
        {
            if (!_isLookupBuilt) BuildLookup();
            
            return _lookup[NormalizeKey(id)];
        }
        
        #region Editor-Only Methods
        #if UNITY_EDITOR
        /// <summary>
        /// Rebuild lookup in Editor mode (useful when entries change)
        /// </summary>
        public void EditorRebuildLookup()
        {
            _isLookupBuilt = false;
            BuildLookup();
        }
        #endif
        #endregion
    }
}
