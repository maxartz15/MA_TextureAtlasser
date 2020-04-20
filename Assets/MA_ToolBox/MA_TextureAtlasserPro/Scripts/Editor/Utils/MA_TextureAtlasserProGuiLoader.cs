#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MA_TextureAtlasserPro
{
    public static class MA_TextureAtlasserProGuiLoader
	{
		private const string LOADICONPATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Icons/";
		
		public static GUIContent createAtlasGC;
		public static GUIContent loadAtlasGC;
		public static GUIContent exportAtlasGC;
		public static GUIContent createQuadGC;
		public static GUIContent removeQuadGC;
		public static GUIContent duplicateQuadGC;
		public static GUIContent showTexturesOnGC;
		public static GUIContent showTexturesOffGC;
		public static GUIContent dragHandleGC;
		public static GUIContent editGC;
        public static GUIContent createExportSettingsGC;

		public static void LoadEditorGui(MA_TextureAtlasserProSettings settings)
		{
            createAtlasGC = new GUIContent();
            loadAtlasGC = new GUIContent();
            exportAtlasGC = new GUIContent();
            createQuadGC = new GUIContent();
            removeQuadGC = new GUIContent();
            duplicateQuadGC = new GUIContent();
            showTexturesOnGC = new GUIContent();
            showTexturesOffGC = new GUIContent();
            dragHandleGC = new GUIContent();
            editGC = new GUIContent();
            createExportSettingsGC = new GUIContent();

            switch (settings.editorGuiSettings.editorGuiMode)
			{
				case MA_EditorGuiMode.IconAndText:
                    LoadIcons();
                    LoadText();
                    break;
				case MA_EditorGuiMode.Icon:
                    LoadIcons();
					break;
				case MA_EditorGuiMode.Text:
                    LoadText();
					break;
				default:
                    LoadIcons();
					break;
			}

            if(settings.editorGuiSettings.enableToolTips)
            {
                LoadToolTips(settings);
            }

            //Exceptions.
            dragHandleGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "dragHandleIcon" + ".png");
            dragHandleGC.text = "";
            editGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "editIcon" + ".png");
            editGC.text = "";
        }

		private static void LoadIcons()
		{
			createAtlasGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "createAtlasIcon" + ".png");
            loadAtlasGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "loadAtlasIcon" + ".png");
            exportAtlasGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "exportAtlasIcon" + ".png");
            createQuadGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "createQuadIcon" + ".png");
            removeQuadGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "removeQuadIcon" + ".png");
            duplicateQuadGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "duplicateQuadIcon" + ".png");
            showTexturesOnGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "showTexturesOnIcon" + ".png");
            showTexturesOffGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "showTexturesOffIcon" + ".png");
            createExportSettingsGC.image = (Texture)EditorGUIUtility.Load(LOADICONPATH + "createAtlasIcon" + ".png");
        }

        private static void LoadText()
        {
            createAtlasGC.text = "Create Atlas";
            loadAtlasGC.text = "Load Atlas";
            exportAtlasGC.text = "Export Atlas";
            createQuadGC.text = "Create Quad";
            removeQuadGC.text = "Remove Quad";
            duplicateQuadGC.text = "Duplicate Quad";
            showTexturesOnGC.text = "Hide Textures";
            showTexturesOffGC.text = "Show Textures";
            createExportSettingsGC.text = "Create Export Settings";
        }
        
        private static void LoadToolTips(MA_TextureAtlasserProSettings settings)
        {
            createAtlasGC.tooltip = "Opens the create atlas window.";
            loadAtlasGC.tooltip = "Load an existing atlas.";
            exportAtlasGC.tooltip = "Opens the export window.";
            if (settings.useHotkeys) 
            {
                createQuadGC.tooltip = string.Format("({0}+{1}), Creates a new quad.", settings.modifierKey, settings.addQuadHotKey);
                removeQuadGC.tooltip = string.Format("({0}+{1}), Removes the selected quad.", settings.modifierKey, settings.removeQuadHotKey);
                duplicateQuadGC.tooltip = string.Format("({0}+{1}), Duplicates the selected quad.", settings.modifierKey, settings.duplicateHotKey);            
            }
            else
            {
                createQuadGC.tooltip = "Creates a new quad.";
                removeQuadGC.tooltip = "Removes the selected quad.";
                duplicateQuadGC.tooltip = "Duplicates the selected quad.";
            }
            showTexturesOnGC.tooltip = "Hides the preview of the first texture on the quads.";
            showTexturesOffGC.tooltip = "Shows a preview of the first texture on the quads.";
            createExportSettingsGC.tooltip = "Opens the create export settings window.";
        }
	}
}
#endif