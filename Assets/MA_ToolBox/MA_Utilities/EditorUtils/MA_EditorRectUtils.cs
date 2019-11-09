//https://github.com/maxartz15/MA_EditorUtils

//References:
//http://martinecker.com/martincodes/unity-editor-window-zooming/

#if UNITY_EDITOR
using UnityEngine;
using MA_Editor;

namespace MA_Editor.RectUtils
{
	public static class RectUtils
	{
		//Start			http://martinecker.com/martincodes/unity-editor-window-zooming/
		public static Vector2 TopLeft(this Rect rect)
		{
			return new Vector2(rect.xMin, rect.yMin);
		}

		public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}

		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}

		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
		//End			http://martinecker.com/martincodes/unity-editor-window-zooming/

		public static Rect MultiplyRectSize(Rect rect, float amount)
		{
			Rect multipliedRect = new Rect(rect.x, rect.y, rect.width * amount, rect.height * amount);
			return multipliedRect;
		}

		public static Rect MultiplyRectSizeAndCenter(Rect rect, float amount)
		{
			Rect multipliedRect = new Rect(rect.x, rect.y, rect.width * amount, rect.height * amount);
			multipliedRect.x = -(multipliedRect.width / 2);
			multipliedRect.y = -(multipliedRect.height / 2);
			return multipliedRect;
		}
	}
}
#endif