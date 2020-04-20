//-

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MA_Toolbox.Utils.Editor
{
    public static class MA_PrefabUtils
    {
        public static string CreatePrefab(string prefabName, string savePath)
        {
            if(string.IsNullOrEmpty(prefabName) || string.IsNullOrWhiteSpace(prefabName))
            {
                Debug.LogError("Invalid prefab name.");
                return null;
            }

            GameObject gameObject = new GameObject
            {
                name = prefabName
            };

            string assetPath = savePath + prefabName + ".prefab";

            PrefabUtility.SaveAsPrefabAsset(gameObject, assetPath);
            UnityEngine.Object.DestroyImmediate(gameObject);

            return assetPath;
        }

        public static void AddChild(GameObject prefab, GameObject child)
        {
            GameObject p = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            child.transform.SetParent(p.transform);
            PrefabUtility.ApplyPrefabInstance(p, InteractionMode.AutomatedAction);
            UnityEngine.Object.DestroyImmediate(p);
        }
    }
}
#endif