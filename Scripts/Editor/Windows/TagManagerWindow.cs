using SimpleSave.Models;
using SimpleSave.Services;
using UnityEditor;
using UnityEngine;

namespace SimpleSave
{
    internal class TagManagerWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private static GUIStyle _editButtonStyle;
        private static GUIStyle _deleteButtonStyle;

        private TagId? _editingTagId;
        private string _editTagName;
        private string _editTagDescription;

        private string _newTagName;
        private string _newTagDescription;

        #region Services

        private static ITagManager TagManager => ServiceWrapper.GetService<ITagManager>();
        
        #endregion

        #region Setup

        [MenuItem("Window/Simple Save/Tag Manager")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(TagManagerWindow), false);
            SetupWindow(window);
        }

        private static void SetupWindow(EditorWindow window)
        {
            window.minSize = new Vector2(400, 400);
            window.maxSize = new Vector2(window.minSize.x + 300, window.minSize.y + 100);

            var icon = Resources.Load<Texture2D>("Icons/settings_window_icon");
            window.titleContent = new GUIContent("Tag Manager", icon);
        }

        #endregion

        void OnGUI()
        {
            EditorGUILayout.Space(10);
            DrawHeader();
            DrawList();
            EditorGUILayout.Space(5);
        }

        private void DrawHeader()
        {
            int tagAmount = TagManager.GetAllTagInfos().Length;

            EditorGUILayout.LabelField($"All defined tags ({tagAmount}/{TagManager.MaxTagCount}).", (GUIStyle)"BoldLabel");
        }

        #region List

        private void DrawList()
        {
            DrawListHeader();
             
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);

            var tagInfos = TagManager.GetAllTagInfos();
            for (int i = 0; i < tagInfos.Length; i++)
            {
                DrawTag(tagInfos[i], i % 2 == 0);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(5);
            DrawTagCreationMenu();
        }

        private void DrawListHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", (GUIStyle)"BoldLabel", GUILayout.MinWidth(1));
            EditorGUILayout.LabelField("Description", (GUIStyle)"BoldLabel", GUILayout.MinWidth(1));
            DrawSpacingLabel(46); 
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTag(TagInfo tagInfo, bool colored)
        {
            if (_editingTagId.HasValue &&
                _editingTagId.Value == tagInfo.Id)
            {
                DrawTagEditMode(tagInfo, colored);
            }
            else
            {
                DrawTagNormal(tagInfo, colored);
            }
        }

        private void DrawTagNormal(TagInfo tagInfo, bool colored)
        {
            EditorGUILayout.BeginHorizontal(colored? GUI.skin.box : GUI.skin.label);

            EditorGUILayout.LabelField(tagInfo.Name, GUILayout.MinWidth(1));
            EditorGUILayout.LabelField(tagInfo.Description, GUILayout.MinWidth(1));

            DrawSpacingLabel(1);
            _editButtonStyle ??= new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("icons/edit_icon") as Texture2D
                },
            };
            if (GUILayout.Button("", _editButtonStyle, GUILayout.Width(16), GUILayout.Height(16)))
            {
                _editingTagId = tagInfo.Id;
                _editTagName = tagInfo.Name;
                _editTagDescription = tagInfo.Description;
            }

            DrawSpacingLabel(1);
            _deleteButtonStyle ??= new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("icons/delete_icon") as Texture2D
                },
            };
            if (GUILayout.Button("", _deleteButtonStyle, GUILayout.Width(16), GUILayout.Height(16)))
            {
                if (EditorUtility.DisplayDialog("Delete Tag", $"Are you sure you want to delete the tag \"{tagInfo.Name}\"?", "Yes", "No"))
                {
                    TagManager.DeleteTag(tagInfo.Id);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTagEditMode(TagInfo tagInfo, bool colored)
        {
            EditorGUILayout.BeginHorizontal(colored ? GUI.skin.box : GUI.skin.label);

            _editTagName = EditorGUILayout.TextField("", _editTagName, GUILayout.MinWidth(1));
            _editTagDescription = EditorGUILayout.TextField("", _editTagDescription, GUILayout.MinWidth(1));

            DrawSpacingLabel(1);
            if (GUILayout.Button("Save", GUILayout.Width(45)))
            {
                if (_editTagName != tagInfo.Name)
                {
                    if (!TagNameIsAvailable(tagInfo, _editTagName))
                    {
                        EditorUtility.DisplayDialog("Error", "There is already a tag with this name!", "Ok");
                        return;
                    }

                    TagManager.UpdateTagName(tagInfo.Id, _editTagName);
                }

                if (_editTagDescription != tagInfo.Description)
                {
                    TagManager.UpdateTagDescription(tagInfo.Id, _editTagDescription);
                }

                _editingTagId = null;
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool TagNameIsAvailable(TagInfo tagToCompare, string tagName)
        {
            var tagInfos = TagManager.GetAllTagInfos();

            for (int i = 0; i < tagInfos.Length; i++)
            {
                if (tagInfos[i].Name == tagName && 
                    tagInfos[i].Id != tagToCompare.Id)
                {
                    return false;
                }
            }

            return true;
        }

        private void DrawTagCreationMenu()
        {
            EditorGUI.BeginDisabledGroup(TagManager.GetAllTagInfos().Length >= TagManager.MaxTagCount);

            EditorGUILayout.LabelField("Create Tag", (GUIStyle)"BoldLabel");

            _newTagName = EditorGUILayout.TextField("Name", _newTagName);
            _newTagDescription = EditorGUILayout.TextField("Description", _newTagDescription);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Empty);

            if (GUILayout.Button("Create"))
            {
                if (string.IsNullOrEmpty(_newTagName))
                {
                    return;
                }

                if (TagManager.TagExists(_newTagName))
                {
                    return;
                }

                TagManager.TryCreateTag(_newTagName, _newTagDescription, out var tagInfo);
                
                _newTagName = string.Empty;
                _newTagDescription = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        #endregion

        #region Helper

        private void DrawSpacingLabel(int width)
        {
            EditorGUILayout.LabelField("", GUILayout.Width(width));
        }

        #endregion
    }
}