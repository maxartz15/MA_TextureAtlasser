//Maxartz15
//Version 1.0
//Part of MA_TextureUtils
//https://github.com/maxartz15/MA_TextureUtils

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//http://www.gamasutra.com/blogs/JoshSutphin/20131007/201829/Adding_to_Unitys_BuiltIn_Classes_Using_Extension_Methods.php
//https://forum.unity3d.com/threads/contribution-texture2d-blur-in-c.185694/
//http://orbcreation.com/orbcreation/page.orb?1180
//https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-

namespace MA_Texture
{
    public static class MA_TextureUtils
    {
        /// <summary>
        /// Some base converters and texture settings setters.
        /// </summary>

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

            Debug.Log("Saved texture: " + texture.name);

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
        public static Texture2D MA_Scale2D(this Texture2D texture, int newWidth, int newHeight)
        {
            Texture2D texture2D = new Texture2D(newWidth, newHeight);
            float ratioWidth = texture.width / newWidth;
            float ratioHeight = texture.height / newHeight;

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    int posX = Mathf.FloorToInt(x / ratioWidth);
                    int posY = Mathf.FloorToInt(y / ratioHeight);
                    texture2D.SetPixel(posX, posY, new Color(pixel.r, pixel.g, pixel.b, pixel.a));
                }
            }
            texture2D.Apply();

            return texture2D;
        }

        public static Texture MA_Scale(this Texture texture, int newWidth, int newHeight)
        {
            Texture2D texture2D = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(texture);

            texture2D.MA_Scale2D(newWidth, newHeight);

            texture = texture2D;

            return texture;
        }

        public static Texture2D MA_Scale22D(this Texture2D texture, float width, float height)
        {
            float ratioWidth = width / texture.width;
            float ratioHeight = height / texture.height;

            int newWidth = Mathf.RoundToInt(texture.width * ratioWidth);
            int newHeight = Mathf.RoundToInt(texture.height * ratioHeight);

            Texture2D newTexture = new Texture2D(newWidth, newHeight);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    int posX = Mathf.RoundToInt(x * ratioWidth);
                    int posY = Mathf.RoundToInt(y * ratioHeight);
                    newTexture.SetPixel(posX, posY, new Color(pixel.r, pixel.g, pixel.b, pixel.a));
                }
            }

            newTexture.name = texture.name;

            newTexture.Apply();
            return newTexture;
        }

        public static Texture MA_Scale2(this Texture texture, float newWidth, float newHeight)
        {
            Texture2D texture2D = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(texture);

            texture = texture2D.MA_Scale22D(newWidth, newHeight);

            return texture;
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
                        texture.SetPixel(x + offsetX, texture.height - y - offsetY, combineTexture.GetPixel(x, texture.height - y));
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