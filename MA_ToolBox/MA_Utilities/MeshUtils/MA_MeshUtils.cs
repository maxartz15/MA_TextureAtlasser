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
		public static void MA_SaveMeshAsset(Mesh mesh, string savePath, string meshName = "")
		{
			Mesh newMesh = new Mesh();
			newMesh.SetVertices(new List<Vector3>(mesh.vertices));
			newMesh.SetTriangles(mesh.triangles, 0);
			newMesh.SetUVs(0, new List<Vector2>(mesh.uv));

			if(meshName == "")
			{
				newMesh.name = mesh.name;
			}
			else
			{
				newMesh.name = meshName;
			}

			AssetDatabase.CreateAsset(newMesh, savePath);
			AssetDatabase.SaveAssets();
		}

		public static Mesh MA_DuplicateMesh(Mesh mesh)
		{
			Mesh newMesh = new Mesh();
			newMesh.name = mesh.name;
			newMesh.SetVertices(new List<Vector3>(mesh.vertices));
			newMesh.bounds = mesh.bounds;
			newMesh.colors = mesh.colors.ToArray();
			newMesh.subMeshCount = mesh.subMeshCount;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				newMesh.SetTriangles(mesh.GetTriangles(i), i);
			}
			newMesh.subMeshCount = mesh.subMeshCount;
			newMesh.SetNormals(new List<Vector3>(mesh.normals));
			newMesh.SetUVs(0, new List<Vector2>(mesh.uv));
			newMesh.SetTangents(new List<Vector4>(mesh.tangents));

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

		public static Mesh MA_UVReMap(this Mesh mesh, Vector2 atlasSize, Rect textureRect, int uvChannel = 0, bool flipY = true)
		{
			List<Vector2> uvs = new List<Vector2>();
			
			//Get UV's
			mesh.GetUVs(uvChannel, uvs);

			for (int i = 0; i < uvs.Count; i++)
			{
				if(flipY)
				{
					//Debug.Log("01" + uvs[i].x);
					uvs[i] = new Vector2((uvs[i].x / atlasSize.x * textureRect.width) + (1 / atlasSize.x * textureRect.x), (uvs[i].y / atlasSize.y * textureRect.height) + (1 / atlasSize.y * (atlasSize.y - textureRect.height - textureRect.y)));
					//Debug.Log("02" + uvs[i].x);
				}
				else
				{			
					//Debug.Log("01" + uvs[i].x);
					uvs[i] = new Vector2((uvs[i].x / atlasSize.x * textureRect.width) + (1 / atlasSize.x * textureRect.x), (uvs[i].y / atlasSize.y * textureRect.height) + (1 / atlasSize.y * textureRect.y));
					//Debug.Log("02" + uvs[i].x);
				}
			}

			mesh.SetUVs(uvChannel, uvs);

			return mesh;
		}

		//Start http://wiki.unity3d.com/index.php?title=ObjExporter
		public static string MeshToString(Mesh mesh) 
		{
			int vertexOffset = 0;
			int normalOffset = 0;
			int uvOffset = 0;


			Material material = new Material(Shader.Find("Standard"));
	
			StringBuilder sb = new StringBuilder();
	
			sb.Append("g ").Append(mesh.name).Append("\n");

			foreach(Vector3 v in mesh.vertices) 
			{
				//This is sort of ugly - inverting x-component since we're in
				//a different coordinate system than "everyone" is "used to".
				sb.Append(string.Format("v {0} {1} {2}\n", -v.x, v.y, v.z));
			}

			sb.Append("\n");

			foreach(Vector3 v in mesh.normals) 
			{
				sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, v.y, v.z));
			}

			sb.Append("\n");

			foreach(Vector3 v in mesh.uv) 
			{
				sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
			}

			for (int m = 0 ; m < mesh.subMeshCount; m++) 
			{
				sb.Append("\n");
				sb.Append("usemtl ").Append(material.name + m).Append("\n");
				sb.Append("usemap ").Append(material.name + m).Append("\n");
	
				// int[] triangles = mesh.GetTriangles(m);
				// for (int i = 0; i < triangles.Length; i += 3)
				// {
				// 	sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i]+1, triangles[i+1]+1, triangles[i+2]+1));
				// }

				int[] triangles = mesh.GetTriangles(m);
				for (int i = 0; i < triangles.Length; i += 3) 
				{
					//Because we inverted the x-component, we also needed to alter the triangle winding.
					sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", triangles[i]+  1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i +2 ] + 1 + uvOffset));
				}
			}

			vertexOffset += mesh.vertices.Length;
			normalOffset += mesh.normals.Length;
			uvOffset += mesh.uv.Length;

			return sb.ToString();
		}
 
		public static void MeshToFile(Mesh mesh, string filename, string savePath) 
		{
			using (StreamWriter sw = new StreamWriter(savePath + filename + ".obj")) 
			{
				sw.Write(MeshToString(mesh));
				Debug.Log(savePath + filename);
			}			
		}
		//End
	}

	struct ObjMaterial
	{
		public string name;
		public string textureName;
	}
}
#endif