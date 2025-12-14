using UnityEngine;
using UnityEditor;

public class RemoveMeshColliders
{
    [MenuItem("Tools/Remove All Mesh Colliders")]
    static void RemoveAllMeshColliders()
    {
        MeshCollider[] colliders = GameObject.FindObjectsOfType<MeshCollider>();
        int count = colliders.Length;

        foreach (MeshCollider mc in colliders)
        {
            Undo.DestroyObjectImmediate(mc); // safe, allows undo
        }

        Debug.Log("Removed " + count + " MeshColliders from scene.");
    }
}
