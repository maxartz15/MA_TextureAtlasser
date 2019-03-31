using UnityEngine;
using UnityEditor;

namespace MA_TextureAtlasserPro
{
    public static class MA_TextureAtlasserProIcons
	{
		private const string LOADICONPATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Icons/";
		
		public static GUIContent createAtlasIcon;
		public static GUIContent loadAtlasIcon;
		public static GUIContent exportAtlasIcon;
		public static GUIContent createQuadIcon;
		public static GUIContent removeQuadIcon;
		public static GUIContent duplicateQuadIcon;
		public static GUIContent showTexturesOnIcon;
		public static GUIContent showTexturesOffIcon;
		public static GUIContent dragHandleIcon;
		public static GUIContent editIcon;

		public static void LoadIcons()
		{
			createAtlasIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "createAtlasIcon" + ".png"));
			loadAtlasIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "loadAtlasIcon" + ".png"));
			exportAtlasIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "exportAtlasIcon" + ".png"));
			createQuadIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "createQuadIcon" + ".png"));
			removeQuadIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "removeQuadIcon" + ".png"));
			duplicateQuadIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "duplicateQuadIcon" + ".png"));
			showTexturesOnIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "showTexturesOnIcon" + ".png"));
			showTexturesOffIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "showTexturesOffIcon" + ".png"));
			dragHandleIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "dragHandleIcon" + ".png"));
			editIcon = new GUIContent("", (Texture)EditorGUIUtility.Load(LOADICONPATH + "editIcon" + ".png"));
		}
	}
}