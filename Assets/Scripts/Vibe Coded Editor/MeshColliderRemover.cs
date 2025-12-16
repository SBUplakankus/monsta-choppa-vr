using UnityEditor;
using UnityEngine;

namespace Vibe_Coded_Editor
{
    public class ReplaceMeshColliders : MonoBehaviour
    {
        // Add a static method with MenuItem attribute
        [MenuItem("Tools/Replace All MeshColliders")]
        public static void ReplaceAllMeshColliders()
        {
            // Use Undo to make it work in prefab mode
            Undo.SetCurrentGroupName("Replace All MeshColliders");
            var group = Undo.GetCurrentGroup();
            
            foreach (var mc in FindObjectsOfType<MeshCollider>())
            {
                var go = mc.gameObject;
                
                // Use Undo.DestroyObjectImmediate for prefab compatibility
                Undo.DestroyObjectImmediate(mc);
                
                // Record the addition of BoxCollider
                var boxCollider = go.AddComponent<BoxCollider>();
                Undo.RegisterCreatedObjectUndo(boxCollider, "Add BoxCollider");
            }
            
            Undo.CollapseUndoOperations(group);
            Debug.Log("Replaced all MeshColliders with BoxColliders");
        }
    }
}
