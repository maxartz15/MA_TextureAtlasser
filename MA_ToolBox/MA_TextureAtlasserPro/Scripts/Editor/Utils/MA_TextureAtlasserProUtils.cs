using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Mesh;
using MA_Texture;

namespace MA_TextureAtlasserPro
{
	public static class MA_TextureAtlasserProUtils
	{
		public const string SETTINGSASSETPATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Settings/";
		public const string SAVEASSETPATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Atlasses/"; 
		public const string LOADASSETPATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Atlasses/";
		public const string EXPORTASSETPATH = "Assets/MA_ToolBox/MA_TextureAtlasserPro/Exports/"; 
		public const float VIEWOFFSET = 20;
		public const string DEFAULTTEXTUREGROUPNAME = "Albedo";

		public static MA_TextureAtlasserProSettings CreateSettings()
		{
			MA_TextureAtlasserProSettings _settings = (MA_TextureAtlasserProSettings)ScriptableObject.CreateInstance<MA_TextureAtlasserProSettings>();

			if(_settings != null)
			{
				AssetDatabase.CreateAsset(_settings, SETTINGSASSETPATH + "MA_TextureAtlasserProSettings.asset");
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
			MA_TextureAtlasserProSettings _settings = AssetDatabase.LoadAssetAtPath(SETTINGSASSETPATH + "MA_TextureAtlasserProSettings.asset", typeof(MA_TextureAtlasserProSettings)) as MA_TextureAtlasserProSettings;

			if (_settings == null)
			{
				_settings = CreateSettings();
			}

			return _settings;
		}

		public static MA_TextureAtlasserProAtlas CreateTextureAtlas(string name, Vector2 size)
		{
			MA_TextureAtlasserProAtlas _atlas = (MA_TextureAtlasserProAtlas)ScriptableObject.CreateInstance<MA_TextureAtlasserProAtlas>();

			if(_atlas != null)
			{
				_atlas.CreateAtlas(name, size);
				MA_CheckTextureAtlas(_atlas);

				AssetDatabase.CreateAsset(_atlas, SAVEASSETPATH + name + ".asset");
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
			string absPath = EditorUtility.OpenFilePanel("Select Texture Atlas", LOADASSETPATH, "");

			if(absPath.StartsWith(Application.dataPath))
			{
				string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
				_atlas = AssetDatabase.LoadAssetAtPath(relPath, typeof(MA_TextureAtlasserProAtlas)) as MA_TextureAtlasserProAtlas;

				MA_CheckTextureAtlas(_atlas);

				if(_atlas)
				{
					EditorPrefs.SetString("AtlasPath", null);
				}
			}

			if(_atlas != null)
			{
				if(_atlas.selectedTextureQuad != null)
				{
					_atlas.selectedTextureQuad.isSelected = false;
				}
				_atlas.selectedTextureQuad = null;
			}
			
			return _atlas;
		}

		public static void MA_CheckTextureAtlas(MA_TextureAtlasserProAtlas atlas)
		{
			if(atlas.textureGroupRegistration == null)
			{
				atlas.textureGroupRegistration = new List<MA_TextureGroupRegistration>();

				MA_TextureGroupRegistration groupRegistration = new MA_TextureGroupRegistration();
				groupRegistration.name = DEFAULTTEXTUREGROUPNAME;

				atlas.textureGroupRegistration.Add(groupRegistration);
			}

			if(atlas.textureQuads == null)
			{
				atlas.textureQuads = new List<MA_TextureAtlasserProQuad>();
			}
			else
			{
				bool _sameCount = true;
				foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
				{
					if(q.textureGroups.Count != atlas.textureGroupRegistration.Count)
					{
						_sameCount = false;
						Debug.LogWarning("TextureAtlasser: " + q.name + " doesn't have the right amount of texture groups!");
					}
				}

				if(_sameCount)
				{
					foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
					{
						for (int i = 0; i < atlas.textureQuads.Count; i++)
						{
							for (int j = 0; j < atlas.textureGroupRegistration.Count; j++)
							{
								if(atlas.textureQuads[i].textureGroups[j].name != atlas.textureGroupRegistration[j].name)
								{
									Debug.LogWarning("TextureAtlasser: " + q.name + " doesn't have the right texture group name!");
								}
							}
						}
					}
				}
			}
		}

		public static void CreateTextureQuad(MA_TextureAtlasserProAtlas atlas, string name, Rect rect, bool focus = true)
		{
			if(atlas != null)
			{
				//Create new list if we haven't one already
				if(atlas.textureQuads == null)
				{
					atlas.textureQuads = new List<MA_TextureAtlasserProQuad>();
				}

				//Create new quad
				MA_TextureAtlasserProQuad _quad = (MA_TextureAtlasserProQuad)ScriptableObject.CreateInstance("MA_TextureAtlasserProQuad");

				//Add quad to asset
				if(_quad != null)
				{
					//Set quad settings
					_quad.name = name;
					_quad.rect = rect;

					SetTextureGroups(atlas, _quad);
					
					atlas.textureQuads.Add((MA_TextureAtlasserProQuad)_quad);

					AssetDatabase.AddObjectToAsset(_quad, atlas);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();

					if(focus)
					{
						atlas.selectedTextureQuad = atlas.textureQuads[atlas.textureQuads.Count - 1];
					}
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
		}

		public static void RemoveTextureQuad(MA_TextureAtlasserProAtlas atlas, bool focus = true)
		{
			if(atlas != null && atlas.selectedTextureQuad != null)
			{
				int _index = atlas.textureQuads.IndexOf(atlas.selectedTextureQuad);

				atlas.textureQuads.RemoveAt(_index);
				GameObject.DestroyImmediate(atlas.selectedTextureQuad, true);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				if (focus && atlas.textureQuads.Count > 0)
				{
					_index = Mathf.Clamp(_index, 0, atlas.textureQuads.Count - 1);
					atlas.selectedTextureQuad = atlas.textureQuads[_index];
				}
			}
		}

		public static void DuplicateTextureQuad(MA_TextureAtlasserProAtlas atlas, bool focus = true, bool copyData = false, string namePrefix = "new ")
		{
			if(atlas != null && atlas.selectedTextureQuad != null)
			{
				CreateTextureQuad(atlas, namePrefix + atlas.selectedTextureQuad.name, atlas.selectedTextureQuad.rect, false);

				if(copyData)
				{
					atlas.textureQuads[atlas.textureQuads.Count - 1].meshes = atlas.selectedTextureQuad.meshes;
					atlas.textureQuads[atlas.textureQuads.Count - 1].textureGroups = atlas.selectedTextureQuad.textureGroups;
				}

				if(focus)
				{
					atlas.selectedTextureQuad = atlas.textureQuads[atlas.textureQuads.Count - 1];
				}
			}
		}

		public static void SetTextureGroups(MA_TextureAtlasserProAtlas atlas, MA_TextureAtlasserProQuad quad)
		{
			if(quad.textureGroups == null)
			{
				quad.textureGroups = new List<MA_TextureGroup>();
			}

			//Add texture groups
			foreach (MA_TextureGroupRegistration tgr in atlas.textureGroupRegistration)
			{
				MA_TextureGroup textureGroup = new MA_TextureGroup();
				textureGroup.name = tgr.name;
				quad.textureGroups.Add(textureGroup);
			}
		}

		public static void CreateTextureGroup(MA_TextureAtlasserProAtlas atlas, string name)
		{
			MA_TextureGroupRegistration _textureGroupRegistration = new MA_TextureGroupRegistration();
			_textureGroupRegistration.name = name;
			atlas.textureGroupRegistration.Add(_textureGroupRegistration);

			foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
			{
				MA_TextureGroup _textureGroup = new MA_TextureGroup();
				_textureGroup.name = name;
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
			if(curWindow == null)
			{
				Debug.LogError("Closing window Failed: curWindow == null");
			}
			curWindow.Close();
		}

		public static void ExportAtlas(MA_TextureAtlasserProAtlas atlas, string savePath = EXPORTASSETPATH)
		{
			if(atlas != null && atlas.textureQuads != null)
			{
				ExportAtlasMeshesObj(atlas);
				ExportAtlasTexturesPNG(atlas);

				AssetDatabase.Refresh();
			}
		}

		public static void ExportAtlasMeshesObj(MA_TextureAtlasserProAtlas atlas, string savePath = EXPORTASSETPATH)
		{
			if(atlas != null && atlas.textureQuads != null)
			{
				foreach (MA_TextureAtlasserProQuad ta in atlas.textureQuads)
				{
					//Export Mesh
					if(ta.meshes != null)
					{
						for (int m = 0; m < ta.meshes.Count; m++)
						{
							if(ta.meshes[m] != null)
							{
								//Create new mesh
								Mesh newMesh = new Mesh();
								//Duplicate it from the current one
								newMesh = MA_MeshUtils.MA_DuplicateMesh(ta.meshes[m]);
								//Remap uvs
								newMesh = MA_MeshUtils.MA_UVReMap(newMesh, atlas.textureAtlasSize, ta.guiRect);
								//Save it
								MA_MeshUtils.MeshToFile(newMesh, "MA_" + ta.name, savePath);
							}
						}
					}
				}
			}
		}

		// public static void ExportAtlasTexturePNG(MA_TextureAtlasserProAtlas atlas, string savePath = EXPORTASSETPATH)
		// {
		// 	if(atlas != null && atlas.textureQuads != null)
		// 	{
		// 		//Create new Texture Atlas
		// 		Texture2D newTexture = new Texture2D((int)atlas.textureAtlasSize.x, (int)atlas.textureAtlasSize.y);
		// 		newTexture.name = atlas.name;

		// 		foreach (MA_TextureAtlasserProQuad ta in atlas.textureQuads)
		// 		{
		// 			//Export Texture Atlas
		// 			//TODO: Replace with texture groups (foreacht ...)
		// 			if(ta.texture != null)
		// 			{
		// 				//Create new texture part
		// 				Texture2D newTexturePart = (Texture2D)MA_Texture.MA_TextureUtils.ConvertToReadableTexture(ta.texture);
		// 				//Scale it
		// 				newTexturePart = newTexturePart.MA_Scale2D((int)ta.guiRect.width, (int)ta.guiRect.height);
		// 				//Add it
		// 				newTexture = newTexture.MA_Combine2D(newTexturePart, (int)ta.guiRect.x, (int)ta.guiRect.y);
		// 			}
		// 		}

		// 		//Save it
		// 		newTexture.MA_Save2D("MA_" + newTexture.name, savePath);
		// 		//Refresh
		// 		AssetDatabase.Refresh();
		// 	}
		// }

		public static void ExportAtlasTexturesPNG(MA_TextureAtlasserProAtlas atlas, string savePath = EXPORTASSETPATH)
		{
			if(atlas != null && atlas.textureQuads != null && atlas.textureGroupRegistration != null)
			{
				//Foreach texture group
				for (int i = 0; i < atlas.textureGroupRegistration.Count; i++)
				{
					//Create new Texture Atlas
					Texture2D newTexture = new Texture2D((int)atlas.textureAtlasSize.x, (int)atlas.textureAtlasSize.y);
					newTexture.name = atlas.name + "_" + atlas.textureGroupRegistration[i].name;

					foreach (MA_TextureAtlasserProQuad q in atlas.textureQuads)
					{
						if(q.textureGroups != null && q.textureGroups[i].texture != null)
						{
							//Create new texture part
							Texture2D newTexturePart = (Texture2D)MA_Texture.MA_TextureUtils.ConvertToReadableTexture(q.textureGroups[i].texture);
							//Scale it
							newTexturePart = newTexturePart.MA_Scale2D((int)q.guiRect.width, (int)q.guiRect.height);
							//Add it
							newTexture = newTexture.MA_Combine2D(newTexturePart, (int)q.guiRect.x, (int)q.guiRect.y);
						}
					}

					//Save it
					newTexture.MA_Save2D("MA_" + newTexture.name, savePath);
				}

				//Refresh
				AssetDatabase.Refresh();
			}
		}
	}
}