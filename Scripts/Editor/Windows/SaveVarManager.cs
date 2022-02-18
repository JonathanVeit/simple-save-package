using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSave.Models;
using SimpleSave.Services;
using UnityEditor;
using UnityEngine;

namespace SimpleSave
{
    internal class SaveVarManagerWindow : EditorWindow
    {
        private const int itemExtraWidth = 50;

        private  readonly Dictionary<Type, bool> _foldOuts = new Dictionary<Type, bool>();
        private  Vector2 _scrollPosition;
        private  bool _hasMissingSaveRefAttribute;

        private  bool AllowEditing => Application.isPlaying;

        #region Services

        private static IAssemblyScanner AssemblyScanner => ServiceWrapper.GetService<IAssemblyScanner>();
        
        #endregion

        #region Setup

        [MenuItem("Window/Simple Save/SaveVar Manager")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(SaveVarManagerWindow), false);
            SetupWindow(window);
        }

        private static void SetupWindow(EditorWindow window)
        {
            window.minSize = new Vector2(400, 400);
            window.maxSize = new Vector2(window.minSize.x + 300, window.minSize.y + 100);

            var icon = Resources.Load<Texture2D>("Icons/settings_window_icon");
            window.titleContent = new GUIContent("SaveVar Manager", icon);
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        #endregion

        void OnGUI()
        {
            EditorGUILayout.Space(10);
            DrawHeader();
            DrawMissingSaveRefAttributeInfo();

            EditorGUILayout.Space(5);
            DrawList();
            DrawFoldButtons();
            EditorGUILayout.Space(5);
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("All current member using the [SaveVar] attribute.", (GUIStyle)"BoldLabel");
            if (!AllowEditing)
            {
                var style = new GUIStyle
                {
                    wordWrap = true,
                    fontStyle = FontStyle.Italic,
                    normal =
                    {
                        textColor = Color.yellow
                    },
                    contentOffset = new Vector2(3, 0)
                };

                EditorGUILayout.LabelField("Editing is only available at runtime.", style);
            }
        }

        private void DrawMissingSaveRefAttributeInfo()
        {
            if (_hasMissingSaveRefAttribute)
            {
                var style = new GUIStyle
                {
                    wordWrap = true,
                    fontStyle = FontStyle.Italic,
                    normal =
                    {
                        textColor = Color.red
                    },
                    contentOffset = new Vector2(3, 0)
                };

                EditorGUILayout.LabelField("(!) : the field is missing the [SaveRef] attribute. ", style);
            }
        }
        
        private void DrawFoldButtons()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");

            if (GUILayout.Button("Fold All", new GUIStyle("button") { fixedWidth = 70 }))
            {
                foreach (var key in _foldOuts.Keys.ToArray())
                {
                    _foldOuts[key] = false;
                }
            }

            if (GUILayout.Button("Unfold All", new GUIStyle("button") { fixedWidth = 70 }))
            {
                foreach (var key in _foldOuts.Keys.ToArray())
                {
                    _foldOuts[key] = true;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        #region List

        private void DrawList()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);
            
            var infosByType = GetInfosByDeclaringType();
            if (infosByType.Count > 0)
            {
                foreach (var entry in GetInfosByDeclaringType())
                {
                    DrawDeclaringTypeMember(entry.Key, entry.Value);
                    GUILayout.Space(2);
                }
            }
            else
            {
                EditorGUILayout.LabelField("There are no SaveVars yet.");
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawDeclaringTypeMember(Type declaringType, List<SaveVarInfo> saveVarInfos)
        {
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(25));

            if (!_foldOuts.ContainsKey(declaringType))
            {
                _foldOuts.Add(declaringType, true);
            }

            var foldoutBool = _foldOuts[declaringType];
            DrawFoldout($"{declaringType.Name} ({declaringType.FullName})", 2, ref foldoutBool, () =>
            {
                for (int i = 0; i < saveVarInfos.Count; i++)
                {
                    DrawMemberItem(saveVarInfos[i]);
                }

            });
            _foldOuts[declaringType] = foldoutBool;

            EditorGUILayout.EndVertical();
        }

        private void DrawMemberItem(SaveVarInfo saveVarInfo)
        {
            EditorGUI.BeginDisabledGroup(!AllowEditing);
            DrawMemberValue(saveVarInfo);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawMemberValue(SaveVarInfo saveVarInfo)
        {
            object curValue = null;
            object newValue = null;

            DrawWithExtraLabelWith(itemExtraWidth, () =>
            {
                curValue = GetMemberValue(saveVarInfo);

                if (saveVarInfo.MemberType == typeof(GameObject) ||
                    saveVarInfo.MemberType.IsSubclassOf(typeof(Component)))
                {
                    var hasSaveRefAttribute = HasSaveRefAttribute(saveVarInfo);
                    var memberName = (hasSaveRefAttribute ? "" : "(!) ") + saveVarInfo.MemberName;

                    newValue = EditorGUILayout.ObjectField(memberName,
                        (UnityEngine.Object)curValue, saveVarInfo.MemberType,
                        true);
                    return;
                }

                switch (curValue)
                {
                    case string asString:
                        newValue = EditorGUILayout.TextField(saveVarInfo.MemberName, asString);
                        break;
                    case int asInt:
                        newValue = EditorGUILayout.IntField(saveVarInfo.MemberName, asInt);
                        break;
                    case float asFloat:
                        newValue = EditorGUILayout.FloatField(saveVarInfo.MemberName, asFloat);
                        break;
                    case bool asBool:
                        newValue = EditorGUILayout.Toggle(saveVarInfo.MemberName, asBool);
                        break;

                    default:
                        if (saveVarInfo.MemberType == typeof(string))
                        {
                            newValue = EditorGUILayout.TextField(saveVarInfo.MemberName, curValue as string);
                            break;
                        }

                        EditorGUILayout.LabelField(saveVarInfo.MemberName, "Type not supported.");

                        newValue = null;
                        break;
                }
            });

            if (AllowEditing &&
                curValue != newValue && 
                !ObjectsAreSameValueType(curValue, newValue))
            {
                SetMemberValue(saveVarInfo, newValue);
            }
        }

        private object GetMemberValue(SaveVarInfo saveVarInfo)
        {
            object result;

            switch (saveVarInfo.MemberCategory)
            {
                case MemberCategory.Field:
                {
                    var fieldInfo =
                        saveVarInfo.DeclaringType.GetField(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);
                    result = fieldInfo.GetValue(null);
                    break;
                }
                case MemberCategory.Property:
                {
                    var propertyInfo =
                        saveVarInfo.DeclaringType.GetProperty(saveVarInfo.MemberName,
                            AssemblyScanner.ScanningFlags);
                    result = propertyInfo.GetValue(null);
                    break;
                }
                default:
                {
                    result = null;
                    break;
                }
            }

            return result;
        }

        private void SetMemberValue(SaveVarInfo saveVarInfo, object value)
        {
            switch (saveVarInfo.MemberCategory)
            {
                case MemberCategory.Field:
                {
                    var fieldInfo =
                        saveVarInfo.DeclaringType.GetField(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);

                    fieldInfo.SetValue(null, value);
                    break;
                }
                case MemberCategory.Property:
                {
                    var propertyInfo =
                        saveVarInfo.DeclaringType.GetProperty(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);

                    propertyInfo.SetValue(null, value);
                    break;
                }
            }
        }

        #endregion

        #region Helper

        private Dictionary<Type, List<SaveVarInfo>> GetInfosByDeclaringType()
        {
            var saveVarInfos = SaveVarInfoContainer.Instance.GetAll();

            var result = new Dictionary<Type, List<SaveVarInfo>>();

            for (int i = 0; i < saveVarInfos.Length; i++)
            {
                if (!result.ContainsKey(saveVarInfos[i].DeclaringType))
                {
                    result.Add(saveVarInfos[i].DeclaringType, new List<SaveVarInfo>());
                }

                result[saveVarInfos[i].DeclaringType].Add(saveVarInfos[i]);
            }

            return result;
        }

        private static void DrawFoldout(string caption, int indentLevel, ref bool controlBool, Action onFolded)
        {
            controlBool = EditorGUILayout.Foldout(controlBool, new GUIContent(caption), true);

            if (controlBool)
            {
                var prevLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel;
                onFolded?.Invoke();
                EditorGUI.indentLevel = prevLevel;
            }
        }

        private static void DrawWithExtraLabelWith(int extraWidth, Action onDraw)
        {
            var prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = prevWidth + extraWidth;
            onDraw?.Invoke();
            EditorGUIUtility.labelWidth = prevWidth;
        }

        private static bool ObjectsAreSameValueType(object object1, object object2)
        {
            if (object1 == null && object2 == null)
            {
                return true;
            }

            if (object1 == null ||
                object2 == null)
            {
                return false;
            }

            if (object1.GetType() != object2.GetType())
            {
                return false;
            }

            switch (object1)
            {
                case string asString:
                    return asString == (string) object2;
                case int asInt:
                    return asInt == (int)object2;
                case float asFloat:
                    return asFloat == (float)object2;
                case bool asBool:
                    return asBool == (bool)object2;

                default:
                    return false;
            }
        }

        private bool HasSaveRefAttribute(SaveVarInfo saveVarInfo)
        {
            if (ReferenceInfoContainer.Instance.TryGet(saveVarInfo.DeclaringType, out var referenceInfos))
            {
                for (int i = 0; i < referenceInfos.Length; i++)
                {
                    if (referenceInfos[i].MemberName == saveVarInfo.MemberName)
                    {
                        return true;
                    }
                }
            }

            _hasMissingSaveRefAttribute = true;
            return false;
        }

        #endregion
    }
}