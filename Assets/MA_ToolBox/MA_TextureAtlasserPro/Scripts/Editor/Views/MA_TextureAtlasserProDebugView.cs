#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProDebugView : MA_TextureAtlasserProViewBase 
	{
		private bool isEditing = false;

		public MA_TextureAtlasserProDebugView(MA_TextureAtlasserProWindow currentEditorWindow, string title) : base(currentEditorWindow, title)
		{
			
		}

		public override void UpdateView(Event e, Rect editorViewRect)
		{
			//Update base derived class
			base.UpdateView(e, editorViewRect);

            //Draw inspector
            if(isLoaded)
            {
				GUILayout.BeginArea(editorViewRect, EditorStyles.helpBox);	
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));				

				//GUILayout.Label(curWindow.workView.Zoom.ToString("F2"));
				if(GUILayout.Button(curWindow.workView.Zoom.ToString("F2"), EditorStyles.label))
				{
					curWindow.workView.ResetWindow();
				}

				if (curWindow.textureAtlas != null)
				{
					GUILayout.FlexibleSpace();
					//GUILayout.Label(curWindow.textureAtlas.textureAtlasSize.ToString());
					if(!isEditing)
					{
						if(GUILayout.Button(curWindow.textureAtlas.textureAtlasSize.ToString("F0"), EditorStyles.label))
						{
							isEditing = true;
						}
					}
					else
					{
						curWindow.textureAtlas.textureAtlasSize = EditorGUILayout.Vector2Field("", curWindow.textureAtlas.textureAtlasSize, GUILayout.Width(110));
					}				
				}
                   
				GUILayout.EndHorizontal();
                GUILayout.EndArea();       
            }

            ProcessEvents(e, editorViewRect);
		}

		protected override void ProcessEvents(Event e, Rect editorViewRect)
		{
            base.ProcessEvents(e, editorViewRect);

			if(isEditing)
			{
				if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
				{
					isEditing = false;
					return;
				}
				if(e.type == EventType.MouseDown && !isMouseInEditorViewRect)
				{
					isEditing = false;
					return;
				}
			}
		}
	}
}
#endif