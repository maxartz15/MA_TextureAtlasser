#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;
using MA_Texture;

namespace MA_TextureAtlasserPro
{
    public class MA_TextureAtlasserProCreateExportWindow : EditorWindow
    {
        private const int windowHeight = 97;
        private const int windowWidth = 320;

        //Editor
        private static MA_TextureAtlasserProCreateExportWindow thisWindow;
        public static MA_TextureAtlasserProWindow curWindow;

        //Data
        string settingsName = "Settings name";
        bool nameError = true;

        [MenuItem("MA_ToolKit/MA_TextureAtlasserPro/New Export Settings")]
        public static void Init()
        {
            InitWindow(null);
        }

        public static void InitWindow(MA_TextureAtlasserProWindow currentEditorWindow)
        {
            curWindow = currentEditorWindow;

            GetCurrentWindow();

            thisWindow.minSize = new Vector2(windowWidth, windowHeight);
            thisWindow.maxSize = new Vector2(windowWidth, windowHeight);
            thisWindow.titleContent.text = "MA_CreateExportSettings";

            thisWindow.Show();
        }

        private static void GetCurrentWindow()
        {
            thisWindow = (MA_TextureAtlasserProCreateExportWindow)EditorWindow.GetWindow<MA_TextureAtlasserProCreateExportWindow>();
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(MA_TextureAtlasserProUtils.VIEW_OFFSET, MA_TextureAtlasserProUtils.VIEW_OFFSET, position.width - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2), position.height - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2)));
            GUILayout.BeginVertical();

            //Input options
            settingsName = EditorGUILayout.TextField("Settings name", settingsName, GUILayout.ExpandWidth(true));
            if (settingsName == "Settings name" || string.IsNullOrEmpty(settingsName))
            {
                nameError = true;
                GUI.backgroundColor = Color.red;
                GUILayout.Box("Error: Enter a valid settings name!", EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;
            }
            else
            {
                nameError = false;
            }

            //Create
            if (!nameError)
            {
                if (GUILayout.Button("Create!", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
                {
                    MA_TextureAtlasserProExportSettings exportSettings = MA_TextureAtlasserProUtils.CreateExportSettings(settingsName, true);

                    if (curWindow != null && curWindow.textureAtlas != null)
                    {
                        curWindow.textureAtlas.exportSettings = exportSettings;
                    }

                    this.Close();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
#endif