using System.Threading;
using UnityEngine;

namespace MA_Texture
{
	public static class TextureScaler
	{
		public class ThreadData
		{
			public int start;
			public int end;
			public ThreadData (int s, int e) {
				start = s;
				end = e;
			}
		}
	 
		private static Color[] texColors;
		private static Color[] newColors;
		private static int w;
		private static float ratioX;
		private static float ratioY;
		private static int w2;
		private static int finishCount;
	 
		public static void Scale (Texture2D tex, int newWidth, int newHeight, bool useBilinear)
		{
			texColors = tex.GetPixels();
			newColors = new Color[newWidth * newHeight];
			if (useBilinear)
			{
				ratioX = 1.0f / ((float)newWidth / (tex.width-1));
				ratioY = 1.0f / ((float)newHeight / (tex.height-1));
			}
			else {
				ratioX = ((float)tex.width) / newWidth;
				ratioY = ((float)tex.height) / newHeight;
			}
			w = tex.width;
			w2 = newWidth;
			finishCount = 0;
			if (useBilinear)
			{
				BilinearScale(0, newHeight);
			}
			else
			{
				PointScale(0, newHeight);
			}
			
	 
			tex.Resize(newWidth, newHeight);
			tex.SetPixels(newColors);
			tex.Apply();
	 
			texColors = null;
			newColors = null;
		}
	 
		public static void BilinearScale (int start, int end)
		{
			for (var y = start; y < end; y++)
			{
				int yFloor = (int)Mathf.Floor(y * ratioY);
				var y1 = yFloor * w;
				var y2 = (yFloor+1) * w;
				var yw = y * w2;
	 
				for (var x = 0; x < w2; x++) {
					int xFloor = (int)Mathf.Floor(x * ratioX);
					var xLerp = x * ratioX-xFloor;
					newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor+1], xLerp),
														   ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor+1], xLerp),
														   y*ratioY-yFloor);
				}
			}
	 
			finishCount++;
		}
	 
		public static void PointScale (int start, int end)
		{
			for (var y = start; y < end; y++)
			{
				var thisY = (int)(ratioY * y) * w;
				var yw = y * w2;
				for (var x = 0; x < w2; x++) {
					newColors[yw + x] = texColors[(int)(thisY + ratioX*x)];
				}
			}
	 
			finishCount++;
		}
	 
		private static Color ColorLerpUnclamped (Color c1, Color c2, float value)
		{
			return new Color (c1.r + (c2.r - c1.r)*value, 
							  c1.g + (c2.g - c1.g)*value, 
							  c1.b + (c2.b - c1.b)*value, 
							  c1.a + (c2.a - c1.a)*value);
		}
	}
}