using SimpleSave.Services;
using UnityEditor;
using UnityEngine;

namespace SimpleSave
{
    internal static class Tools
    {
        [MenuItem("Tools/Show Container")]
        public static void ShowContainer()
        {
            foreach (var container in GameObject.FindObjectsOfType<SaveItemContainer>())
            {
                container.gameObject.hideFlags = HideFlags.None;
            }
        }

        [MenuItem("Tools/Hide Container")]
        public static void HideContainer()
        {
            foreach (var container in GameObject.FindObjectsOfType<SaveItemContainer>())
            {
                container.gameObject.hideFlags =
                    HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
            }
        }
    }
}