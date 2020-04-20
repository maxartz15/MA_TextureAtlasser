#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	[System.Serializable]
	public class MA_TextureAtlasserProQuad : ScriptableObject
	{
		//Editor
		[HideInInspector]
		public bool isSelected = false;             //Is this thing selected
		public Rect rect;                           //The internal rect
		[HideInInspector]
		public Rect guiRect;                        //The visual clamped and snapped rect
		[HideInInspector]
		public bool debugMode = false;              //Are we debugging, for showing some other things (like handles)

		private bool isDragging = false;            //Are we editing the pos or size
		private bool isDraggingRectHeigt = false;
		[HideInInspector]
		public Rect dragRectHeight;
		private bool isDraggingRectWidth = false;
		[HideInInspector]
		public Rect dragRectWidth;
		private bool isDraggingRectPos = false;
		[HideInInspector]
		public Rect dragRectPos;

		//Data
		public List<MA_TextureGroup> textureGroups;
		public List<MA_ModelGroup> modelGroups;

		public void UpdateTextureQuad(Event e, Rect editorViewRect, Rect editorWorkRect, Vector2 zoomCoordsOrigin, bool useEvents, bool showTexture)
		{
			if(isSelected)
			{
				GUI.backgroundColor = new Color(0.05f, 0.05f, 0.05f, 0.75f);
			}
			else
			{
				GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
			}

			//Clamp and snap the guiRect
			guiRect = new Rect(Mathf.RoundToInt(rect.x / 32) * 32, Mathf.RoundToInt(rect.y / 32) * 32, Mathf.RoundToInt(rect.width / 32) * 32, Mathf.RoundToInt(rect.height / 32) * 32);

			//Draw the quad background
			if(showTexture && textureGroups != null && textureGroups.Count > 0 && textureGroups[0].texture != null)
				GUI.DrawTexture(new Rect(guiRect.x - zoomCoordsOrigin.x, guiRect.y - zoomCoordsOrigin.y, guiRect.width, guiRect.height), textureGroups[0].texture, ScaleMode.StretchToFill);
			else
				GUI.Box(new Rect(guiRect.x - zoomCoordsOrigin.x, guiRect.y - zoomCoordsOrigin.y, guiRect.width, guiRect.height), "");

			GUILayout.BeginArea(new Rect(guiRect.x - zoomCoordsOrigin.x, guiRect.y - zoomCoordsOrigin.y, guiRect.width, guiRect.height));

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var tempColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(1, 1, 1, 0.7f);
			GUILayout.Label(" " + this.name + " ", GUI.skin.box);
			GUI.backgroundColor = tempColor;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if(isSelected)
			{
				dragRectPos = new Rect(guiRect.width / 2 + guiRect.x - zoomCoordsOrigin.x - 16, guiRect.height / 2 + guiRect.y - zoomCoordsOrigin.y - 16, 32, 32);
				dragRectWidth = new Rect(guiRect.width + guiRect.x - zoomCoordsOrigin.x - 16, guiRect.height / 2 + guiRect.y - zoomCoordsOrigin.y - 32, 16, 64);
				dragRectHeight = new Rect(guiRect.width / 2 + guiRect.x - zoomCoordsOrigin.x - 32, guiRect.height + guiRect.y - zoomCoordsOrigin.y - 16, 64, 16);

				if(debugMode)
				{
					GUI.Box(new Rect(dragRectPos.x - guiRect.x + zoomCoordsOrigin.x, dragRectPos.y - guiRect.y + zoomCoordsOrigin.y, dragRectPos.width, dragRectPos.height), "");
					GUI.Box(new Rect(dragRectWidth.x - guiRect.x + zoomCoordsOrigin.x, dragRectWidth.y - guiRect.y + zoomCoordsOrigin.y, dragRectWidth.width, dragRectWidth.height), "");
					GUI.Box(new Rect(dragRectHeight.x - guiRect.x + zoomCoordsOrigin.x, dragRectHeight.y - guiRect.y + zoomCoordsOrigin.y, dragRectHeight.width, dragRectHeight.height), "");
				}
			}
			else
			{

			}

			GUI.backgroundColor = Color.white;
			GUILayout.EndArea();

			if(useEvents)
				ProcessEvents(e, editorViewRect, editorWorkRect, zoomCoordsOrigin);

			EditorUtility.SetDirty(this);
		}

		void ProcessEvents(Event e, Rect editorViewRect, Rect editorWorkRect, Vector2 zoomCoordsOrigin)
		{
			if(isSelected)
			{
				//Right mouse
				if(e.button == 0)
				{
					//Mouse drag
					if(e.type == EventType.MouseDrag)
					{
						if(dragRectPos.Contains(e.mousePosition) && isDragging == false)
						{
							//Debug.Log("P");
							isDragging = true;
							isDraggingRectPos = true;
						}
						if(dragRectWidth.Contains(e.mousePosition) && isDragging == false)
						{
							//Debug.Log("W");
							isDragging = true;
							isDraggingRectWidth = true;
						}
						if(dragRectHeight.Contains(e.mousePosition) && isDragging == false)
						{
							//Debug.Log("W");
							isDragging = true;
							isDraggingRectHeigt = true;
						}

						if(isDraggingRectPos)
						{
							rect.x += e.delta.x;
							rect.y += e.delta.y;
						}
						if(isDraggingRectWidth)
						{
							rect.width += e.delta.x;
						}
						if(isDraggingRectHeigt)
						{
							rect.height += e.delta.y;
						}

						//Clamp rect with min/max values to stay inside the workrect
						rect.width = Mathf.Clamp(rect.width, 64, editorWorkRect.width);
						rect.height = Mathf.Clamp(rect.height, 64, editorWorkRect.height);
						rect.x = Mathf.Clamp(rect.x, 0, editorWorkRect.width - rect.width);
						rect.y = Mathf.Clamp(rect.y, 0, editorWorkRect.height - rect.height);

						if(isDragging)
							e.Use();
					}
				}
				//Deselect on mouse up
				if(e.type == EventType.MouseUp)
				{
					StopDragging();
				}
			}
			//Stop if we are not selected
			else if(!isSelected && isDragging)
			{
				StopDragging();
			}
		}

		private void StopDragging()
		{
			//Debug.Log("StopDragging");
			isDragging = false;
			isDraggingRectPos = false;
			isDraggingRectWidth = false;
			isDraggingRectHeigt = false;		
		}

		public void SetDebugMode(bool isDebugging)
		{
			debugMode = isDebugging;
		}
	}
}
#endif