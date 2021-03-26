//https://github.com/maxartz15/MA_MeshUtils

//References:
//http://wiki.unity3d.com/index.php?title=ObjExporter

#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace MA_Mesh
{
	public static class MA_MeshUtils
	{
		public static string MA_SaveMeshAsset(Mesh mesh, string savePath)
		{
			if (string.IsNullOrEmpty(mesh.name))
			{
				mesh.name = UnityEngine.Random.Range(11111, 99999).ToString();
			}

			string assetPath = savePath + mesh.name + ".asset";

			AssetDatabase.CreateAsset(mesh, assetPath);
			AssetDatabase.SaveAssets();

			return assetPath;
		}

		public static string MA_SaveMeshPrefab(Mesh mesh, string prefabName, string savePath, string materialPath)
		{
			string assetPath = null;

			string meshAssetPath = MA_SaveMeshAsset(mesh, savePath);
			Mesh meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(meshAssetPath);

			if (meshAsset != null)
			{
				GameObject gameObject = new GameObject
				{
					name = prefabName
				};

				gameObject.AddComponent<MeshFilter>().mesh = meshAsset;
				gameObject.AddComponent<MeshRenderer>();

				Material curMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
				if (curMaterial != null)
				{
					gameObject.GetComponent<MeshRenderer>().material = curMaterial;
				}

				if (string.IsNullOrEmpty(prefabName))
				{
					prefabName = UnityEngine.Random.Range(11111, 99999).ToString();
				}

				assetPath = savePath + prefabName + ".prefab";

				PrefabUtility.SaveAsPrefabAsset(gameObject, assetPath);
				UnityEngine.Object.DestroyImmediate(gameObject);
			}

			return assetPath;
		}

		public static Mesh MA_DuplicateMesh(Mesh mesh)
		{
			Mesh newMesh = new Mesh
			{
				name = mesh.name,
				bounds = mesh.bounds,
				subMeshCount = mesh.subMeshCount
			};

			newMesh.SetVertices(new List<Vector3>(mesh.vertices));
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				newMesh.SetTriangles(mesh.GetTriangles(i), i);
			}
			newMesh.SetNormals(new List<Vector3>(mesh.normals));
			for (int i = 0; i < 8; i++)
			{
				List<Vector2> uvs = new List<Vector2>();
				mesh.GetUVs(i, uvs);
				newMesh.SetUVs(i, uvs);
			}
			newMesh.SetTangents(new List<Vector4>(mesh.tangents));
			newMesh.SetColors(new List<Color>(mesh.colors));

			return newMesh;
		}

		public static Mesh MA_ReMapUV(this Mesh mesh, Vector2 atlasSize, Vector2 textureSize, Vector2 texturePosition, int uvChannel = 0)
		{
			/*
			 0     1
			512 x 512

			 0    .5 	= 1 / 512 * 256
			256 x 256

			+ pos
			*/

			List<Vector2> uvs = new List<Vector2>();

			//Get UV's
			mesh.GetUVs(uvChannel, uvs);

			foreach (Vector2 uvCordinate in uvs)
			{
				float x = (uvCordinate.x / atlasSize.x * textureSize.x) + texturePosition.x;
				float y = (uvCordinate.y / atlasSize.y * textureSize.y) + texturePosition.y;
				uvCordinate.Set(x, y);
			}

			mesh.SetUVs(uvChannel, uvs);

			return mesh;
		}

		public static Mesh MA_UVReMap(this Mesh mesh, Vector2 atlasSize, Rect textureRect, int uvChannel = 0, bool flipY = true, bool wrap = true)
		{
			//Get UV's
			List<Vector2> uvs = new List<Vector2>();
			mesh.GetUVs(uvChannel, uvs);

			//Min and max bounds in 0-1 space.
			float xMin, xMax, yMin, yMax;
			xMin = (1f / atlasSize.x * textureRect.width);
			xMax = (1f / atlasSize.x * textureRect.x);
			yMin = (1f / atlasSize.y * textureRect.height);

			//Flip uv's if needed.
			if (flipY)
			{
				yMax = (1f / atlasSize.y * (atlasSize.y - textureRect.height - textureRect.y));
			}
			else
			{
				yMax = (1f / atlasSize.y * textureRect.y);
			}

			for (int i = 0; i < uvs.Count; i++)
			{
				float newX = uvs[i].x * xMin + xMax;
				float newY = uvs[i].y * yMin + yMax;

				//Wrap the verts outside of the uv space around back into the uv space.
				if (wrap)
				{
					newX = Wrap(newX, xMax, xMin + xMax);
					newY = Wrap(newY, yMax, yMin + yMax);
				}

				uvs[i] = new Vector2(newX, newY);
			}

			mesh.SetUVs(uvChannel, uvs);

			return mesh;
		}

		public static float Wrap(float val, float min, float max)
		{
			val -= (float)Math.Round((val - min) / (max - min)) * (max - min);
			if (val < min)
				val = val + max - min;
			return val;
		}
	}
}
#endif