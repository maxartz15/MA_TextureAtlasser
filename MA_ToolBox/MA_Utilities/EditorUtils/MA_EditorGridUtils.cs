//https://github.com/maxartz15/MA_EditorUtils

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MA_Editor.Grid
{
	public static class Grid
	{
		public static void DrawGrid(Rect editorWorkRect, float gridSpacing, Color gridColor)
		{
			//Process grid spacing values
			int widthDivs = Mathf.CeilToInt(editorWorkRect.width / gridSpacing);
			int heightDivs = Mathf.CeilToInt(editorWorkRect.height / gridSpacing);

			//Using handles
			Handles.BeginGUI();
			Handles.color = gridColor;

			for (int x = 0; x < widthDivs; x++)
			{
				Handles.DrawLine(new Vector3(gridSpacing * x, 0, 0), new Vector3(gridSpacing * x, editorWorkRect.height, 0));
			}

			for (int y = 0; y < heightDivs; y++)
			{
				Handles.DrawLine(new Vector3(0, gridSpacing * y, 0), new Vector3(editorWorkRect.width, gridSpacing * y, 0));
			}

			Handles.color = Color.white;
			Handles.EndGUI();
		}

		public static void DrawZoomableGrid(Rect editorWorkRect, float gridSpacing, Color gridColor, Vector2 zoomCoordsOrigin)
		{
			//Process grid spacing values
			int widthDivs = Mathf.CeilToInt(editorWorkRect.width / gridSpacing);
			int heightDivs = Mathf.CeilToInt(editorWorkRect.height / gridSpacing);

			//Using handles
			Handles.BeginGUI();
			Handles.color = gridColor;

			for (int x = 1; x < widthDivs; x++)
			{
				Handles.DrawLine(new Vector3(gridSpacing * x - zoomCoordsOrigin.x, -zoomCoordsOrigin.y, 0), new Vector3(gridSpacing * x - zoomCoordsOrigin.x, editorWorkRect.height - zoomCoordsOrigin.y, 0));
			}

			for (int y = 1; y < heightDivs; y++)
			{
				Handles.DrawLine(new Vector3(-zoomCoordsOrigin.x, gridSpacing * y - zoomCoordsOrigin.y, 0), new Vector3(editorWorkRect.width - zoomCoordsOrigin.x, gridSpacing * y - zoomCoordsOrigin.y, 0));
			}

			Handles.color = Color.white;
			Handles.EndGUI();
		}
	}
}
#endif