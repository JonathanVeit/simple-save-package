using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleSave.Services;
using UnityEditor;
using SimpleSave.Models;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using EditorStyles = UnityEditor.EditorStyles;

namespace SimpleSave.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveItem))]
    internal class SaveItemInspector : Editor
    {
        private SaveItem Target => target as SaveItem;
        private SaveItemState State => CalculateState();
        private SaveItemId ItemId => CalculateId();

        private bool _tagsFolded;
        private Vector2 _tagsScrollPosition;
        private string _tagInput;

        private ReorderableList _componentList;
        private bool _editingItemId;
        private string _newItemId;

        #region Services

        private static ISaveItemHelper SaveItemHelper => ServiceWrapper.GetService<ISaveItemHelper>();

        private static ISaveItemContainerManager ContainerManager =>
            ServiceWrapper.GetService<ISaveItemContainerManager>();

        private static ITagManager TagManager => ServiceWrapper.GetService<ITagManager>();

        private static ILogger Logger => ServiceWrapper.GetService<ILogger>();

        #endregion

        #region Setup

        public void OnEnable()
        {
            SetupComponentList();
        }

        #endregion

        #region Component List

        private void SetupComponentList()
        {
            _componentList = new ReorderableList(Target.GetComponents(), typeof(ComponentInfo), true,
                true, true, true);
#if UNITY_2021_1_OR_NEWER
            _componentList.multiSelect = true;
#endif
            _componentList.drawHeaderCallback = DrawComponentListHeader;
            _componentList.drawElementCallback = DrawComponentListElement;
            _componentList.onAddCallback = AddElementToComponentList;
            _componentList.onRemoveCallback = RemoveElementFromComponentList;
        }

        private void DrawComponentListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Save Components");
        }

        private void DrawComponentListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;
            GUIContent objectLabel = new GUIContent($"Component {index + 1}");

            var savedComponents = Target.GetComponents();
            var component = savedComponents[index].Component;

            savedComponents[index]
                .SetComponent((Component)EditorGUI.ObjectField(rect, objectLabel, component, typeof(Component), true));

            if (Selection.count == 1 || 
                component == savedComponents[index].Component)
            {
                return;
            }

            for (int i = 0; i < Selection.count; i++)
            {
                if (!(Selection.objects[i] is GameObject asGameObject))
                {
                    continue;
                }

                var curSaveItem = asGameObject.GetComponent<SaveItem>();
                if (curSaveItem == Target)
                {
                    continue;
                }

                if (curSaveItem.GetComponents().Count < i - 1)
                {
                    continue;
                }

                curSaveItem.GetComponents()[index]
                    .SetComponent(savedComponents[index].Component);
            }
        }

        private void AddElementToComponentList(ReorderableList list)
        {
            for (int i = 0; i < Selection.count; i++)
            {
                if (!(Selection.objects[i] is GameObject asGameObject))
                {
                    continue;
                }

                var curSaveItem = asGameObject.GetComponent<SaveItem>();
                curSaveItem.AddComponent(null);
            }
        }

        private void RemoveElementFromComponentList(ReorderableList list)
        {
            for (int i = 0; i < Selection.count; i++)
            {
                if (!(Selection.objects[i] is GameObject asGameObject))
                {
                    continue;
                }

                var curSaveItem = asGameObject.GetComponent<SaveItem>();

#if UNITY_2021_1_OR_NEWER
                for (int j = list.selectedIndices.Count - 1; j >= 0; j--)
                {
                    var index = list.selectedIndices[j];
                    
                    if (index > curSaveItem.GetComponents().Count - 1)
                    {
                        continue;
                    }

                    curSaveItem.GetComponents().RemoveAt(index);
                }
#else
                curSaveItem.GetComponents().RemoveAt(list.index);
#endif
            }
        }

#endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            switch (State)
            {
                case SaveItemState.Scene:
                    DrawSceneTypeGUI(Application.isPlaying);
                    break;

                case SaveItemState.Prefab:
                    DrawPrefabTypeGUI(Application.isPlaying);
                    break;

                case SaveItemState.PrefabInstance:
                    DrawPrefabInstanceTypeGUI(Application.isPlaying);
                    break;

                default:
                    Debug.LogError($"{nameof(SaveItemInspector)} cannot display item state {Target.State}.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed && 
                !Application.isPlaying)
            {
                EditorUtility.SetDirty(Target);
                EditorSceneManager.MarkSceneDirty(Target.gameObject.scene);
            }
        }

        private void DrawSceneTypeGUI(bool runtime)
        {
            DrawIdAndState();
            DrawHeader("Settings");
            if (runtime)
            {
                DrawRuntimeInformation();
            }

            DrawTags(runtime);
            DrawSaveProperty(runtime);
            DrawComponentList(runtime);
        }

        private void DrawPrefabTypeGUI(bool runtime)
        {
            DrawIdAndState();
            DrawHeader("Settings");
            if (runtime)
            {
                DrawRuntimeInformation();
            }

            DrawPrefabId(runtime);
            DrawTags(runtime);
            DrawSaveProperty(runtime);
            DrawComponentList(runtime);
        }

        private void DrawPrefabInstanceTypeGUI(bool runtime)
        {
            DrawIdAndState();
            DrawHeader("Settings");
            if (runtime)
            {
                DrawRuntimeInformation();
            }

            DrawPrefabId(true);
            DrawTags(runtime);
            DrawSaveProperty(runtime);
            DrawComponentList(runtime);
        }

#region Id and State

        private void DrawIdAndState()
        {
            DrawId();
            DrawLabel("State", GetStateAsString(State));
        }

        private void DrawId()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(25));
            EditorGUILayout.BeginHorizontal();

            if (!_editingItemId)
            {
                var identifier = State == SaveItemState.Prefab ? "will be set runtime" : ItemId.ToString();
                DrawLabel("Identifier", identifier);

                if (State != SaveItemState.Prefab &&
                    !Application.isPlaying &&
                    Selection.count == 1)
                {
                    DrawIdEditButton();
                }
            }
            else
            {
                _newItemId = EditorGUILayout.TextField("Identifier", _newItemId);
                DrawSaveIdButton();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawIdEditButton()
        {
            var icon = Resources.Load("icons/edit_icon") as Texture2D;
            var style = new GUIStyle
            {
                normal =
                {
                    background = icon
                },
            };

            if (GUILayout.Button("", style, GUILayout.Width(16), GUILayout.Height(16)))
            {
                _newItemId = ContainerManager.GetContainerFor(Target).GetIdFor(Target);
                _editingItemId = !_editingItemId;
            }
        }

        private void DrawSaveIdButton()
        {
            if (!GUILayout.Button("Save", GUILayout.Width(40), GUILayout.Height(18)))
            {
                return;
            }

            if (ItemId == _newItemId)
            {
                _editingItemId = false;
                return;
            }

            if (_newItemId.Contains(":"))
            {
                Logger.Log(LogType.Error, $"The character \":\" is not supported. Please choose another id.");
                return;
            }

            if (string.IsNullOrEmpty(_newItemId))
            {
                return;
            }

            if (ContainerManager.GetContainerFor(Target).ContainsItemWithId(_newItemId))
            {
                Logger.Log(LogType.Error, $"There is already a {nameof(SaveItem)} with id \"{_newItemId}\" in this scene.");
                return;
            }

            ContainerManager.GetContainerFor(Target).OverrideId(Target, new SaveItemId(_newItemId));
            _editingItemId = false;

            EditorSceneManager.MarkSceneDirty(Target.gameObject.scene);
        }

#endregion

        private void DrawHeader(string name)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(name, (GUIStyle)"BoldLabel");
        }

#region Tags

        private void DrawTags(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);

            var tags = GetTagsToDisplay();

            var tagPreview = CreateTagPreview(tags);
            _tagsFolded = EditorGUILayout.Foldout(_tagsFolded, tagPreview);

            if (_tagsFolded)
            {
                DrawTagList(tags);
                EditorGUILayout.Space(2);

                DrawTagSearchBar();
                EditorGUILayout.Space(5);
            }

            EditorGUI.EndDisabledGroup();
        }
        private TagId[] GetTagsToDisplay()
        {
            if (Selection.count == 1)
            {
                ValidateItemTags(Target);
                return Target.GetTags();
            }

            var result = new List<TagId>();

            for (int i = 0; i < Selection.count; i++)
            {
                if (!(Selection.objects[i] is GameObject asGameObject))
                {
                    continue;
                }

                var curSaveItem = asGameObject.GetComponent<SaveItem>();

                if (i == 0)
                {
                    result.AddRange(curSaveItem.GetTags());
                    continue;
                }

                var tmp = new List<TagId>(result);
                result.Clear();
                result.AddRange(curSaveItem.GetTags().Intersect(tmp));
            }

            return result.ToArray();
        }

        private void ValidateItemTags(SaveItem item)
        {
            var tags = item.GetTags();

            for (int i = tags.Length - 1; i >= 0 ; i--)
            {
                if (!TagManager.TagExists(tags[i]))
                {
                    Target.RemoveTag(tags[i]);
                }
            }
        }

        private string CreateTagPreview(TagId[] tagIds)
        {
            var sb = new StringBuilder("Tags (");

            if (tagIds.Length == 0)
            {
                sb.Append("0)");
                return sb.ToString();
            }

            for (int i = 0; i < 2 && i < tagIds.Length; i++)
            {
                if (TagManager.TryGetTagInfo(tagIds[i], out var tagInfo))
                {
                    sb.Append(tagInfo.Name);

                    if (i < 1 &&
                        i < tagIds.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }

            if (tagIds.Length > 2)
            {
                sb.Append(", ...");
            }

            sb.Append(")");
            return sb.ToString();
        }

        private void DrawTagList(TagId[] tags)
        {
            _tagsScrollPosition = EditorGUILayout.BeginScrollView(_tagsScrollPosition, EditorStyles.helpBox, GUILayout.Height(83));

            for (var i = 0; i < tags.Length; i++)
            {
                var tagId = tags[i];
                if (!TagManager.TryGetTagInfo(tagId, out TagInfo tagInfo))
                {
                    Target.RemoveTag(tagId);
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                DrawTagItem(tagInfo);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTagItem(TagInfo tagInfo)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            var labelStyle = new GUIStyle("boldLabel");
            labelStyle.stretchWidth = false;
            EditorGUILayout.LabelField(new GUIContent(tagInfo.Name, tagInfo.Description), labelStyle, GUILayout.MinWidth(1));
           
            var deleteIcon = Resources.Load("icons/delete_icon") as Texture2D;
            var buttonStyle = new GUIStyle
            {
                normal =
                {
                    background = deleteIcon
                },
            };

            if (GUILayout.Button("", buttonStyle, GUILayout.Width(16), GUILayout.Height(16), GUILayout.MinWidth(1)))
            {
                for (int i = 0; i < Selection.count; i++)
                {
                    if (!(Selection.objects[i] is GameObject asGameObject))
                    {
                        continue;
                    }

                    var curSaveItem = asGameObject.GetComponent<SaveItem>();
                    curSaveItem.RemoveTag(tagInfo.Id);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTagSearchBar()
        {
            EditorGUILayout.BeginHorizontal();

            var tagExists = TagManager.TagExists(_tagInput);
            var itemHasTag = ItemHasTag(_tagInput);
            var textColor = tagExists && !itemHasTag ? Color.green : Color.red;

            var style = new GUIStyle("textfield")
            {
                normal =
                {
                    textColor = textColor
                },
                active =
                {
                    textColor = textColor
                },
                focused =
                {
                    textColor = textColor
                },
                hover =
                {
                    textColor = textColor
                }
            };

            _tagInput = EditorGUILayout.TextField("", _tagInput, style);

            if (GUILayout.Button("Add") &&
                !string.IsNullOrEmpty(_tagInput))
            {
                if (itemHasTag)
                {
                    EditorUtility.DisplayDialog("Error", $"The item already has the tag \"{_tagInput}\".", "OK");
                }
                else if (TagManager.TryGetTagInfo(_tagInput, out TagInfo tagInfo))
                {
                    for (int i = 0; i < Selection.count; i++)
                    {
                        if (!(Selection.objects[i] is GameObject asGameObject))
                        {
                            continue;
                        }

                        var curSaveItem = asGameObject.GetComponent<SaveItem>();

                        if (!curSaveItem.HasTag(tagInfo.Id))
                        {
                            curSaveItem.AddTag(tagInfo.Id);
                        }
                    }

                    _tagInput = string.Empty;
                }
                else if (TagManager.GetAllTagInfos().Length >= TagManager.MaxTagCount)
                {
                    EditorUtility.DisplayDialog("Error",
                        $"The maximum amount of tags is reached.", "Ok");
                }
                else if(EditorUtility.DisplayDialog("Add Tag",
                    $"The tag \"{_tagInput}\" does not exist yet. Do you want to create it?", "Yes", "No"))
                {
                    if (TagManager.TryCreateTag(_tagInput, string.Empty, out var createdTagInfo))
                    {
                        for (int i = 0; i < Selection.count; i++)
                        {
                            if (!(Selection.objects[i] is GameObject asGameObject))
                            {
                                continue;
                            }

                            var curSaveItem = asGameObject.GetComponent<SaveItem>();

                            if (!curSaveItem.HasTag(createdTagInfo.Id))
                            {
                                curSaveItem.AddTag(createdTagInfo.Id);
                            }
                        }
                        
                        _tagInput = string.Empty;
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool ItemHasTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return false;
            }

            if (!TagManager.TryGetTagInfo(tagName, out TagInfo tagInfo))
            {
                return false;
            }

            for (int i = 0; i < Selection.count; i++)
            {
                if (!(Selection.objects[i] is GameObject asGameObject))
                {
                    continue;
                }

                var curSaveItem = asGameObject.GetComponent<SaveItem>();
                if (!curSaveItem.HasTag(tagInfo.Id))
                {
                    return false;
                }
            }

            return true;
        }

#endregion

        private void DrawSaveProperty(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);

            var property = serializedObject.FindProperty("_properties");
            EditorGUILayout.PropertyField(property, new GUIContent("Save Properties", "Properties of the GameObject to be saved."));
          
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPrefabId(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);

            var prevPrefabId = Target.PrefabId;
            var property = serializedObject.FindProperty("_prefabId");
            EditorGUILayout.PropertyField(property, new GUIContent("Prefab Id", "Identifier of the prefab which will be used to load the item."));
            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = prevPrefabId;
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawComponentList(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);
            EditorGUILayout.Space(5);
            _componentList.DoLayoutList();
            Target.ValidateComponents();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawRuntimeInformation()
        {
            var style = new GUIStyle();
            style.wordWrap = true;
            style.fontStyle = FontStyle.Italic;
            style.normal.textColor = Color.yellow;

            EditorGUILayout.LabelField("Runtime editing is not supported.", style);
        }

#region Helper

        private SaveItemId CalculateId()
        {
            return Application.isPlaying ? Target.Id : ContainerManager.GetContainerFor(Target).GetIdFor(Target);
        }

        private SaveItemState CalculateState()
        {
            var result = SaveItemHelper.GetItemState(Target);
            if (result == SaveItemState.Prefab)
            {
                return result;
            }

            if (Application.isPlaying)
            {
                return Target.State;
            }

            return result;
        }

        private void DrawLabel(string identifier, string value)
        {
            EditorGUILayout.LabelField(identifier, value);
        }

        private string GetStateAsString(SaveItemState state)
        {
            switch (state)
            {
                case SaveItemState.Uninitialized:
                    return "Uninitialized. Something went wrong!";
                case SaveItemState.Scene:
                    return "Scene";
                case SaveItemState.Prefab:
                    return "Prefab";
                case SaveItemState.PrefabInstance:
                    return "Prefab Instance";
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
#endregion
    }
}