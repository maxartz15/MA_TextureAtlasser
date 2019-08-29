#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;
using MA_Texture;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProExportWindow : EditorWindow
	{
		private const int WindowHeight = 235;

		//Editor
		private static MA_TextureAtlasserProExportWindow thisWindow;
		public static MA_TextureAtlasserProWindow curWindow;

		//Data
		private static bool isLoaded = false;       //Make sure we wait a frame at the start to setup and don't draw.

		//Export settings.
		private ExportPreset exportPreset = ExportPreset.Default;
		private ModelFormat modelFormat = ModelFormat.Obj;
		private TextureFormat textureFormat = TextureFormat.Png;
		private TextureType textureType = TextureType.Default;
		private MA_TextureUtils.TextureScaleMode textureScaleMode = MA_TextureUtils.TextureScaleMode.Bilinear;

		[MenuItem("MA_ToolKit/MA_TextureAtlasserPro/Export Atlas")]	
		private static void Init()
        {
			GetCurrentWindow();

			thisWindow.minSize = new Vector2(420, WindowHeight);
			thisWindow.maxSize = new Vector2(420, WindowHeight);

			thisWindow.titleContent.text = "MA_ExportTextureAtlas";

			thisWindow.Show();
		}

		public static void InitEditorWindow(MA_TextureAtlasserProWindow currentEditorWindow)
		{
			curWindow = currentEditorWindow;

			GetCurrentWindow();

			thisWindow.minSize = new Vector2(420, WindowHeight);
			thisWindow.maxSize = new Vector2(420, WindowHeight);

			thisWindow.titleContent.text = "MA_ExportTextureAtlas";

			thisWindow.Show();
		}

		private static void GetCurrentWindow()
		{
			thisWindow = (MA_TextureAtlasserProExportWindow)EditorWindow.GetWindow<MA_TextureAtlasserProExportWindow>();
		}

		private void CloseWindow()
		{
			if(thisWindow == null)
			{
				GetCurrentWindow();
				thisWindow.Close();
			}
			else
			{
				thisWindow.Close();
			}
		}

		private Event ProcessEvents()
		{
			Event e = Event.current;

			return e;
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnReload()
		{
			//Make sure that when the compiler is finished and reloads the scripts, we are waiting for the next Event.
			isLoaded = false;
		}

		private void OnGUI()
		{
			if(thisWindow == null)
			{
				GetCurrentWindow();
				return;
			}

			//Get current event
            Event e = ProcessEvents();

			if(isLoaded)
			{
				GUILayout.BeginArea(new Rect(MA_TextureAtlasserProUtils.VIEWOFFSET, MA_TextureAtlasserProUtils.VIEWOFFSET, position.width - (MA_TextureAtlasserProUtils.VIEWOFFSET * 2), position.height - (MA_TextureAtlasserProUtils.VIEWOFFSET * 2)));
				GUILayout.BeginVertical();

				
				if(curWindow != null && curWindow.textureAtlas != null)
				{
					//Export
					GUILayout.BeginVertical();

					DrawExportPresetMenu();
					DrawExportAdvancedOptions();

					GUILayout.BeginHorizontal(EditorStyles.helpBox);

					switch (exportPreset)
					{
						case ExportPreset.Custom:
							break;
						case ExportPreset.Default:
							modelFormat = ModelFormat.Obj;
							textureFormat = TextureFormat.Png;
							textureType = TextureType.Default;
							textureScaleMode = MA_TextureUtils.TextureScaleMode.Bilinear;
							break;
						case ExportPreset.Sprites:
							modelFormat = ModelFormat.None;
							textureFormat = TextureFormat.Png;
							textureType = TextureType.SpriteSliced;
							textureScaleMode = MA_TextureUtils.TextureScaleMode.Bilinear;
							break;
						case ExportPreset.ReplaceObjMeshes:
							modelFormat = ModelFormat.ReplaceObj;
							textureFormat = TextureFormat.Png;
							textureType = TextureType.Default;
							textureScaleMode = MA_TextureUtils.TextureScaleMode.Bilinear;
							break;
						default:
							break;
					}

					if (GUILayout.Button("Export", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
					{
						MA_TextureAtlasserProUtils.ExportAtlasModels(curWindow.textureAtlas, modelFormat);
						MA_TextureAtlasserProUtils.ExportAtlasTextures(curWindow.textureAtlas, textureFormat, textureType, textureScaleMode);
					}
					
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
				else if(curWindow == null)
				{
					GUI.backgroundColor = Color.red;
					GUILayout.Box("Error: Link with the Texture Atlas Editor lost!", EditorStyles.helpBox);
					if(GUILayout.Button("Link Atlas Editor", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
					{
						curWindow = (MA_TextureAtlasserProWindow)EditorWindow.GetWindow<MA_TextureAtlasserProWindow>();
					}
					GUI.backgroundColor = Color.white;
				}
				else if(curWindow.textureAtlas == null)
				{
					GUI.backgroundColor = Color.red;
					GUILayout.Box("Error: No Texture Atlas found make sure to open one!", EditorStyles.helpBox);
					GUI.backgroundColor = Color.white;
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}

			if(e.type == EventType.Repaint)
				isLoaded = true;
		}

		private void DrawExportPresetMenu()
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox);

			exportPreset = (ExportPreset)EditorGUILayout.EnumPopup("ExportPreset:", exportPreset, GUILayout.ExpandWidth(true));

			GUILayout.EndHorizontal();
		}

		private void DrawExportAdvancedOptions()
		{
			bool wasEnabled = GUI.enabled;

			if(exportPreset == ExportPreset.Custom)
			{
				GUI.enabled = true;
			}
			else
			{
				GUI.enabled = false;
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			GUILayout.Label("Models:", EditorStyles.miniBoldLabel);
			modelFormat = (ModelFormat)EditorGUILayout.EnumPopup("ModelFormat:", modelFormat);

			GUILayout.Label("Textures:", EditorStyles.miniBoldLabel);
			textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("TextureFormat:", textureFormat);
			textureType = (TextureType)EditorGUILayout.EnumPopup("TextureType:", textureType);
			textureScaleMode = (MA_TextureUtils.TextureScaleMode)EditorGUILayout.EnumPopup("TextureScaleMode:", textureScaleMode);

			EditorGUILayout.EndVertical();

			GUI.enabled = wasEnabled;
		}
	}
}
#endif