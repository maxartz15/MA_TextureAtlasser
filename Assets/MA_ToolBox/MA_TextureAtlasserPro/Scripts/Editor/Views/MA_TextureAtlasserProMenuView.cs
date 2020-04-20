#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProMenuView : MA_TextureAtlasserProViewBase 
	{
		public MA_TextureAtlasserProMenuView(MA_TextureAtlasserProWindow currentEditorWindow, string title) : base(currentEditorWindow, title)
		{
			
		}

		public override void UpdateView(Event e, Rect editorViewRect)
		{
			//Update base derived class
			base.UpdateView(e, editorViewRect);

			//Draw Menu
			if(isLoaded)
			{
				GUILayout.BeginArea(editorViewRect, EditorStyles.helpBox);	
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

				if(GUILayout.Button(MA_TextureAtlasserProGuiLoader.createAtlasGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
				{
					MA_TextureAtlasserProCreateWindow.InitEditorWindow(curWindow);
				}
				if(GUILayout.Button(MA_TextureAtlasserProGuiLoader.loadAtlasGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
				{
					curWindow.textureAtlas = MA_TextureAtlasserProUtils.LoadTextureAtlas();
				}

				if(curWindow.textureAtlas != null)
				{
					if(GUILayout.Button(MA_TextureAtlasserProGuiLoader.exportAtlasGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
					{
						MA_TextureAtlasserProExportWindow.InitEditorWindow(curWindow);
						//MA_TextureAtlasserProUtils.ExportAtlas(curWindow.textureAtlas);
					}
					GUILayout.Space(MA_TextureAtlasserProUtils.VIEW_OFFSET);
					if(curWindow.textureAtlas.showTextures && GUILayout.Button(MA_TextureAtlasserProGuiLoader.showTexturesOnGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
					{
						curWindow.textureAtlas.showTextures = false;
					}
					else if(!curWindow.textureAtlas.showTextures && GUILayout.Button(MA_TextureAtlasserProGuiLoader.showTexturesOffGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
					{
						curWindow.textureAtlas.showTextures = true;
					}
					GUILayout.Space(MA_TextureAtlasserProUtils.VIEW_OFFSET);
					if(GUILayout.Button(MA_TextureAtlasserProGuiLoader.createQuadGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
					{
						MA_TextureAtlasserProUtils.CreateTextureQuad(curWindow.textureAtlas, "new Quad", new Rect(0, 0, 128, 128), curWindow.settings.autoFocus);
					}
					if(curWindow.textureAtlas.selectedTextureQuad != null && GUILayout.Button(MA_TextureAtlasserProGuiLoader.removeQuadGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
					{
						if(curWindow.textureAtlas.selectedTextureQuad != null)
							MA_TextureAtlasserProUtils.RemoveTextureQuad(curWindow.textureAtlas, curWindow.settings.autoFocus);		
					}
					if (curWindow.textureAtlas.selectedTextureQuad != null && GUILayout.Button(MA_TextureAtlasserProGuiLoader.duplicateQuadGC, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
					{
						if (curWindow.textureAtlas.selectedTextureQuad != null)
							MA_TextureAtlasserProUtils.DuplicateTextureQuad(curWindow.textureAtlas, curWindow.settings.autoFocus, curWindow.settings.duplicatedQuadNamePrefix);
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
		}
	}
}
#endif