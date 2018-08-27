using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	public class MA_TextureAtlasserProInspectorView : MA_TextureAtlasserProViewBase 
	{
		private MA_TextureAtlasserProQuad lastSelectedQuad;

		private bool isEditing = false;

		public MA_TextureAtlasserProInspectorView(MA_TextureAtlasserProWindow currentEditorWindow, string title) : base(currentEditorWindow, title)
		{
			
		}

		public override void UpdateView(Event e, Rect editorViewRect)
		{
			//Update base derived class
			base.UpdateView(e, editorViewRect);

			if(isLoaded)
			{
				//Draw inspector
				if(curWindow.textureAtlas != null && curWindow.textureAtlas.selectedTextureQuad != null)
				{
					//Deselect GUI elements when we are focusing on a new quad
					if(lastSelectedQuad != curWindow.textureAtlas.selectedTextureQuad)
					{
						lastSelectedQuad = curWindow.textureAtlas.selectedTextureQuad;
						GUI.FocusControl(null);
					}

					GUILayout.BeginArea(editorViewRect, EditorStyles.helpBox);		
					GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

					GUILayout.Label("Quad Name");
					curWindow.textureAtlas.selectedTextureQuad.name = EditorGUILayout.TextField(curWindow.textureAtlas.selectedTextureQuad.name);

					GUILayout.Space(MA_TextureAtlasserProUtils.VIEWOFFSET / 2);

					//Textures
					GUILayout.BeginHorizontal();
					GUILayout.Label("Textures", GUILayout.ExpandWidth(true));
					if(GUILayout.Button(MA_TextureAtlasserProIcons.editIcon, EditorStyles.miniButton, GUILayout.Width(36), GUILayout.Height(15)))
					{
						isEditing = !isEditing;
					}
					GUILayout.EndHorizontal();
					if(curWindow.textureAtlas.textureGroupRegistration == null || curWindow.textureAtlas.textureGroupRegistration.Count == 0)
					{
						if(GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
						{
							MA_TextureAtlasserProUtils.CreateTextureGroup(curWindow.textureAtlas, "New TextureGroup");
						}
					}
					for (int i = 0; i < curWindow.textureAtlas.textureGroupRegistration.Count; i++)
					{
						if(isEditing)
						{
							curWindow.textureAtlas.textureGroupRegistration[i].name = curWindow.textureAtlas.selectedTextureQuad.textureGroups[i].name = EditorGUILayout.TextField(curWindow.textureAtlas.textureGroupRegistration[i].name);
						}
						else
						{
							GUILayout.Label(curWindow.textureAtlas.textureGroupRegistration[i].name);
						}
						GUILayout.BeginHorizontal();
						curWindow.textureAtlas.selectedTextureQuad.textureGroups[i].texture = (Texture)EditorGUILayout.ObjectField(curWindow.textureAtlas.selectedTextureQuad.textureGroups[i].texture, typeof(Texture), false);
						if(isEditing && GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false)))
						{
							MA_TextureAtlasserProUtils.RemoveTextureGroup(curWindow.textureAtlas, i);
						}
						if(isEditing && GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
						{
							MA_TextureAtlasserProUtils.CreateTextureGroup(curWindow.textureAtlas, "New TextureGroup");
						}
						GUILayout.EndHorizontal();
					}

					GUILayout.Space(MA_TextureAtlasserProUtils.VIEWOFFSET / 2);

					//Meshes	
					GUILayout.Label("Meshes");
					if(curWindow.textureAtlas.selectedTextureQuad.meshes != null)
					{
						if(curWindow.textureAtlas.selectedTextureQuad.meshes.Count == 0)
						{
							if(GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
							{
								curWindow.textureAtlas.selectedTextureQuad.meshes.Add(null);
							}
						}
						for (int i = 0; i < curWindow.textureAtlas.selectedTextureQuad.meshes.Count; i++)
						{
							GUILayout.BeginHorizontal();
							curWindow.textureAtlas.selectedTextureQuad.meshes[i] = (Mesh)EditorGUILayout.ObjectField(curWindow.textureAtlas.selectedTextureQuad.meshes[i], typeof(Mesh), false);
							if(GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false)))
							{
								curWindow.textureAtlas.selectedTextureQuad.meshes.RemoveAt(i);
							}
							if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
							{
								curWindow.textureAtlas.selectedTextureQuad.meshes.Insert(i, null);
							}
							GUILayout.EndHorizontal();
						}		
					}
					else
					{
						curWindow.textureAtlas.selectedTextureQuad.meshes = new List<Mesh>();
					}

					GUILayout.FlexibleSpace();
					GUILayout.Label("x " + curWindow.textureAtlas.selectedTextureQuad.guiRect.x.ToString() + ", y " + curWindow.textureAtlas.selectedTextureQuad.guiRect.y.ToString());
					GUILayout.Label("w " + curWindow.textureAtlas.selectedTextureQuad.guiRect.width.ToString() + ", h " + curWindow.textureAtlas.selectedTextureQuad.guiRect.height.ToString());

					GUILayout.EndVertical();
					GUILayout.EndArea();           
				}
			}

			if(curWindow.textureAtlas != null && curWindow.textureAtlas.selectedTextureQuad != null)				
				ProcessEvents(e, editorViewRect);
		}

		protected override void ProcessEvents(Event e, Rect editorViewRect)
		{
			base.ProcessEvents(e, editorViewRect);
		}
	}
}