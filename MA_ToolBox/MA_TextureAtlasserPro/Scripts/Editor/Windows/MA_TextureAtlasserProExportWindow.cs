#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProExportWindow : EditorWindow
	{
		private const int WindowHeight = 215;
		
		private enum  ExportMode
		{
			None,
			D3,
			D2,
			Meshes,
		}
		
		//Editor
		private static MA_TextureAtlasserProExportWindow thisWindow;
		public static MA_TextureAtlasserProWindow curWindow;

		//Data
		private static bool isLoaded = false;       //Make sure we wait a frame at the start to setup and don't draw.

		private ExportMode _selectedExportMode;
		private bool _showAdvancedEditor;
		
		private bool exportObjDefault = false;
		private bool _replaceMeshes = false;
		private bool exportPngDefault = false;
		private bool exportSprite = false;
		private bool exportSliceSprite = false;

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

					DrawExportModeEditor();
					DrawAdvancedEditor();

					GUILayout.BeginHorizontal(EditorStyles.helpBox);

					GUI.enabled = _selectedExportMode != ExportMode.None;
					if (GUILayout.Button("Export", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
					{
						if(exportObjDefault)
						{
							MA_TextureAtlasserProUtils.ExportAtlasMeshesObj(curWindow.textureAtlas);
						}

						if (_replaceMeshes)
						{
							
						}

						if(exportPngDefault)
						{
							if(exportSprite)
							{
								MA_TextureAtlasserProUtils.ExportAtlasSpritesPNG(curWindow.textureAtlas, exportSliceSprite);
							}
							else
							{
								MA_TextureAtlasserProUtils.ExportAtlasTexturesPNG(curWindow.textureAtlas);
							}
						}
					}

					GUI.enabled = true;
					
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

		private void DrawExportModeEditor()
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox);
			GUILayout.FlexibleSpace();
			var value = GUILayout.Toggle(_selectedExportMode == ExportMode.D3, "3D", EditorStyles.miniButtonLeft, 
				GUILayout.ExpandWidth(false));
			if (value && _selectedExportMode != ExportMode.D3)
			{
				_selectedExportMode = ExportMode.D3;
				exportObjDefault = true;
				_replaceMeshes = false;
				exportPngDefault = true;
				exportSprite = false;
				exportSliceSprite = false;
			}
			value = GUILayout.Toggle(_selectedExportMode == ExportMode.D2, "2D", EditorStyles.miniButtonMid, 
				GUILayout.ExpandWidth(false));
			if (value && _selectedExportMode != ExportMode.D2)
			{
				_selectedExportMode = ExportMode.D2;
				exportObjDefault = false;
				_replaceMeshes = false;
				exportPngDefault = true;
				exportSprite = true;
				exportSliceSprite = true;
			}
			value = GUILayout.Toggle(_selectedExportMode == ExportMode.Meshes, "Replace source meshes", EditorStyles.miniButtonRight, 
				GUILayout.ExpandWidth(false));
			if (value && _selectedExportMode != ExportMode.Meshes)
			{
				_selectedExportMode = ExportMode.Meshes;
				exportObjDefault = false;
				_replaceMeshes = true;
				exportPngDefault = true;
				exportSprite = false;
				exportSliceSprite = false;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawAdvancedEditor()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			_showAdvancedEditor = EditorGUILayout.Foldout(_showAdvancedEditor, "Advanced editor");
			if (!_showAdvancedEditor)
			{
				EditorGUILayout.EndVertical();	
				return;
			}

			GUILayout.Label("Meshes:", EditorStyles.miniBoldLabel);
			EditorGUILayout.BeginHorizontal();
			exportObjDefault = GUILayout.Toggle(exportObjDefault, "OBJ default.");
			_replaceMeshes = GUILayout.Toggle(_replaceMeshes, "Replace meshes");
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Label("Textures:", EditorStyles.miniBoldLabel);
			GUILayout.BeginHorizontal();
			exportPngDefault = GUILayout.Toggle(exportPngDefault, "PNG default.");
			if(exportPngDefault)
			{
				exportSprite = GUILayout.Toggle(exportSprite, "Sprite.");
				if (exportSprite)
				{
					exportSliceSprite = GUILayout.Toggle(exportSliceSprite, "Slice sprites.");
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
	}
}
#endif