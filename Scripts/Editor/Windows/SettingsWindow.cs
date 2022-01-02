using System;
using SimpleSave.Models;
using SimpleSave.Services;
using UnityEditor;
using UnityEngine;

namespace SimpleSave
{
    internal class SettingsWindow : EditorWindow
    {
        private static SettingsContainer Container => SettingsContainer.Instance;

        private static Vector2 _scrollPosition;
        private static bool _specificSettingsFoldout;
        private static bool _debugSettingsFoldout;
        private static bool _aboutInformationFoldout;

        private static PackageInformation? _packageInfo;

        #region Tooltips

        private const string LoggingTooltip = "Logging type for none-critical errors during the saving and loading process.";
        private const string VersionTooltip = "Version of the save games.";
       
        private const string LocationTooltip = "Location to save and load the save games.";
        private const string SpecifiedLocationTooltip = "Additional relative location from the specified location.";
        private const string CustomLocationTooltip = "Custom absolute location.";
        private const string AutoCreateDirectoryTooltip = "Should the directory be created if necessary?";
        private const string OverrideSaveGamesTooltip = "Should existing save games be overridden?";

        private const string SceneLoadingTooltip = "How to handle scenes during the loading process.";
        private const string SaveGameEndConditionTooltip = "Condition to determine whether a save game ended.";

        private const string AlwaysSaveTaglessTooltip = "Should SaveItems and SaveVars that do not have any tags defined always be saved or loaded?";
        private const string AllowCrossComponentSavingTooltip = "Allow saving components from other non-child GameObjects.";
        private const string ScanAssembliesTooltip = "Assemblies to scan for SaveVar and SaveRef attributes.";

        private const string HotKeyTooltips = "Enable hotkeys for quick save and load.";
        private const string DebugLoggingTooltip = "Log debug information in the editor.";

        #endregion

        #region Setup

        [MenuItem("Window/Simple Save/Settings")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(SettingsWindow), false);
            SetupWindow(window);
        }

        private static void SetupWindow(EditorWindow window)
        {
            window.minSize = new Vector2(300, 400);
            window.maxSize = new Vector2(window.minSize.x + 100, window.minSize.y + 100);

            var icon = Resources.Load<Texture2D>("Icons/settings_window_icon");
            window.titleContent = new GUIContent("Settings", icon);
        }

        #endregion

        void OnGUI()
        {
            SerializedObject serializedSettings = new SerializedObject(Container);

            EditorGUILayout.Space(10);
            DrawWindow(serializedSettings);
            EditorGUILayout.Space(5);

            if (serializedSettings.hasModifiedProperties)
            {
                serializedSettings.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawWindow(SerializedObject serializedSettings)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawAreaSeparation();
            DrawGeneralSettings(serializedSettings);

            DrawAreaSeparation();
            DrawAdvancedSettings(serializedSettings);

            DrawAreaSeparation();
            DrawDebugSettings(serializedSettings);

            DrawAreaSeparation();
            DrawAboutInformation();

            EditorGUILayout.EndScrollView();
        }

        private static void DrawGeneralSettings(SerializedObject serializedSettings)
        {
            DrawHeader("General Settings");

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("LogType"), new GUIContent("Logging", LoggingTooltip));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("Version"), new GUIContent("Version", VersionTooltip));

            DrawGroupSeparation();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("Location"), new GUIContent("Location", LocationTooltip));

            if (Container.Location == Location.Custom)
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("CustomLocation"), new GUIContent("Custom Location", CustomLocationTooltip));
            }
            else if (Container.Location != Location.PlayerPrefs)
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("SpecifiedLocation"), new GUIContent("Specified", SpecifiedLocationTooltip));
            }

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("AutoCreateDirectory"), new GUIContent("Auto Create Directory", AutoCreateDirectoryTooltip));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("OverrideSaveGames"), new GUIContent("Override Save Games", OverrideSaveGamesTooltip));
        }

        private void DrawAdvancedSettings(SerializedObject serializedSettings)
        {
            DrawFoldout("Advanced Settings", 1, ref _specificSettingsFoldout, () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("SceneLoading"), new GUIContent("Scene Loading", SceneLoadingTooltip));
                
                DrawWidthLabelWidth(170, () =>
                {
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("SaveGameEndCondition"), new GUIContent("End Save Game Condition", SaveGameEndConditionTooltip));
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("AlwaysSaveTagless"), new GUIContent("Always Save Tagless", AlwaysSaveTaglessTooltip));
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("AllowCrossComponentSaving"), new GUIContent("Cross Component Saving", AllowCrossComponentSavingTooltip));
                });


                EditorGUILayout.PropertyField(serializedSettings.FindProperty("AssembliesToScan"), new GUIContent("Scan Assemblies", ScanAssembliesTooltip));
            });
        }

        private void DrawDebugSettings(SerializedObject serializedSettings)
        {
            DrawFoldout("Debug Settings", 1, ref _debugSettingsFoldout, () =>
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("HotKeysEnabled"), new GUIContent("Enable Hotkeys", HotKeyTooltips));
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("DebugLogging"), new GUIContent("Enable Logs", DebugLoggingTooltip));
            });
        }

        private void DrawAboutInformation()
        {
            DrawFoldout("About Simple Save", 1, ref _aboutInformationFoldout, () =>
            {
                if (!_packageInfo.HasValue)
                {
                    LoadPackageInformation();
                }

                EditorGUILayout.LabelField("Version", _packageInfo.HasValue? _packageInfo.Value.version : "failed to load package info.");
                EditorGUILayout.LabelField("Author", _packageInfo.HasValue ? _packageInfo.Value.author.name : "failed to load package info.");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Documentation");
                if (EditorGUILayout.LinkButton("Click here"))
                {
                    if (!_packageInfo.HasValue)
                    {
                        return;
                    }

                    Application.OpenURL(_packageInfo.Value.documentationUrl);
                }
                EditorGUILayout.EndHorizontal();
            });
        }

        private static void LoadPackageInformation()
        {
            _packageInfo = ServiceWrapper.GetService<IPackageHelper>().GetPackageInformation();
        }

        #region Helper

        private static void DrawHeader(string caption)
        {
            GUILayout.Label(caption, EditorStyles.boldLabel);
        }

        private static void DrawAreaSeparation()
        {
            GUILayout.Space(5);
        }

        private static void DrawGroupSeparation()
        {
            GUILayout.Space(5);
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

        private static void DrawWidthLabelWidth(int labelWidth, Action drawAction)
        {
            var prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            drawAction?.Invoke();
            EditorGUIUtility.labelWidth = prevWidth;
        }

        #endregion
    }
}
