#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProViewBase
	{
		public MA_TextureAtlasserProWindow curWindow;
		public string viewTitle;
		public bool isMouseInEditorViewRect = false;
		public static bool editorIsLoaded = false;
		public bool isLoaded = false;

		public MA_TextureAtlasserProViewBase(MA_TextureAtlasserProWindow currentEditorWindow, string title)
		{
			curWindow = currentEditorWindow;
			viewTitle = title;
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		static void OnReload()
		{
			//Make sure that when the compiler is finished and reloads the scripts, we are waiting for the next Event.
			editorIsLoaded = false;
		}

		public virtual void UpdateView(Event e, Rect editorViewRect)
		{

		}

		protected virtual void ProcessEvents(Event e, Rect editorViewRect)
		{
			if(e.type == EventType.Repaint)
			{
				if(!isLoaded && editorIsLoaded)
					isLoaded = true;

				if(!editorIsLoaded)
					editorIsLoaded = true;
			}

			if(editorViewRect.Contains(e.mousePosition))
			{
				isMouseInEditorViewRect = true;
			}
			else
			{
				isMouseInEditorViewRect = false;
			}
		}
	}
}
#endif