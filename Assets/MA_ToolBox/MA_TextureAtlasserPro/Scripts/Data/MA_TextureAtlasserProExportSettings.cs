#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using MA_Texture;
using System.Collections.Generic;

namespace MA_TextureAtlasserPro
{
    [System.Serializable]
    public class MA_TextureAtlasserProExportSettings : ScriptableObject
    {
        [HideInInspector]
        public bool canModify = true;

        public bool exportModels = true;
        public ModelExportSettings modelExportSettings = new ModelExportSettings();
        public bool exportTextures = true;
        public TextureExportSettings textureExportSettings = new TextureExportSettings();
        public bool exportMaterials = true;
        public MaterialExportSettings materialExportSettings = new MaterialExportSettings();
    }

    [System.Serializable]
    public class ModelExportSettings
    {
        [Header("Model settings:")]
        public ModelFormat modelFormat = ModelFormat.UnityMeshPrefab;
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

    [System.Serializable]
    public class MaterialExportSettings
    {
        [Header("Material settings:")]
        public Shader shader = null;
        public List<string> shaderPropertyNames = new List<string>() { "_MainTex", "_MetallicGlossMap", "_BumpMap" };
    }

    public enum ModelFormat
    {
        None,
        UnityMeshPrefab
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
#endif