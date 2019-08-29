//https://github.com/maxartz15/MA_TextureUtils

//References:
//http://www.gamasutra.com/blogs/JoshSutphin/20131007/201829/Adding_to_Unitys_BuiltIn_Classes_Using_Extension_Methods.php
//https://forum.unity3d.com/threads/contribution-texture2d-blur-in-c.185694/
//http://orbcreation.com/orbcreation/page.orb?1180
//https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
//https://github.com/maxartz15/MA_TextureAtlasser/commit/9f5240967a51692fa2a17a6b3c8d124dd5dc60f9

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MA_Texture
{
    public static class MA_TextureUtils
    {
        public static Texture ConvertToReadableTexture(Texture texture)
        {
            if (texture == null)
                return texture;
            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(
                                texture.width,
                                texture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);

            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;

            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;

            // Create a new readable Texture2D to copy the pixels to it
            Texture2D myTexture2D = new Texture2D(texture.width, texture.width);

            // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            myTexture2D.name = texture.name;

            // Reset the active RenderTexture
            RenderTexture.active = previous;

            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);
            // "myTexture2D" now has the same pixels from "texture" and it's readable.

            return myTexture2D;
        }

        #region Save
        public static Texture2D MA_Save2D(this Texture2D texture, string textureName, string savePath)
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            FileStream fs = new FileStream(savePath + "/" + textureName + ".png", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(texture.EncodeToPNG());
            bw.Close();
            fs.Close();

            AssetDatabase.Refresh();

            return texture;
        }

        public static Texture MA_Save(this Texture texture, string name, string savePath)
        {
            Texture2D texture2D = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(texture);

            texture2D.MA_Save2D(name, savePath);

            texture = texture2D;

            return texture;
        }
		#endregion

		#region Scale
		public enum TextureScaleMode
		{
			Bilinear,
			Point
		}

        public static Texture MA_Scale(this Texture texture, int width, int height, TextureScaleMode scaleMode)
        {
			Texture2D texture2D = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(texture);

			texture2D.MA_Scale2D(width, height, scaleMode);

			texture = texture2D;

            return texture;
        }

		public static Texture2D MA_Scale2D(this Texture2D texture, int newWidth, int newHeight, TextureScaleMode scaleMode)
		{
			Color[] curColors = texture.GetPixels();
			Color[] newColors = new Color[newWidth * newHeight];

			switch (scaleMode)
			{
				case TextureScaleMode.Bilinear:
					newColors = MA_BilinearScale(curColors, texture.width, texture.height, newWidth, newHeight);
					break;
				case TextureScaleMode.Point:
					newColors = MA_PointScale(curColors, texture.width, texture.height, newWidth, newHeight);
					break;

			}

			texture.Resize(newWidth, newHeight);
			texture.SetPixels(newColors);
			texture.Apply();

			return texture;
		}

		private static Color[] MA_BilinearScale(Color[] curColors, int curWidth, int curHeight, int newWidth, int newHeight)
		{
			Color[] newColors = new Color[newWidth * newHeight];

			float ratioX = 1.0f / ((float)newWidth / (curWidth - 1));
			float ratioY = 1.0f / ((float)newHeight / (curHeight - 1));

			for (int y = 0; y < newHeight; y++)
			{
				int yFloor = Mathf.FloorToInt(y * ratioY);
				var y1 = yFloor * curWidth;
				var y2 = (yFloor + 1) * curWidth;
				var yw = y * newWidth;

				for (int x = 0; x < newWidth; x++)
				{
					int xFloor = Mathf.FloorToInt(x * ratioX);
					var xLerp = x * ratioX - xFloor;

					newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(curColors[y1 + xFloor], curColors[y1 + xFloor + 1], xLerp),
														ColorLerpUnclamped(curColors[y2 + xFloor], curColors[y2 + xFloor + 1], xLerp),
														y * ratioY - yFloor);
				}
			}

			return newColors;
		}

		private static Color[] MA_PointScale(Color[] curColors, int curWidth, int curHeight, int newWidth, int newHeight)
		{
			Color[] newColors = new Color[newWidth * newHeight];

			float ratioX = ((float)curWidth) / newWidth;
			float ratioY = ((float)curHeight) / newHeight;

			for (int y = 0; y < newHeight; y++)
			{
				var thisY = Mathf.RoundToInt((ratioY * y) * curWidth);
				var yw = y * newWidth;

				for (int x = 0; x < newWidth; x++)
				{
					newColors[yw + x] = curColors[Mathf.RoundToInt(thisY + ratioX * x)];
				}
			}

			return newColors;
		}

		private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
		{
			return new Color(c1.r + (c2.r - c1.r) * value,
							  c1.g + (c2.g - c1.g) * value,
							  c1.b + (c2.b - c1.b) * value,
							  c1.a + (c2.a - c1.a) * value);
		}

		#endregion

		#region combine
		public static Texture2D MA_Combine2D(this Texture2D texture, Texture2D combineTexture, int offsetX, int offsetY, bool flipY = true)
        {
            for (int x = 0; x < combineTexture.width; x++)
            {
                if(flipY)             
                {
                    //Y is 'flipped' because textures are made from left to right, bottom to top. We want to draw from left to right and top to bottom.
                    for (int y = combineTexture.height; y > 0; y--)
                    {
                        texture.SetPixel(x + offsetX, y + (texture.height - offsetY - combineTexture.height), combineTexture.GetPixel(x, y));
                    }
                }
                else
                {
                    for (int y = 0; y < combineTexture.height; y++)
                    {
                        texture.SetPixel(x + offsetX, y + offsetY, combineTexture.GetPixel(x, y));
                    }
                }
            }

            texture.Apply();

            return texture;
        }

        public static Texture MA_Combine(this Texture texture, Texture combineTexture, int offsetX, int offsetY)
        {
            Texture2D texture2D = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(texture);
            Texture2D combineTexture2D = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(texture);

            texture = texture2D.MA_Combine2D(combineTexture2D, offsetX, offsetY);

            return texture;
        }
        #endregion
    }
}
#endif