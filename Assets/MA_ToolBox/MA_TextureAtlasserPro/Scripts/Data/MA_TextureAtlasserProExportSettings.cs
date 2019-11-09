using UnityEngine;
using System.Collections;
using MA_Texture;

namespace MA_TextureAtlasserPro
{
    [System.Serializable]
    public class MA_TextureAtlasserProExportSettings : ScriptableObject
    {
        [HideInInspector]
        public bool canModify = true;

        public ModelExportSettings modelExportSettings = new ModelExportSettings();
        public TextureExportSettings textureExportSettings = new TextureExportSettings();
    }

    [System.Serializable]
    public class ModelExportSettings
    {
        [Header("Model settings:")]
        public ModelFormat modelFormat = ModelFormat.Obj;
        public bool replaceModel = false;
        public bool uvFlipY = true;
        public int uvChannel = 0;
        public bool uvWrap = true;
    }

    [System.Serializable]
    public class TextureExportSettings
    {
        [Header("Texture settings:")]
        public TextureFormat textureFormat = TextureFormat.Png;
        public TextureType textureType = TextureType.Default;
        public MA_TextureUtils.TextureScaleMode textureScaleMode = MA_TextureUtils.TextureScaleMode.Bilinear;
    }

    public enum ExportPreset
    {
        Custom,
        Default,
        Sprites,
        ReplaceObjMeshes
    }

    public enum ModelFormat
    {
        None,
        Obj
    }

    public enum TextureFormat
    {
        None,
        Png
    }

    public enum TextureType
    {
        Default,
        Sprite,
        SpriteSliced
    }
}