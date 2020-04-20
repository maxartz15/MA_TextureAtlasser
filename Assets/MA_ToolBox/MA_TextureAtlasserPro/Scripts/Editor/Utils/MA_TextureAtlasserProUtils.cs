#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Mesh;
using MA_Texture;
using MA_Toolbox.Utils.Editor;
using MA_Toolbox.Utils;

namespace MA_TextureAtlasserPro
{
    public static class MA_TextureAtlasserProUtils
    {
        public const string TEXTURE_ATLASSER_PATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/";
        public const string SETTINGS_ASSET_PATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Settings/";
        public const string SAVE_ASSET_PATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Atlasses/";
        public const string LOAD_ASSET_PATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Atlasses/";
        public const string EXPORT_ASSET_PATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Exports/";
        public const string TEMP_ASSET_PATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Temp/";

        public const string DEFAULT_TEXTURE_GROUP_NAME = "Albedo";
        public const float VIEW_OFFSET = 20;

        public static MA_TextureAtlasserProSettings CreateSettings()
        {
            MA_TextureAtlasserProSettings _settings = ScriptableObject.CreateInstance<MA_TextureAtlasserProSettings>();

            if (_settings != null)
            {
                CreateFolder(SETTINGS_ASSET_PATH);
                AssetDatabase.CreateAsset(_settings, SETTINGS_ASSET_PATH + "MA_TextureAtlasserProSettings.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return _settings;
            }
            else
            {
                return null;
            }
        }

        public static MA_TextureAtlasserProSettings LoadSettings()
        {
            MA_TextureAtlasserProSettings _settings = AssetDatabase.LoadAssetAtPath(SETTINGS_ASSET_PATH + "MA_TextureAtlasserProSettings.asset", typeof(MA_TextureAtlasserProSettings)) as MA_TextureAtlasserProSettings;

            if (_settings == null)
            {
                _settings = CreateSettings();
            }

            return _settings;
        }

        public static MA_TextureAtlasserProExportSettings CreateExportSettings(string name, bool canModify = true)
        {
            MA_TextureAtlasserProExportSettings _settings = ScriptableObject.CreateInstance<MA_TextureAtlasserProExportSettings>();
            _settings.canModify = canModify;

            if (_settings != null)
            {
                _settings.materialExportSettings.shader = Shader.Find("Standard");

                CreateFolder(EXPORT_ASSET_PATH);
                AssetDatabase.CreateAsset(_settings, SETTINGS_ASSET_PATH + name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return _settings;
            }
            else
            {
                return null;
            }
        }

        public static MA_TextureAtlasserProExportSettings LoadExportSettings()
        {
            string name = "MA_DefaultExportSettings";
            MA_TextureAtlasserProExportSettings _settings = AssetDatabase.LoadAssetAtPath(SETTINGS_ASSET_PATH + name + ".asset", typeof(MA_TextureAtlasserProExportSettings)) as MA_TextureAtlasserProExportSettings;

            if (_settings == null)
            {
                _settings = CreateExportSettings(name, false);
            }

            return _settings;
        }

        public static MA_TextureAtlasserProAtlas CreateTextureAtlas(string name, Vector2 size)
        {
            MA_TextureAtlasserProAtlas _atlas = ScriptableObject.CreateInstance<MA_TextureAtlasserProAtlas>();

            if (_atlas != null)
            {
                _atlas.CreateAtlas(name, size);
                MA_CheckTextureAtlas(_atlas);

                CreateFolder(SAVE_ASSET_PATH);
                AssetDatabase.CreateAsset(_atlas, SAVE_ASSET_PATH + name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return _atlas;
            }
            else
            {
                return null;
            }
        }

        public static MA_TextureAtlasserProAtlas LoadTextureAtlas()
        {
            MA_TextureAtlasserProAtlas _atlas = null;
            string absPath = EditorUtility.OpenFilePanel("Select Texture Atlas", LOAD_ASSET_PATH, "");

            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                _atlas = AssetDatabase.LoadAssetAtPath(relPath, typeof(MA_TextureAtlasserProAtlas)) as MA_TextureAtlasserProAtlas;

                MA_CheckTextureAtlas(_atlas);

                if (_atlas)
                {
                    EditorPrefs.SetString("AtlasPath", null);
                }
            }

            if (_atlas != null)
            {
                if (_atlas.selectedTextureQuad != null)
                {
                    _atlas.selectedTextureQuad.isSelected = false;
                }
                _atlas.selectedTextureQuad = null;
            }

            return _atlas;
        }

        public static void MA_CheckTextureAtlas(MA_TextureAtlasserProAtlas atlas)
        {
            if (atlas.textureGroupRegistration == null)
            {
                atlas.textureGroupRegistration = new List<MA_TextureGroupRegistration>();

                MA_TextureGroupRegistration groupRegistration = new MA_TextureGroupRegistration
                {
                    name = DEFAULT_TEXTURE_GROUP_NAME
                };

                atlas.textureGroupRegistration.Add(groupRegistration);
            }

            if (atlas.textureQuads == null)
            {
                atlas.textureQuads = new List<MA_TextureAtlasserProQuad>();
            }
            else
            {
                bool _sameCount = true;
                foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
                {
                    if (q.textureGroups.Count != atlas.textureGroupRegistration.Count)
                    {
                        _sameCount = false;
                        Debug.LogWarning("TextureAtlasser: " + q.name + " doesn't have the right amount of texture groups!");
                    }
                }

                if (_sameCount)
                {
                    foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
                    {
                        for (int i = 0; i < atlas.textureQuads.Count; i++)
                        {
                            for (int j = 0; j < atlas.textureGroupRegistration.Count; j++)
                            {
                                if (atlas.textureQuads[i].textureGroups[j].name != atlas.textureGroupRegistration[j].name)
                                {
                                    Debug.LogWarning("TextureAtlasser: " + q.name + " doesn't have the right texture group name!");
                                }
                            }
                        }
                    }
                }
            }

            if (atlas.exportSettings == null)
            {
                atlas.exportSettings = LoadExportSettings();
            }
        }

        public static MA_TextureAtlasserProQuad CreateTextureQuad(MA_TextureAtlasserProAtlas atlas, string name, Rect rect, bool focus = true)
        {
            if (atlas != null)
            {
                //Create new list if we haven't one already
                if (atlas.textureQuads == null)
                {
                    atlas.textureQuads = new List<MA_TextureAtlasserProQuad>();
                }

                //Create new quad
                MA_TextureAtlasserProQuad _quad = ScriptableObject.CreateInstance<MA_TextureAtlasserProQuad>();

                //Add quad to asset
                if (_quad != null)
                {
                    //Set quad settings
                    _quad.name = name;
                    _quad.rect = rect;

                    SetTextureGroups(atlas, _quad);

                    atlas.textureQuads.Add(_quad);

                    AssetDatabase.AddObjectToAsset(_quad, atlas);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    if (focus)
                    {
                        atlas.selectedTextureQuad = atlas.textureQuads[atlas.textureQuads.Count - 1];
                    }

                    return _quad;
                }
                else
                {
                    Debug.LogError("CreateTextureQuad Failed: _TextureQuad");
                }
            }
            else
            {
                Debug.LogError("CreateTextureQuad Failed: textureAtlas");
            }

            return null;
        }

        public static void RemoveTextureQuad(MA_TextureAtlasserProAtlas atlas, bool focus = true)
        {
            if (atlas != null && atlas.selectedTextureQuad != null)
            {
                int _index = atlas.textureQuads.IndexOf(atlas.selectedTextureQuad);

                atlas.textureQuads.RemoveAt(_index);
                Object.DestroyImmediate(atlas.selectedTextureQuad, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (focus && atlas.textureQuads.Count > 0)
                {
                    _index = Mathf.Clamp(_index, 0, atlas.textureQuads.Count - 1);
                    atlas.selectedTextureQuad = atlas.textureQuads[_index];
                }
            }
        }

        public static void DuplicateTextureQuad(MA_TextureAtlasserProAtlas atlas, bool focus = true, string namePrefix = "new ")
        {
            if (atlas != null && atlas.selectedTextureQuad != null)
            {
                //MA_TextureAtlasserProQuad q = CreateTextureQuad(atlas, namePrefix + atlas.selectedTextureQuad.name, atlas.selectedTextureQuad.rect, false);
                MA_TextureAtlasserProQuad q = Object.Instantiate(atlas.selectedTextureQuad);
                q.name = string.Format("{0}{1}", namePrefix, atlas.selectedTextureQuad.name);
                atlas.textureQuads.Add(q);

                AssetDatabase.AddObjectToAsset(q, atlas);

                if (focus)
                {
                    atlas.selectedTextureQuad = q;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static void SetTextureGroups(MA_TextureAtlasserProAtlas atlas, MA_TextureAtlasserProQuad quad)
        {
            if (quad.textureGroups == null)
            {
                quad.textureGroups = new List<MA_TextureGroup>();
            }

            //Add texture groups
            foreach (MA_TextureGroupRegistration tgr in atlas.textureGroupRegistration)
            {
                MA_TextureGroup textureGroup = new MA_TextureGroup
                {
                    name = tgr.name
                };
                quad.textureGroups.Add(textureGroup);
            }
        }

        public static void CreateTextureGroup(MA_TextureAtlasserProAtlas atlas, string name)
        {
            MA_TextureGroupRegistration _textureGroupRegistration = new MA_TextureGroupRegistration
            {
                name = name
            };
            atlas.textureGroupRegistration.Add(_textureGroupRegistration);

            foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
            {
                MA_TextureGroup _textureGroup = new MA_TextureGroup
                {
                    name = name
                };
                q.textureGroups.Add(_textureGroup);
            }
        }

        public static void RemoveTextureGroup(MA_TextureAtlasserProAtlas atlas, int index)
        {
            atlas.textureGroupRegistration.RemoveAt(index);

            foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
            {
                q.textureGroups.RemoveAt(index);
            }
        }

        public static void CloseWindow(MA_TextureAtlasserProWindow curWindow)
        {
            if (curWindow == null)
            {
                Debug.LogError("Closing window Failed: curWindow == null");
            }
            curWindow.Close();
        }

        public static bool IsPowerOfTwo(int value)
        {
            //While x is even and > 1
            while (((value % 2) == 0) && value > 1)
            {
                value /= 2;
            }

            return (value == 1);
        }

        public static void CreateFolder(string folderPath)
        {
            if (folderPath.LastIndexOf('/') == folderPath.Length - 1)
            {
                folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            }

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string parentPath = folderPath.Substring(0, folderPath.LastIndexOf('/'));

                if(!AssetDatabase.IsValidFolder(parentPath))
                {
                    CreateFolder(parentPath);
                }

                string folderName = folderPath.Substring(folderPath.LastIndexOf('/') + 1);

                AssetDatabase.CreateFolder(parentPath, folderName);
                AssetDatabase.Refresh();
            }
        }

        public static void DeleteFolder(string folderPath)
        {
            if (folderPath.LastIndexOf('/') == folderPath.Length - 1)
            {
                folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            }

            if (AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.DeleteAsset(folderPath);
                AssetDatabase.Refresh();
            }
        }

        #region Export
        public static string[] ExportAtlasModels(MA_TextureAtlasserProAtlas atlas, ModelExportSettings modelExportSettings, string materialPath = null, string savePath = EXPORT_ASSET_PATH)
        {
            switch (modelExportSettings.modelFormat)
            {
                case ModelFormat.None:
                    break;
                case ModelFormat.UnityMeshPrefab:
                    return ExportAtlasUnityMeshPrefab(atlas, modelExportSettings, materialPath: materialPath, savePath: savePath);
                //case ModelFormat.Obj:
                //    return ExportAtlasObj(atlas, modelExportSettings, savePath: savePath);
                default:
                    break;
            }

            return null;
        }

        private static string[] ExportAtlasUnityMeshPrefab(MA_TextureAtlasserProAtlas atlas, ModelExportSettings modelExportSettings, string materialPath = null, string savePath = EXPORT_ASSET_PATH)
        {
            if (atlas == null || atlas.textureQuads == null)
            {
                return null;
            }

            List<string> assetPaths = new List<string>();

            //Directories.
            string savePathPrefab = savePath + atlas.name + "/";
            string savePathMeshes = savePathPrefab + "Meshes/";
            CreateFolder(savePathPrefab);
            CreateFolder(savePathMeshes);

            foreach (MA_TextureAtlasserProQuad quad in atlas.textureQuads)
            {
                foreach (MA_ModelGroup mg in quad.modelGroups)
                {
                    //Validate name.
                    if (string.IsNullOrEmpty(mg.name) || string.IsNullOrWhiteSpace(mg.name))
                    {
                        mg.name = MA_StringUtils.RandomAlphabetString(6);
                        Debug.LogWarning("No valid model name assigned!");
                    }

                    //Create new prefab asset.
                    string newPrefabPath = MA_PrefabUtils.CreatePrefab(mg.name, savePathPrefab);
                    GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);

                    foreach (Mesh m in mg.meshes)
                    {
                        if(m != null)
                        {
                            //Validate name.
                            if (string.IsNullOrEmpty(m.name) || string.IsNullOrWhiteSpace(m.name))
                            {
                                m.name = MA_StringUtils.RandomAlphabetString(6);
                                Debug.LogWarning("No valid mesh name assigned!");
                            }

                            //Create new mesh.
                            //Duplicate it from the current one.
                            Mesh newMesh = MA_MeshUtils.MA_DuplicateMesh(m);
                            //Remap UV's.
                            newMesh = MA_MeshUtils.MA_UVReMap(newMesh, atlas.textureAtlasSize, quad.guiRect, modelExportSettings.uvChannel, modelExportSettings.uvFlipY, modelExportSettings.uvWrap);
                            //Set name.
                            newMesh.name = string.Format("{0}_{1}", mg.name, m.name);
                            //Save mesh.
                            string savedMeshPath = MA_MeshUtils.MA_SaveMeshAsset(newMesh, savePathMeshes);

                            //Load mesh.
                            Mesh savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(savedMeshPath);
                            //Load material.
                            Material savedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                            //Create gameObject.
                            GameObject newGameObject = new GameObject(m.name);
                            //Add mesh filter.
                            MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
                            mf.mesh = savedMesh;
                            //Add mesh renderer.
                            MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();
                            mr.material = savedMaterial;

                            //Add to parent gameObject (prefab).
                            MA_PrefabUtils.AddChild(newPrefab, newGameObject);
                            Object.DestroyImmediate(newGameObject);
                        }
                    }

                    assetPaths.Add(newPrefabPath);
                }
            }

            return assetPaths.ToArray();
        }

        public static string[] ExportAtlasTextures(MA_TextureAtlasserProAtlas atlas, TextureExportSettings textureExportSettings, string savePath = EXPORT_ASSET_PATH, string tempPath = TEXTURE_ATLASSER_PATH)
        {
            switch (textureExportSettings.textureFormat)
            {
                case TextureFormat.None:
                    break;
                case TextureFormat.Png:
                    return ExportAtlasPNG(atlas, textureExportSettings, savePath);
                default:
                    break;
            }

            return null;
        }

        private static string[] ExportAtlasPNG(MA_TextureAtlasserProAtlas atlas, TextureExportSettings textureExportSettings, string savePath = EXPORT_ASSET_PATH, string tempPath = TEMP_ASSET_PATH)
        {
            if (atlas == null || atlas.textureQuads == null || atlas.textureGroupRegistration == null)
            {
                return null;
            }

            string[] assetPaths = new string[atlas.textureGroupRegistration.Count];

            //Directories.
            string savePathTextures = savePath + atlas.name + "/Textures/";
            CreateFolder(savePathTextures);

            //Create temp folder
            CreateFolder(tempPath);

            //Foreach texture group
            for (int i = 0; i < atlas.textureGroupRegistration.Count; i++)
            {
                //Create new Texture Atlas
                Texture2D newTexture = new Texture2D((int)atlas.textureAtlasSize.x, (int)atlas.textureAtlasSize.y)
                {
                    name = atlas.name + "_" + atlas.textureGroupRegistration[i].name
                };

                foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
                {
                    if (q.textureGroups != null && q.textureGroups[i].texture != null)
                    {
                        //Make temp copy
                        string orginalTexturePath = AssetDatabase.GetAssetPath(q.textureGroups[i].texture);
                        string orginalTextureExtension = System.IO.Path.GetExtension(orginalTexturePath);

                        string tempTexturePath = tempPath + q.textureGroups[i].texture.name + orginalTextureExtension;
                        AssetDatabase.CopyAsset(orginalTexturePath, tempTexturePath);

                        //Set temp copy to default settings
                        TextureImporter tempTextureImporter = (TextureImporter)AssetImporter.GetAtPath(tempTexturePath);
                        tempTextureImporter.textureType = TextureImporterType.Default;
                        tempTextureImporter.sRGBTexture = false;
                        tempTextureImporter.alphaIsTransparency = false;
                        tempTextureImporter.maxTextureSize = (int)Mathf.Max(atlas.textureAtlasSize.x, atlas.textureAtlasSize.y);
                        tempTextureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                        tempTextureImporter.SaveAndReimport();

                        //Load temp copy
                        Texture tempCopy = AssetDatabase.LoadAssetAtPath<Texture>(tempTextureImporter.assetPath);

                        //Create new texture part
                        Texture2D newTexturePart = (Texture2D)MA_TextureUtils.ConvertToReadableTexture(tempCopy);

                        //Scale it
                        newTexturePart = newTexturePart.MA_Scale2D((int)q.guiRect.width, (int)q.guiRect.height, textureExportSettings.textureScaleMode);

                        //Add it
                        newTexture = newTexture.MA_Combine2D(newTexturePart, (int)q.guiRect.x, (int)q.guiRect.y);

                        //Delete temp copy
                        AssetDatabase.DeleteAsset(tempTextureImporter.assetPath);
                    }
                }

                //Save it
                newTexture.MA_Save2D(newTexture.name, savePathTextures);
                assetPaths[i] = (savePathTextures + newTexture.name + '.' + textureExportSettings.textureFormat.ToString());

                //Set settings.
                switch (textureExportSettings.textureType)
                {
                    case TextureType.Default:
                        {
                            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePathTextures + newTexture.name + '.' + textureExportSettings.textureFormat.ToString());
                            textureImporter.textureType = TextureImporterType.Default;
                            textureImporter.SaveAndReimport();
                        }
                        break;
                    case TextureType.Sprite:
                        SetAtlasSpriteSettings(atlas, textureExportSettings, savePathTextures);
                        break;
                    case TextureType.SpriteSliced:
                        SetAtlasSpriteSettings(atlas, textureExportSettings, savePathTextures);
                        break;
                    default:
                        break;
                }
            }

            //Delete temp folder
            DeleteFolder(tempPath);

            //Refresh
            AssetDatabase.Refresh();

            return assetPaths;
        }

        private static void SetAtlasSpriteSettings(MA_TextureAtlasserProAtlas atlas, TextureExportSettings textureExportSettings, string savePath)
        {
            //Foreach texture group
            for (int i = 0; i < atlas.textureGroupRegistration.Count; i++)
            {
                //Convert
                string textureName = atlas.name + "_" + atlas.textureGroupRegistration[i].name + '.' + textureExportSettings.textureFormat.ToString();
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePath + textureName);
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.alphaIsTransparency = true;

                //Slice sprites.
                if (textureExportSettings.textureType == TextureType.SpriteSliced)
                {
                    textureImporter.spriteImportMode = SpriteImportMode.None; //Reset it to update?
                    textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                    List<SpriteMetaData> spriteMetaData = new List<SpriteMetaData>();

                    foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
                    {
                        if (q.textureGroups != null && q.textureGroups[i].texture != null)
                        {
                            //Create new SpriteMetaData.
                            SpriteMetaData smd = new SpriteMetaData
                            {
                                name = q.name,
                                rect = new Rect(q.guiRect.x, atlas.textureAtlasSize.y - q.guiRect.y - q.guiRect.height, q.guiRect.width, q.guiRect.height)
                            };

                            spriteMetaData.Add(smd);
                        }
                    }

                    textureImporter.spritesheet = spriteMetaData.ToArray();
                }
                else
                {
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                }

                textureImporter.SaveAndReimport();
            }
        }

        public static string ExportAtlasMaterial(MA_TextureAtlasserProAtlas atlas, MaterialExportSettings materialExportSettings, string[] textures = null, string savePath = EXPORT_ASSET_PATH)
        {
            if (atlas == null || atlas.textureQuads == null || atlas.textureGroupRegistration == null)
            {
                return null;
            }

            //Directories.
            string savePathMaterial = savePath + atlas.name + "/Materials/";
            CreateFolder(savePathMaterial);

            Shader shader = materialExportSettings.shader;
            if (shader)
            {
                Material material = new Material(shader)
                {
                    name = atlas.name
                };

                if (textures != null)
                {
                    for (int i = 0; i < (int)Mathf.Min(materialExportSettings.shaderPropertyNames.Count, textures.Length); i++)
                    {
                        Texture t = AssetDatabase.LoadAssetAtPath<Texture>(textures[i]);
                        if (t != null)
                        {
                            material.SetTexture(materialExportSettings.shaderPropertyNames[i], t);
                        }
                    }
                }

                string assetPath = savePathMaterial + material.name + ".mat";

                //Save material
                AssetDatabase.CreateAsset(material, assetPath);
                AssetDatabase.Refresh();

                return assetPath;
            }

            return null;
        }
        #endregion
    }
}
#endif