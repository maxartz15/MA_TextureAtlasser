using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProExportWindow : EditorWindow 
	{
		//Editor
		private static MA_TextureAtlasserProExportWindow thisWindow;
		public static MA_TextureAtlasserProWindow curWindow;

		//Data
		private static bool isLoaded = false;		//Make sure we wait a frame at the start to setup and don't draw.

		[MenuItem("MA_ToolKit/MA_TextureAtlasserPro/Export Atlas")]	
		private static void Init()
        {
			GetCurrentWindow();

			thisWindow.minSize = new Vector2(500,160);
			thisWindow.maxSize = new Vector2(500,160);

			thisWindow.titleContent.text = "MA_ExportTextureAtlas";

			thisWindow.Show();
		}

		public static void InitEditorWindow(MA_TextureAtlasserProWindow currentEditorWindow)
		{
			curWindow = currentEditorWindow;

			GetCurrentWindow();

			thisWindow.minSize = new Vector2(500,160);
			thisWindow.maxSize = new Vector2(500,160);

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
					//Export options
					GUILayout.Box("Note: No custom export options right now.. :<", EditorStyles.helpBox);

					//Export
					GUILayout.BeginVertical(EditorStyles.helpBox);

					GUILayout.Label("Meshes: OBJ | Textures: PNG");
					if(GUILayout.Button("Export Atlas", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
					{
						MA_TextureAtlasserProUtils.ExportAtlas(curWindow.textureAtlas);
					}

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
	}
}