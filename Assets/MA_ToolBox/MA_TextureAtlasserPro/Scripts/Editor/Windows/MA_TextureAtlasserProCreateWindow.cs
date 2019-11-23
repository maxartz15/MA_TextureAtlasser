#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProCreateWindow : EditorWindow 
	{
		//Editor
		private static MA_TextureAtlasserProCreateWindow thisWindow;
		public static MA_TextureAtlasserProWindow curWindow;
				
		private bool linkedAtlasSize = true;
		private bool nameError = true;
		private bool sizeError = true;

		//Data
		private string textureAtlasName = "Atlas name";
		private Vector2 textureAtlasSize = new Vector2(512, 512);
		private MA_TextureAtlasserProAtlas textureAtlas;
		private static bool isLoaded = false;

		[MenuItem("MA_ToolKit/MA_TextureAtlasserPro/New Atlas")]	
		private static void Init()
        {
			GetCurrentWindow();

			thisWindow.minSize = new Vector2(500,160);
			thisWindow.maxSize = new Vector2(500,160);

			thisWindow.titleContent.text = "MA_CreateTextureAtlas";

			thisWindow.Show();
		}

		public static void InitEditorWindow(MA_TextureAtlasserProWindow currentEditorWindow)
		{
			curWindow = currentEditorWindow;

			GetCurrentWindow();

			thisWindow.minSize = new Vector2(500,160);
			thisWindow.maxSize = new Vector2(500,160);

			thisWindow.titleContent.text = "MA_CreateTextureAtlas";

			thisWindow.Show();
		}

		private static void GetCurrentWindow()
		{
			thisWindow = (MA_TextureAtlasserProCreateWindow)EditorWindow.GetWindow<MA_TextureAtlasserProCreateWindow>();
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
		static void OnReload()
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
				GUILayout.BeginArea(new Rect(MA_TextureAtlasserProUtils.VIEW_OFFSET, MA_TextureAtlasserProUtils.VIEW_OFFSET, position.width - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2), position.height - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2)));
				GUILayout.BeginVertical();

				//Input options
				textureAtlasName = EditorGUILayout.TextField("Atlas name", textureAtlasName, GUILayout.ExpandWidth(true));
				if(textureAtlasName == "Atlas name" || string.IsNullOrEmpty(textureAtlasName))
				{
					nameError = true;
					GUI.backgroundColor = Color.red;
					GUILayout.Box("Error: Enter a valid atlas name!", EditorStyles.helpBox);
					GUI.backgroundColor = Color.white;
				}
				else
				{
					nameError = false;
				}
				
				textureAtlasSize.x = EditorGUILayout.IntField("Atlas width", (int)textureAtlasSize.x, GUILayout.ExpandWidth(true));
				if(linkedAtlasSize)
				{
					linkedAtlasSize = EditorGUILayout.Toggle("link height", linkedAtlasSize, GUILayout.ExpandWidth(true));
					textureAtlasSize.y = textureAtlasSize.x;
				}
				else
				{
					textureAtlasSize.y = EditorGUILayout.IntField("Atlas height", (int)textureAtlasSize.y, GUILayout.ExpandWidth(true));
				}
				if(!Mathf.IsPowerOfTwo((int)textureAtlasSize.x) || !Mathf.IsPowerOfTwo((int)textureAtlasSize.y))
				{
					GUI.backgroundColor = Color.yellow;
					GUILayout.Box("Warning: Atlas size value isn't a power of two!", EditorStyles.helpBox);
					GUI.backgroundColor = Color.white;
				}
				if(textureAtlasSize.x < 64 || textureAtlasSize.y < 64)
				{
					sizeError = true;
					GUI.backgroundColor = Color.red;
					GUILayout.Box("Error: The minimum atlas size is 64!", EditorStyles.helpBox);
					GUI.backgroundColor = Color.white;
				}
				else
				{
					sizeError = false;
				}

				//Create
				if(!nameError && !sizeError)
				{
					if(GUILayout.Button("Create Atlas", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
					{
						textureAtlas = MA_TextureAtlasserProUtils.CreateTextureAtlas(textureAtlasName, textureAtlasSize);

						if(curWindow != null)
						{
							curWindow.textureAtlas = textureAtlas;				
						}
						else
						{
							Debug.Log("No editor window found");
						}

						CloseWindow();
					}
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}

			if(e.type == EventType.Repaint)
				isLoaded = true;
		}
	}
}
#endif