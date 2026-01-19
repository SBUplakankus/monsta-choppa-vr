using Esper.ESave;
using UnityEngine;

namespace Saves
{
    [RequireComponent(typeof(SaveFile))]
    public abstract class SaveFileManagerBase : MonoBehaviour
    {
        #region Fields

        [Header("Save Data")]
        protected SaveFile SaveFile;

        #endregion
        
        #region Class Methods

        protected abstract void HandleSaveRequested();

        protected abstract void HandleSaveCompleted();

        protected abstract void HandleLoadRequested();

        protected abstract void HandleLoadCompleted();

        #endregion
        
        #region Unity Methods

        private void Awake() => SaveFile = GetComponent<SaveFile>();
        
        #endregion
        
        
    }
}