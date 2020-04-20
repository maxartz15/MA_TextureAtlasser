#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MA_Editor;
using MA_Editor.GUILayoutZoom;
using MA_Editor.RectUtils;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProWorkView : MA_TextureAtlasserProViewBase 
	{
		public MA_TextureAtlasserProWorkView(MA_TextureAtlasserProWindow currentEditorWindow, string title) : base(currentEditorWindow, title)
		{
			zoomCoordsOrigin = new Vector2(-(curWindow.position.width / 2) + (curWindow.position.width / 3), -(curWindow.position.height / 2) + (curWindow.position.height / 3));
		}

		private const float kZoomMin = 0.05f;
        private const float kZoomMax = 2.0f;
		private Rect zoomArea;
		private float zoom = 1.0f;
        public float Zoom { get { return zoom; } set { zoom = Mathf.Clamp(value, kZoomMin, kZoomMax); } }
        private Vector2 zoomCoordsOrigin = Vector2.zero;

		public override void UpdateView(Event e, Rect editorViewRect)
		{
			//Update base derived class
			base.UpdateView(e, editorViewRect);

            zoomArea = editorViewRect;

            bool useEvents;
            if(curWindow.menuView.isMouseInEditorViewRect || curWindow.inspectorView.isMouseInEditorViewRect || curWindow.debugView.isMouseInEditorViewRect)
                useEvents = false;
            else
                useEvents = true;   

            //Draw Work
            if(isLoaded)
            {
                //Start zoom area
                GUILayoutZoom.BeginArea(zoom, zoomArea);

                //Draw quads
                if(curWindow.textureAtlas != null)
                {
                    curWindow.textureAtlas.UpdateTextureQuads(e, editorViewRect, zoomCoordsOrigin, useEvents);
                    DrawQuadGUI();
                }

                //End zoom area
                GUILayoutZoom.EndArea();
            }

            if(useEvents)
                ProcessEvents(e, editorViewRect);
		}

		protected override void ProcessEvents(Event e, Rect editorViewRect)
		{
            base.ProcessEvents(e, editorViewRect);

            // Allow adjusting the zoom with the mouse wheel as well. In this case, use the mouse coordinates
            // as the zoom center instead of the top left corner of the zoom area. This is achieved by
            // maintaining an origin that is used as offset when drawing any GUI elements in the zoom area.
            if (e.type == EventType.ScrollWheel)
            {
                Vector2 screenCoordsMousePos = e.mousePosition;
                Vector2 delta = e.delta;
                Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
                float zoomDelta = -delta.y / 100.0f;
                float oldZoom = zoom;
                zoom += zoomDelta;
                zoom = Mathf.Clamp(zoom, kZoomMin, kZoomMax);
                if(zoom < 1.025f && zoom > 0.995f)
                {
                    zoom = 1;
                }
                zoomCoordsOrigin += (zoomCoordsMousePos - zoomCoordsOrigin) - (oldZoom / zoom) * (zoomCoordsMousePos - zoomCoordsOrigin);
                
				e.Use();
            }

            // Allow moving the zoom area's origin by dragging by dragging with the left mouse button with Alt pressed.
            if (e.type == EventType.MouseDrag && (e.button == 2 || (e.button == 0 && e.modifiers == EventModifiers.Alt)))
            {
                Vector2 delta = Event.current.delta;
                delta /= zoom;
                zoomCoordsOrigin -= delta;

                e.Use();
            }

			//HotKeys.
			if (curWindow.settings.useHotkeys)
			{
				if(curWindow.textureAtlas != null)
				{
					if (curWindow.settings.GetHotKey(e, curWindow.settings.addQuadHotKey))
					{
						MA_TextureAtlasserProUtils.CreateTextureQuad(curWindow.textureAtlas, "new Quad", new Rect(0, 0, 128, 128), curWindow.settings.autoFocus);
						e.Use();
					}

					if(curWindow.settings.GetHotKey(e, curWindow.settings.zoomInHotKey))
					{
						Zoom += 0.25f;
						e.Use();
					}
					if(curWindow.settings.GetHotKey(e, curWindow.settings.zoomOutHotKey))
					{
						Zoom -= 0.25f;
						e.Use();
					}

					if (curWindow.textureAtlas.selectedTextureQuad != null)
					{
						if (curWindow.settings.GetHotKey(e, curWindow.settings.removeQuadHotKey))
						{
							MA_TextureAtlasserProUtils.RemoveTextureQuad(curWindow.textureAtlas, curWindow.settings.autoFocus);
							e.Use();
						}

						if (curWindow.settings.GetHotKey(e, curWindow.settings.duplicateHotKey))
						{
							MA_TextureAtlasserProUtils.DuplicateTextureQuad(curWindow.textureAtlas, curWindow.settings.autoFocus, curWindow.settings.duplicatedQuadNamePrefix);
							e.Use();
						}
					}
				}
			}
		}

		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
        {
            return (screenCoords - zoomArea.TopLeft()) / zoom + zoomCoordsOrigin;
        }

        //Draw quad GUI ontop of quad with custom GUIContent.
        private void DrawQuadGUI()
        {
            if(curWindow.textureAtlas.textureQuads != null)
            {
                foreach (MA_TextureAtlasserProQuad q in curWindow.textureAtlas.textureQuads)
                {
                    if(q.isSelected)
                    {
                        GUI.Button(new Rect(q.dragRectPos.x, q.dragRectPos.y, q.dragRectPos.width, q.dragRectPos.height), MA_TextureAtlasserProGuiLoader.dragHandleGC);
                        GUI.Button(new Rect(q.dragRectWidth.x, q.dragRectWidth.y, q.dragRectWidth.width, q.dragRectWidth.height), MA_TextureAtlasserProGuiLoader.dragHandleGC);
                        GUI.Button(new Rect(q.dragRectHeight.x, q.dragRectHeight.y, q.dragRectHeight.width, q.dragRectHeight.height), MA_TextureAtlasserProGuiLoader.dragHandleGC);
                    }
                }
            }
        }

        public void ResetWindow()
        {
            zoom = 1;
            zoomCoordsOrigin = new Vector2(-(curWindow.position.width / 2) + (curWindow.position.width / 3), -(curWindow.position.height / 2) + (curWindow.position.height / 3));
        }
	}
}
#endif