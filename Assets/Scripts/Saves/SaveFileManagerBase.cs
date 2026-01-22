using Esper.ESave;
using UnityEngine;

namespace Saves
{
    [RequireComponent(typeof(SaveFileSetup))]
    public abstract class SaveFileManagerBase : MonoBehaviour
    {
        #region Fields

        [Header("Save Data")] 
        private SaveFileSetup _saveFileSetup;
        protected SaveFile SaveFile;

        #endregion
        
        #region Class Methods

        protected abstract void HandleSaveRequested();

        protected abstract void HandleSaveCompleted();

        protected abstract void HandleLoadRequested();

        protected abstract void HandleLoadCompleted();

        #endregion
        
        #region Unity Methods

        protected void GetSaveFile()
        { 
            _saveFileSetup = GetComponent<SaveFileSetup>();
            SaveFile = _saveFileSetup.GetSaveFile();
        }
        
        #endregion
        
        
    }
}