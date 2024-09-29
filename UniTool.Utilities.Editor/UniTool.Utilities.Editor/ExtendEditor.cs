using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniTool.Utilities.Editor
{
    public abstract class ExtendEditor : UnityEditor.Editor
    {
        protected UnityEditor.Editor DefaultEditor;

        protected abstract string EditorTypeName { get; }

        protected virtual void OnEnable()
        {
            DefaultEditor = CreateEditor(targets, Type.GetType(EditorTypeName));
        }

        protected virtual void OnDisable()
        {
            try
            {
                MethodInfo disableMethod = DefaultEditor.GetType().GetMethod("OnDisable",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (disableMethod != null)
                    disableMethod.Invoke(DefaultEditor, null);
                DestroyImmediate(DefaultEditor);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public override void OnInspectorGUI()
        {
            DefaultEditor.OnInspectorGUI();
        }

        protected override void OnHeaderGUI()
        {
            DefaultEditor.DrawHeader();
        }

        public override bool HasPreviewGUI()
        {
            return DefaultEditor.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            DefaultEditor.OnPreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            DefaultEditor.OnPreviewSettings();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return DefaultEditor.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override void ReloadPreviewInstances()
        {
            DefaultEditor.ReloadPreviewInstances();
        }

        public override void DrawPreview(Rect previewArea)
        {
            DefaultEditor.DrawPreview(previewArea);
        }

        // public override VisualElement CreateInspectorGUI()
        // {
        //     return DefaultEditor.CreateInspectorGUI();
        // }

        public override GUIContent GetPreviewTitle()
        {
            return DefaultEditor.GetPreviewTitle();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            DefaultEditor.OnInteractivePreviewGUI(r, background);
        }
    }
}
