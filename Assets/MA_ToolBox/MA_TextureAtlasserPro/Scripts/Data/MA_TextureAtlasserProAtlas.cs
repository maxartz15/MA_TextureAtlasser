#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;
using MA_Editor.Grid;

namespace MA_TextureAtlasserPro
{
	[System.Serializable]
	public class MA_TextureAtlasserProAtlas : ScriptableObject
	{
		//Editor
		public List<MA_TextureAtlasserProQuad> textureQuads;
		public MA_TextureAtlasserProQuad selectedTextureQuad;
		private Rect editorWorkRect;
		public bool showTextures = true;
        public MA_TextureAtlasserProExportSettings exportSettings;

		//Data
		public Vector2 textureAtlasSize;
		public List<MA_TextureGroupRegistration> textureGroupRegistration;

		public void CreateAtlas(string name, Vector2 size)
		{
			this.name = name;
			textureAtlasSize = size;
		}

		public void UpdateTextureQuads(Event e, Rect editorViewRect, Vector2 zoomCoordsOrigin, bool useEvents)
		{
			textureAtlasSize.x = Mathf.Clamp(textureAtlasSize.x, 128, 8192);
			textureAtlasSize.y = Mathf.Clamp(textureAtlasSize.y, 128, 8192);

			editorWorkRect = new Rect(Vector2.zero - zoomCoordsOrigin, textureAtlasSize);

			GUI.backgroundColor = new Color(0, 0, 0, 0.1f);
			GUI.Box(editorWorkRect, "");
			GUI.Box(new Rect(editorWorkRect.x, editorWorkRect.y - 25, editorWorkRect.width, 20), this.name);
			GUI.backgroundColor = Color.white;

			MA_Editor.Grid.Grid.DrawZoomableGrid(editorWorkRect, 64, new Color(0, 0, 0, 0.1f), zoomCoordsOrigin);

			if(textureQuads != null)
			{
				foreach (MA_TextureAtlasserProQuad ts in textureQuads)
				{
					ts.UpdateTextureQuad(e, editorViewRect, editorWorkRect, zoomCoordsOrigin, useEvents, showTextures);
				}
			}

			if(useEvents)
				ProcessEvents(e, zoomCoordsOrigin, useEvents);

			EditorUtility.SetDirty(this);
		}

		private void ProcessEvents(Event e, Vector2 zoomCoordsOrigin, bool useEvents)
		{
			if(e.button == 0)
			{
				if(e.type == EventType.MouseDown)
				{
					DeselectQuad();

					if(textureQuads != null)
					{
						foreach(MA_TextureAtlasserProQuad quad in textureQuads)
						{
							if(new Rect((int)quad.guiRect.x - zoomCoordsOrigin.x, (int)quad.guiRect.y - zoomCoordsOrigin.y, quad.guiRect.width, quad.guiRect.height).Contains(e.mousePosition))
							{
								SelectQuad(quad);
								e.Use();
							}
						}
					}
				}
			}	
		}

		private void SelectQuad(MA_TextureAtlasserProQuad quad)
		{
			if(selectedTextureQuad)
			{
				DeselectQuad();
			}

			quad.isSelected = true;
			selectedTextureQuad = quad;
		}

		private void DeselectQuad()
		{
			if(textureQuads != null)
			{
				foreach(MA_TextureAtlasserProQuad quad in textureQuads)
				{
					quad.isSelected = false;
				}
				selectedTextureQuad = null;
			}
		}
	}
}
#endif