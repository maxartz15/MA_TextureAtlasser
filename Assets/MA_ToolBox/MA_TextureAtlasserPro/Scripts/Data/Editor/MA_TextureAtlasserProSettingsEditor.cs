#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MA_TextureAtlasserPro
{
    [CustomEditor(typeof(MA_TextureAtlasserProSettings))]
    [CanEditMultipleObjects]
    public class MA_TextureAtlasserProSettingsEditor : Editor
    {
        GUIContent reloadButton = new GUIContent("Reload", "Update the editor with the changes made.");

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();
            DrawDefaultInspector();

            GUILayout.Space(15);

            if(GUILayout.Button(reloadButton, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
            {
                //Update necessary systems.
                MA_TextureAtlasserProGuiLoader.LoadEditorGui((MA_TextureAtlasserProSettings)this.target);
            }

            GUILayout.EndVertical();
        }
    }
}
#endif