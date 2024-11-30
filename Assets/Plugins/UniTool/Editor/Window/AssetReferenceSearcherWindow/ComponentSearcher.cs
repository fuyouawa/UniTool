using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UniTool.Editor.Utilities;
using UniTool.Utilities;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace UniTool.Editor.Window
{
    public class ComponentSearcher
    {
        public enum Modes
        {
            [LabelText("在场景中搜索")]
            InScene,

            [LabelText("在资产中搜索")]
            InAssets
        }

        public Type TypeToSearch;
        [LabelText("模式")]
        public Modes Mode;
        public List<Component> Results = new List<Component>();

        public bool ShowError;

        public class ResultTreeNode
        {
            public bool Expand;
            public string Name;
            public ResultTreeNode Parent;
            public Dictionary<string, ResultTreeNode> Children;

            public ResultTreeNode(string name, ResultTreeNode parent = null)
            {
                Name = name;
                Expand = false;
                Children = new Dictionary<string, ResultTreeNode>();
                Parent = parent;
            }

            public int CalcHierarchy()
            {
                int res = 0;
                var p = Parent;
                while (p != null)
                {
                    ++res;
                    p = p.Parent;
                }

                return res;
            }

            public string GetAbsPath()
            {
                string path = Name;
                var p = Parent;
                while (p != null)
                {
                    path = p.Name + "/" + path;
                    p = p.Parent;
                }
                return path;
            }
        }

        public Dictionary<string, ResultTreeNode> ResultTree = new Dictionary<string, ResultTreeNode>();
        private static Type[] _allComponentTypes;

        public static Type[] AllComponentTypes
        {
            get
            {
                if (_allComponentTypes == null)
                {
                    _allComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(Component))).ToArray();
                }

                return _allComponentTypes;
            }
        }

        public void GenerateResultTree()
        {
            ResultTree.Clear();
            foreach (var result in Results)
            {
                var path = result.transform.GetAbsolutePath();
                path = path[1..];

                var split = path.Split('/');

                var s0 = split[0];
                if (!ResultTree.TryGetValue(s0, out var node))
                {
                    node = new ResultTreeNode(s0);
                    ResultTree[s0] = node;
                }

                for (int i = 1; i < split.Length; i++)
                {
                    var s = split[i];
                    if (!node.Children.TryGetValue(s, out var node2))
                    {
                        node2 = new ResultTreeNode(s, node);
                        node.Children[s] = node2;
                        node = node2;
                    }
                    else
                    {
                        node = node2;
                    }
                }
            }
        }
    }

    public class ComponentSearcherDrawer : OdinValueDrawer<ComponentSearcher>
    {
        void DrawResultTree(Dictionary<string, ComponentSearcher.ResultTreeNode> resultTree)
        {
            if (resultTree == null || resultTree.Count == 0)
                return;

            foreach (var node in resultTree)
            {
                DrawResultTreeNode(node);
            }
        }

        void DrawResultTreeNode(KeyValuePair<string, ComponentSearcher.ResultTreeNode> node)
        {
            var off = node.Value.CalcHierarchy() * 10;
            var rect = EditorGUILayout.GetControlRect();
            rect.x += off;
            rect.width -= off;

            if (node.Value.Children.Count == 0)
            {
                if (node.Value.Parent.Expand)
                {
                    rect.x += 5;
                    rect.width -= 5;

                    if (ValueEntry.SmartValue.Mode == ComponentSearcher.Modes.InScene)
                    {
                        var path = node.Value.GetAbsPath();
                        var go = GameObjectHelper.FindByAbsolutePath(path);
                        if (go != null)
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                EditorGUI.ObjectField(rect, go, typeof(GameObject), true);
                            }
                        }
                        else
                        {
                            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                            style.normal.textColor = style.onNormal.textColor = new Color32(209, 137, 24, 255);
                            EditorGUI.LabelField(rect, "Missing GameObject!", style);
                        }
                    }
                }
            }
            else
            {
                var options = new FoldoutGroupOptions()
                {
                    HasBox = false,
                    OnContentGUI = () => { DrawResultTree(node.Value.Children); }
                };
                node.Value.Expand = UniEditorGUI.FoldoutGroup(rect, node.Key, node.Value.Expand, node, options);
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            UniEditorGUI.Title("参数设置");
            var val = ValueEntry.SmartValue;
            if (val.ShowError)
            {
                SirenixEditorGUI.ErrorMessageBox("无效类型");
            }

            UniEditorGUI.DrawSelectorDropdown(
                "要查找的类型",
                val.TypeToSearch == null ? "None" : val.TypeToSearch.FullName,
                ComponentSearcher.AllComponentTypes,
                t => { val.TypeToSearch = t; });

            Property.Children["Mode"].Draw();

            var btnRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2);
            if (SirenixEditorGUI.SDFIconButton(btnRect, "查找引用", SdfIconType.Search))
            {
                val.Results.Clear();

                var t = val.TypeToSearch;
                val.ShowError = t == null;
                if (t != null)
                {
                    if (val.Mode == ComponentSearcher.Modes.InScene)
                    {
                        for (int i = 0; i < SceneManager.sceneCount; i++)
                        {
                            var scene = SceneManager.GetSceneAt(i);
                            if (scene.IsValid() && scene.isLoaded)
                            {
                                foreach (var o in scene.GetRootGameObjects())
                                {
                                    val.Results.AddRange(o.GetComponentsInChildren(t));
                                }
                            }
                        }
                    }

                    val.GenerateResultTree();
                }
            }

            SirenixEditorGUI.BeginBox("结果视图");

            DrawResultTree(val.ResultTree);

            SirenixEditorGUI.EndBox();
        }
    }
}
