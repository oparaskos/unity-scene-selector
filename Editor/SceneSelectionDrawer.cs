using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using SceneSelector.Runtime;

namespace SceneSelector.Editor {
    [CustomPropertyDrawer(typeof(SceneSelection))]
    public class SceneSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            var lbl = EditorGUI.BeginProperty(rect, label, property);

            EditorGUILayout.BeginHorizontal();
            

            SceneSelection value = fieldInfo.GetValue(property.serializedObject.targetObject) as SceneSelection;
            HandleSceneDragAndDrop(rect, value);

            // Draw label
            EditorGUILayout.PrefixLabel(lbl);

            // Draw dropdown
            GenericMenu menu = CreateSceneSelectionMenu(value);
            string description = DescribeSelectedScenes(value);
            if (EditorGUILayout.DropdownButton(new GUIContent(description), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
            var style = new GUIStyle();
            style.padding = new RectOffset(0, 0, 0, 0);
            style.fixedWidth = 18;
            style.fixedHeight = 18;
            style.alignment = TextAnchor.LowerCenter;
            if (GUILayout.Button(
                EditorGUIUtility.IconContent("winbtn_win_close@2x"),
                style,
                
                GUILayout.Width(18),
                GUILayout.Height(18)
                ))
            {
                value.Clear();
            }

            EditorGUILayout.EndHorizontal();
            CheckScenesAreInBuild(value);

            value.RebuildSceneIndices();
            EditorGUI.EndProperty();
        }

        private void CheckScenesAreInBuild(SceneSelection value)
        {
            foreach (GUID guid in value.SceneGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Scene scene = SceneManager.GetSceneByPath(path);
                if (scene.buildIndex < 0)
                {
                    EditorGUILayout.HelpBox($"{path} is not in the build", MessageType.Warning);
                }
            }
        }

        private static string DescribeSelectedScenes(SceneSelection value)
        {
            var selected = value.SceneGUIDs?.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            string description = "Multiple";
            if (selected?.Count() == 1)
            {
                description = selected.First();
            }
            else if (selected == null || selected.Count() < 1)
            {
                description = "Add New...";
            }
            else if (selected?.Count() < 4)
            {
                description = string.Join(", ", selected.Select(it => it.Split(Path.DirectorySeparatorChar).Last()).ToArray());
            }

            return description;
        }

        private static GenericMenu CreateSceneSelectionMenu(SceneSelection value)
        {
            GenericMenu menu = new GenericMenu();
            GenericMenu.MenuFunction2 func = SceneSelectDropdownHandler(value);

            var allSceneGuids = AssetDatabase.FindAssets("t:scene", null).Select(it => new GUID(it));
            foreach (GUID guid in allSceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                int buildIndex = SceneUtility.GetBuildIndexByScenePath(path);

                Scene scene = SceneManager.GetSceneByPath(path);

                bool isSelected = value.SceneGUIDs?.Contains(guid) ?? false;
                if (buildIndex < 0)
                {
                    menu.AddDisabledItem(new GUIContent(path + " (not in build)"), isSelected);
                }
                else if (scene.IsValid())
                {
                    menu.AddItem(new GUIContent(path + " (currently loaded)"), isSelected, func, guid);
                }
                else
                {
                    menu.AddItem(new GUIContent(path), isSelected, func, guid);
                }
            }

            return menu;
        }

        private static GenericMenu.MenuFunction2 SceneSelectDropdownHandler(SceneSelection value)
        {
            return (object userData) =>
            {
                GUID guid = (GUID)userData;
                if (value.SceneGUIDs.Contains(guid))
                {
                    value.RemoveGUID(guid);
                }
                else
                {
                    value.AddGUID(guid);
                }
            };
        }

        private static void HandleSceneDragAndDrop(Rect rect, SceneSelection value)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        UnityEngine.Object ob = DragAndDrop.objectReferences[i];
                        if (ob is SceneAsset)
                        {
                            SceneAsset scene = (SceneAsset)ob;
                            string path = AssetDatabase.GetAssetPath(scene);
                            GUID guid = AssetDatabase.GUIDFromAssetPath(path);
                            value.AddGUID(guid);

                        }
                    }
                    Event.current.Use();
                }
            }
        }
    }
}