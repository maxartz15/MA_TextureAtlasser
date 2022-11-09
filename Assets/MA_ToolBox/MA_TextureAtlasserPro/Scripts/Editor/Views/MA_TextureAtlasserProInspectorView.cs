#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;
using MA_Toolbox.Utils;

namespace MA_TextureAtlasserPro
{
    public class MA_TextureAtlasserProInspectorView : MA_TextureAtlasserProViewBase
    {
        private MA_TextureAtlasserProQuad lastSelectedQuad;
        private bool isEditing = false;
        private GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        private Vector2 scrollPos = Vector2.zero;
        
        public MA_TextureAtlasserProInspectorView(MA_TextureAtlasserProWindow currentEditorWindow, string title) : base(currentEditorWindow, title)
        {

        }

        public override void UpdateView(Event e, Rect editorViewRect)
        {
            //Update base derived class
            base.UpdateView(e, editorViewRect);

            if (isLoaded)
            {
                //Draw inspector
                if (curWindow.textureAtlas != null && curWindow.textureAtlas.selectedTextureQuad != null)
                {
                    //Deselect GUI elements when we are focusing on a new quad
                    if (lastSelectedQuad != curWindow.textureAtlas.selectedTextureQuad)
                    {
                        lastSelectedQuad = curWindow.textureAtlas.selectedTextureQuad;
                        GUI.FocusControl(null);
                    }

                    GUILayout.BeginArea(editorViewRect, EditorStyles.helpBox);
                    using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPos, false, false))
                    {
                        scrollPos = scrollViewScope.scrollPosition;

                        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                        GUILayout.Label("Quad Name");
                        curWindow.textureAtlas.selectedTextureQuad.name = EditorGUILayout.TextField(curWindow.textureAtlas.selectedTextureQuad.name);

                        GUILayout.Space(MA_TextureAtlasserProUtils.VIEW_OFFSET / 2);

                        //Textures
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Textures", GUILayout.ExpandWidth(true));
                        if (GUILayout.Button(MA_TextureAtlasserProGuiLoader.editGC, EditorStyles.miniButton, GUILayout.Width(36), GUILayout.Height(15)))
                        {
                            isEditing = !isEditing;
                        }
                        GUILayout.EndHorizontal();
                        if (curWindow.textureAtlas.textureGroupRegistration == null || curWindow.textureAtlas.textureGroupRegistration.Count == 0)
                        {
                            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
                            {
                                MA_TextureAtlasserProUtils.CreateTextureGroup(curWindow.textureAtlas, "New TextureGroup");
                            }
                        }
                        for (int i = 0; i < curWindow.textureAtlas.textureGroupRegistration.Count; i++)
                        {
                            if (isEditing)
                            {
                                curWindow.textureAtlas.textureGroupRegistration[i].name = curWindow.textureAtlas.selectedTextureQuad.textureGroups[i].name = EditorGUILayout.TextField(curWindow.textureAtlas.textureGroupRegistration[i].name);
                            }
                            else
                            {
                                GUILayout.Label(curWindow.textureAtlas.textureGroupRegistration[i].name);
                            }
                            GUILayout.BeginHorizontal();
                            curWindow.textureAtlas.selectedTextureQuad.textureGroups[i].texture = (Texture)EditorGUILayout.ObjectField(curWindow.textureAtlas.selectedTextureQuad.textureGroups[i].texture, typeof(Texture), false);
                            if (isEditing && GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false)))
                            {
                                MA_TextureAtlasserProUtils.RemoveTextureGroup(curWindow.textureAtlas, i);
                            }
                            if (isEditing && GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
                            {
                                MA_TextureAtlasserProUtils.CreateTextureGroup(curWindow.textureAtlas, "New TextureGroup");
                            }
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.Space(MA_TextureAtlasserProUtils.VIEW_OFFSET / 2);

                        //Models
                        GUILayout.Label("Models");

                        SerializedObject serializedObject = new SerializedObject(curWindow.textureAtlas.selectedTextureQuad);
                        serializedObject.Update();


                        if (curWindow.textureAtlas.selectedTextureQuad.modelGroups != null)
                        {
                            SerializedProperty modelGroupsSP = serializedObject.FindProperty("modelGroups");

                            for (int i = 0; i < curWindow.textureAtlas.selectedTextureQuad.modelGroups.Count; i++)
                            {
                                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                                {
                                    using (new GUILayout.HorizontalScope())
                                    {
                                        curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].name = EditorGUILayout.TextField(curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].name);
                                        if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
                                        {
                                            curWindow.textureAtlas.selectedTextureQuad.modelGroups.RemoveAt(i);
                                            break;
                                        }
                                    }

                                    SerializedProperty meshesSP = modelGroupsSP.GetArrayElementAtIndex(i).FindPropertyRelative("meshes");
#if UNITY_2020_2_OR_NEWER
                                    meshesSP.isExpanded = EditorGUILayout.Foldout(meshesSP.isExpanded, "Meshes", true);
#else
								EditorGUILayout.PropertyField(meshesSP, false, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(editorViewRect.width * 0.5f));
#endif
                                    if (meshesSP.isExpanded)
                                    {
                                        for (int j = 0; j < curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].meshes.Count; j++)
                                        {
                                            using (new GUILayout.HorizontalScope())
                                            {
                                                curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].meshes[j] = (Mesh)EditorGUILayout.ObjectField(curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].meshes[j], typeof(Mesh), false);
                                                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
                                                {
                                                    curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].meshes.RemoveAt(j);
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (GUILayout.Button("+", EditorStyles.miniButton))
                                    {
                                        curWindow.textureAtlas.selectedTextureQuad.modelGroups[i].meshes.Add(null);
                                    }
                                }
                            }

                            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
                            {
                                curWindow.textureAtlas.selectedTextureQuad.modelGroups.Add(new MA_ModelGroup() { name = MA_StringUtils.RandomAlphabetString(6) });
                            }
                        }
                        else
                        {
                            curWindow.textureAtlas.selectedTextureQuad.modelGroups = new List<MA_ModelGroup>();
                        }

                        serializedObject.ApplyModifiedProperties();

                        GUILayout.Space(MA_TextureAtlasserProUtils.VIEW_OFFSET / 2);

                        //x, y, w, h.
                        GUILayout.FlexibleSpace();
                        if (!MA_TextureAtlasserProUtils.IsPowerOfTwo((int)curWindow.textureAtlas.selectedTextureQuad.guiRect.width) || !MA_TextureAtlasserProUtils.IsPowerOfTwo((int)curWindow.textureAtlas.selectedTextureQuad.guiRect.height))
                        {
                            labelStyle.normal.textColor = Color.red;
                        }
                        else
                        {
                            labelStyle.normal.textColor = GUI.skin.label.normal.textColor;
                        }

                        GUILayout.Label("x " + curWindow.textureAtlas.selectedTextureQuad.guiRect.x.ToString() + ", y " + curWindow.textureAtlas.selectedTextureQuad.guiRect.y.ToString());
                        GUILayout.Label("w " + curWindow.textureAtlas.selectedTextureQuad.guiRect.width.ToString() + ", h " + curWindow.textureAtlas.selectedTextureQuad.guiRect.height.ToString(), labelStyle);
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
            }

            if (curWindow.textureAtlas != null && curWindow.textureAtlas.selectedTextureQuad != null)
                ProcessEvents(e, editorViewRect);
        }

        protected override void ProcessEvents(Event e, Rect editorViewRect)
        {
            base.ProcessEvents(e, editorViewRect);
        }
    }
}
#endif