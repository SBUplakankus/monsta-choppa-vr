using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Vibe_Coded_Editor
{
    public class OrganizeSyntyPrefabs : MonoBehaviour
    {
        #region Main Menu Items
        
        [MenuItem("Tools/Synty/Organize by Category (Smart)")]
        public static void OrganizeByCategorySmart()
        {
            Undo.SetCurrentGroupName("Organize by Category");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeByCategory(prefab, false);
            }
            
            Undo.CollapseUndoOperations(group);
            Debug.Log("Organized selected prefabs by category");
        }
        
        [MenuItem("Tools/Synty/Organize by Category (Smart)", true)]
        private static bool ValidateOrganizeByCategory()
        {
            return Selection.gameObjects.Length > 0;
        }
        
        [MenuItem("Tools/Synty/Organize by Category (Flatten)")]
        public static void OrganizeByCategoryFlatten()
        {
            Undo.SetCurrentGroupName("Organize by Category Flatten");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeByCategory(prefab, true);
            }
            
            Undo.CollapseUndoOperations(group);
            Debug.Log("Organized and flattened selected prefabs");
        }
        
        #endregion
        
        #region Category-Based Organization
        
        private static void OrganizeByCategory(GameObject prefab, bool flatten)
        {
            if (prefab.transform.childCount == 0) return;
            
            Undo.RegisterCompleteObjectUndo(prefab, "Organize by Category");
            
            if (flatten)
            {
                FlattenToRoot(prefab);
            }
            
            // Dictionary to store categories
            var categories = new Dictionary<string, List<Transform>>();
            
            // Get all direct children
            var children = new List<Transform>();
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                children.Add(prefab.transform.GetChild(i));
            }
            
            // Categorize each child
            foreach (var child in children)
            {
                string category = GetCategory(child.name);
                
                if (!categories.ContainsKey(category))
                {
                    categories[category] = new List<Transform>();
                }
                
                categories[category].Add(child);
            }
            
            // Create category groups and move children
            foreach (var category in categories)
            {
                if (category.Value.Count > 0)
                {
                    // Create or find category group
                    Transform categoryGroup = prefab.transform.Find(category.Key);
                    if (categoryGroup == null)
                    {
                        GameObject groupObj = new GameObject(category.Key);
                        Undo.RegisterCreatedObjectUndo(groupObj, $"Create {category.Key} Group");
                        categoryGroup = groupObj.transform;
                        categoryGroup.SetParent(prefab.transform);
                    }
                    
                    // Move children to category group
                    foreach (var child in category.Value)
                    {
                        if (child.parent != categoryGroup)
                        {
                            Undo.SetTransformParent(child, categoryGroup, $"Move to {category.Key}");
                        }
                    }
                    
                    // Sort children within category alphabetically
                    SortChildrenAlphabetically(categoryGroup.gameObject);
                }
            }
            
            // Sort category groups alphabetically
            SortChildrenAlphabetically(prefab);
        }
        
        private static string GetCategory(string objectName)
        {
            string name = objectName.ToLower();
            
            // Dungeon Realms specific categories
            if (name.Contains("wall")) return "Walls";
            if (name.Contains("floor")) return "Floors";
            if (name.Contains("ceiling") || name.Contains("roof")) return "Ceilings";
            if (name.Contains("door") || name.Contains("gate") || name.Contains("portcullis")) return "Doors";
            if (name.Contains("arch") || name.Contains("archway")) return "Arches";
            if (name.Contains("column") || name.Contains("pillar")) return "Columns";
            if (name.Contains("stair") || name.Contains("step")) return "Stairs";
            if (name.Contains("bridge")) return "Bridges";
            if (name.Contains("torch") || name.Contains("brazier") || name.Contains("candle")) return "Lights";
            if (name.Contains("chest") || name.Contains("crate") || name.Contains("barrel")) return "Containers";
            if (name.Contains("table") || name.Contains("desk") || name.Contains("counter")) return "Tables";
            if (name.Contains("chair") || name.Contains("bench") || name.Contains("throne")) return "Seating";
            if (name.Contains("bed") || name.Contains("cot")) return "Beds";
            if (name.Contains("bookshelf") || name.Contains("bookcase")) return "Shelves";
            if (name.Contains("altar") || name.Contains("shrine")) return "Altars";
            if (name.Contains("cage") || name.Contains("cell")) return "Cages";
            if (name.Contains("chain") || name.Contains("rope")) return "Chains";
            if (name.Contains("spike") || name.Contains("trap")) return "Traps";
            if (name.Contains("lever") || name.Contains("switch")) return "Switches";
            if (name.Contains("grating") || name.Contains("grate")) return "Grates";
            if (name.Contains("debris") || name.Contains("rubble")) return "Debris";
            if (name.Contains("cobweb") || name.Contains("web")) return "Webs";
            if (name.Contains("fungus") || name.Contains("mushroom")) return "Fungi";
            if (name.Contains("vine") || name.Contains("moss")) return "Vegetation";
            if (name.Contains("puddle") || name.Contains("water")) return "Water";
            if (name.Contains("lava")) return "Lava";
            if (name.Contains("crystal") || name.Contains("gem")) return "Crystals";
            if (name.Contains("bone") || name.Contains("skull")) return "Bones";
            if (name.Contains("flag") || name.Contains("banner") || name.Contains("tapestry")) return "Banners";
            if (name.Contains("weaponrack") || name.Contains("armorstand")) return "Equipment";
            if (name.Contains("pot") || name.Contains("urn") || name.Contains("vase")) return "Pottery";
            if (name.Contains("mirror")) return "Mirrors";
            if (name.Contains("clock") || name.Contains("hourglass")) return "Timepieces";
            if (name.Contains("cage") || name.Contains("jail")) return "Prisons";
            
            // Mesh/collider/material categories
            if (name.Contains("collider") || name.Contains("col_") || name.Contains("_col")) return "Colliders";
            if (name.Contains("mesh") || name.Contains("_mesh") || name.Contains("geo")) return "Meshes";
            if (name.Contains("lod") || name.Contains("_lod")) return "LODs";
            if (name.Contains("material") || name.Contains("mat_") || name.Contains("_mat")) return "Materials";
            if (name.Contains("shadow") || name.Contains("shadowmesh")) return "Shadows";
            if (name.Contains("hitbox") || name.Contains("trigger")) return "Triggers";
            if (name.Contains("particle") || name.Contains("fx_") || name.Contains("vfx")) return "Effects";
            if (name.Contains("light") || name.Contains("_light")) return "LightObjects";
            if (name.Contains("sound") || name.Contains("audio")) return "Audio";
            
            // Default categories based on components
            Transform childTransform = GameObject.Find(objectName)?.transform;
            if (childTransform != null)
            {
                if (childTransform.GetComponent<MeshRenderer>() != null) return "Meshes";
                if (childTransform.GetComponent<Collider>() != null) return "Colliders";
                if (childTransform.GetComponent<LODGroup>() != null) return "LODs";
                if (childTransform.GetComponent<ParticleSystem>() != null) return "Effects";
                if (childTransform.GetComponent<Light>() != null) return "Lights";
                if (childTransform.GetComponent<AudioSource>() != null) return "Audio";
            }
            
            return "Miscellaneous";
        }
        
        #endregion
        
        #region Specialized Sorting Options
        
        [MenuItem("Tools/Synty/Organize Walls & Floors")]
        public static void OrganizeWallsAndFloors()
        {
            Undo.SetCurrentGroupName("Organize Walls & Floors");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeSpecificCategories(prefab, new[] { "Walls", "Floors", "Ceilings" });
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        [MenuItem("Tools/Synty/Organize Props & Decorations")]
        public static void OrganizePropsAndDecor()
        {
            Undo.SetCurrentGroupName("Organize Props & Decor");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeSpecificCategories(prefab, new[] { 
                    "Containers", "Tables", "Seating", "Beds", "Shelves", 
                    "Altars", "Pottery", "Banners", "Equipment", "Mirrors", 
                    "Timepieces", "Prisons", "Miscellaneous" 
                });
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        [MenuItem("Tools/Synty/Organize Environmental")]
        public static void OrganizeEnvironmental()
        {
            Undo.SetCurrentGroupName("Organize Environmental");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeSpecificCategories(prefab, new[] { 
                    "Vegetation", "Fungi", "Webs", "Debris", "Bones",
                    "Water", "Lava", "Crystals", "Traps", "Chains", "Grates"
                });
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        [MenuItem("Tools/Synty/Organize Structural")]
        public static void OrganizeStructural()
        {
            Undo.SetCurrentGroupName("Organize Structural");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeSpecificCategories(prefab, new[] { 
                    "Columns", "Arches", "Doors", "Stairs", "Bridges",
                    "Switches", "Lights", "Triggers"
                });
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        [MenuItem("Tools/Synty/Organize Technical")]
        public static void OrganizeTechnical()
        {
            Undo.SetCurrentGroupName("Organize Technical");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                OrganizeSpecificCategories(prefab, new[] { 
                    "Meshes", "Colliders", "LODs", "Materials", "Shadows",
                    "Effects", "Lights", "Audio", "Triggers"
                });
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        private static void OrganizeSpecificCategories(GameObject prefab, string[] categories)
        {
            if (prefab.transform.childCount == 0) return;
            
            Undo.RegisterCompleteObjectUndo(prefab, "Organize Specific Categories");
            
            // Create groups for specified categories
            foreach (var category in categories)
            {
                // Find all children that belong to this category
                var children = new List<Transform>();
                for (int i = 0; i < prefab.transform.childCount; i++)
                {
                    var child = prefab.transform.GetChild(i);
                    if (GetCategory(child.name) == category)
                    {
                        children.Add(child);
                    }
                }
                
                if (children.Count > 0)
                {
                    // Create category group
                    Transform categoryGroup = prefab.transform.Find(category);
                    if (categoryGroup == null)
                    {
                        GameObject groupObj = new GameObject(category);
                        Undo.RegisterCreatedObjectUndo(groupObj, $"Create {category} Group");
                        categoryGroup = groupObj.transform;
                        categoryGroup.SetParent(prefab.transform);
                    }
                    
                    // Move children to category group
                    foreach (var child in children)
                    {
                        if (child.parent != categoryGroup)
                        {
                            Undo.SetTransformParent(child, categoryGroup, $"Move to {category}");
                        }
                    }
                    
                    // Sort children within category
                    SortChildrenAlphabetically(categoryGroup.gameObject);
                }
            }
            
            // Sort all groups alphabetically
            SortChildrenAlphabetically(prefab);
        }
        
        #endregion
        
        #region Utility Functions
        
        private static void FlattenToRoot(GameObject prefab)
        {
            // Get all transforms except the root
            var allTransforms = prefab.GetComponentsInChildren<Transform>()
                .Where(t => t != prefab.transform)
                .ToList();
            
            foreach (var transform in allTransforms)
            {
                // If it has a parent that's not the root, move it to root
                if (transform.parent != null && transform.parent != prefab.transform)
                {
                    Undo.SetTransformParent(transform, prefab.transform, "Flatten Hierarchy");
                }
            }
            
            // Remove any empty group GameObjects
            var emptyChildren = prefab.GetComponentsInChildren<Transform>()
                .Where(t => t != prefab.transform && t.childCount == 0 && 
                           (t.name.Contains("Group") || t.name.Contains("Container") || t.name == "GameObject"))
                .ToList();
            
            foreach (var empty in emptyChildren)
            {
                Undo.DestroyObjectImmediate(empty.gameObject);
            }
        }
        
        private static void SortChildrenAlphabetically(GameObject parent)
        {
            var children = new List<Transform>();
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                children.Add(parent.transform.GetChild(i));
            }
            
            children = children.OrderBy(t => t.name).ToList();
            
            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }
        
        #endregion
        
        #region Quick Actions
        
        [MenuItem("Tools/Synty/Quick Actions/Sort All Children A-Z")]
        public static void SortAllChildrenAlphabetically()
        {
            Undo.SetCurrentGroupName("Sort All Children A-Z");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                SortChildrenAlphabetically(prefab);
            }
            
            Undo.CollapseUndoOperations(group);
            Debug.Log("Sorted all children alphabetically");
        }
        
        [MenuItem("Tools/Synty/Quick Actions/Create Mesh Groups")]
        public static void CreateMeshGroups()
        {
            Undo.SetCurrentGroupName("Create Mesh Groups");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                GroupByComponent<MeshRenderer>(prefab, "Meshes");
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        [MenuItem("Tools/Synty/Quick Actions/Create Collider Groups")]
        public static void CreateColliderGroups()
        {
            Undo.SetCurrentGroupName("Create Collider Groups");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                GroupByComponent<Collider>(prefab, "Colliders");
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        [MenuItem("Tools/Synty/Quick Actions/Create LOD Groups")]
        public static void CreateLODGroups()
        {
            Undo.SetCurrentGroupName("Create LOD Groups");
            int group = Undo.GetCurrentGroup();
            
            foreach (var prefab in Selection.gameObjects)
            {
                GroupByComponent<LODGroup>(prefab, "LODs");
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        private static void GroupByComponent<T>(GameObject prefab, string groupName) where T : Component
        {
            if (prefab.transform.childCount == 0) return;
            
            Undo.RegisterCompleteObjectUndo(prefab, $"Group {groupName}");
            
            // Find children with component
            var children = new List<Transform>();
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                var child = prefab.transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child);
                }
            }
            
            if (children.Count > 0)
            {
                // Create group
                Transform group = prefab.transform.Find(groupName);
                if (group == null)
                {
                    GameObject groupObj = new GameObject(groupName);
                    Undo.RegisterCreatedObjectUndo(groupObj, $"Create {groupName} Group");
                    group = groupObj.transform;
                    group.SetParent(prefab.transform);
                }
                
                // Move children to group
                foreach (var child in children)
                {
                    if (child.parent != group)
                    {
                        Undo.SetTransformParent(child, group, $"Move to {groupName}");
                    }
                }
                
                // Sort group children
                SortChildrenAlphabetically(group.gameObject);
            }
        }
        
        #endregion
        
        #region Validation Methods
        
        [MenuItem("Tools/Synty/Quick Actions/Sort All Children A-Z", true)]
        [MenuItem("Tools/Synty/Quick Actions/Create Mesh Groups", true)]
        [MenuItem("Tools/Synty/Quick Actions/Create Collider Groups", true)]
        [MenuItem("Tools/Synty/Quick Actions/Create LOD Groups", true)]
        [MenuItem("Tools/Synty/Organize Walls & Floors", true)]
        [MenuItem("Tools/Synty/Organize Props & Decorations", true)]
        [MenuItem("Tools/Synty/Organize Environmental", true)]
        [MenuItem("Tools/Synty/Organize Structural", true)]
        [MenuItem("Tools/Synty/Organize Technical", true)]
        private static bool ValidateSelection()
        {
            return Selection.gameObjects.Length > 0;
        }
        
        #endregion
    }
}