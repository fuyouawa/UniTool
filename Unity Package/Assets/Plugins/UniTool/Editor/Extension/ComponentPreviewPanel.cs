using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniTool.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UniTool.Editor.Extension
{
    [CustomEditor(typeof(GameObject))]
    [CanEditMultipleObjects]
    public class ComponentPreviewPanel : ExtendEditor
    {
        public static int MaxTargetGameObjects = 10;

        protected override string EditorTypeName => "UnityEditor.GameObjectInspector, UnityEditor";

        private static class Icons
        {
            public static readonly GUIContent Expand = EditorGUIUtility.IconContent("winbtn_win_max");
            public static readonly GUIContent Collapse = EditorGUIUtility.IconContent("winbtn_win_min");
            public static readonly GUIContent Add = EditorGUIUtility.IconContent("d_Toolbar Plus More@2x");
            public static readonly GUIContent Remove = EditorGUIUtility.IconContent("CrossIcon");
            public static readonly GUIContent Edit = EditorGUIUtility.IconContent("d_Grid.PaintTool@2x");
            public static readonly GUIContent Warn = EditorGUIUtility.IconContent("d_console.warnicon");
        }

        private static class Styles
        {
            public static readonly GUIStyle Footer = "RL FooterButton";
        }

        private static readonly float BlockWidth = EditorGUIUtility.singleLineHeight;
        private static readonly string CopiedComponentIDPrefsKey = "Component Tool Panel Copied Component ID";

        private static MonoScript[] _allScripts;

        private static MonoScript[] AllScripts
        {
            get
            {
                if (_allScripts == null)
                {
                    _allScripts = MonoImporter.GetAllRuntimeMonoScripts();
                }

                return _allScripts;
            }
        }

        private class TargetItem
        {
            public List<Component> Components;
            private UniReorderableList _listDrawer;
            private bool _foldout;
            private GameObject _gameObject;
            private ComponentPreviewPanel _editor;

            private List<Component> GetComponents()
            {
                return _gameObject.GetComponents<Component>().Where(IsVisibleComponent).ToList();
            }

            public TargetItem(GameObject target, ComponentPreviewPanel editor)
            {
                _gameObject = target;
                _editor = editor;
                Components = GetComponents();
                InitListDrawer();
            }


            private void InitListDrawer()
            {
                _listDrawer = new UniReorderableList(new List<Component>(Components), true, true, true, false);
                _listDrawer.DrawElementBackgroundCallback += (rect, index, active, focused) =>
                {
                    if (index % 2 == 0)
                    {
                        EditorGUI.DrawRect(rect, new Color(0.23f, 0.23f, 0.23f));
                    }
                    else
                    {
                        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
                    }
                };
                _foldout = true;
                _listDrawer.DrawHeaderCallback += rect =>
                {
                    var rightBtnRect = new Rect(rect)
                    {
                        x = rect.xMax - BlockWidth,
                        width = BlockWidth
                    };
                    if (GUI.Button(rightBtnRect, new GUIContent(Icons.Add.image, "添加组件"), Styles.Footer))
                    {
                        AddComponentWindowHelper.Show(
                            new Rect(Screen.width / 2 - 230 / 2, rect.y + BlockWidth, 230, 0),
                            _editor._targetItems.Select(i => i._gameObject).ToArray());
                    }

                    rightBtnRect.x -= BlockWidth + 3;

                    if (GUI.Button(rightBtnRect, new GUIContent(Icons.Collapse.image, "折叠所有"), Styles.Footer))
                    {
                        foreach (var c in Components)
                        {
                            InternalEditorUtility.SetIsInspectorExpanded(c, false);
                        }

                        EditorHelper.ForceRebuildInspectors();
                    }

                    rightBtnRect.x -= BlockWidth + 3;

                    if (GUI.Button(rightBtnRect, new GUIContent(Icons.Expand.image, "展开所有"), Styles.Footer))
                    {
                        foreach (var c in Components)
                        {
                            InternalEditorUtility.SetIsInspectorExpanded(c, true);
                        }

                        EditorHelper.ForceRebuildInspectors();
                    }

                    var foldoutRect = new Rect(rect);
                    if (_editor._targetItems.Count > 1)
                    {
                        _foldout = EditorGUI.Foldout(foldoutRect, _foldout, new GUIContent($"组件预览面板（{_gameObject.name}）"), true);
                    }
                    else
                    {
                        _foldout = EditorGUI.Foldout(foldoutRect, _foldout, new GUIContent("组件预览面板"), true);
                    }
                };
                _listDrawer.DrawElementCallback += (rect, index, active, focused) =>
                {
                    rect.y += 2;
                    _editor.DrawElement(rect, Components[index], index);
                };
                _listDrawer.OnReorderDecide = (list, index, newIndex) =>
                {
                    if (index == 0)
                    {
                        return false;
                    }

                    return true;
                };
                _listDrawer.OnReorderCallback += internalList =>
                {
                    //Move Down
                    for (int i = 0; i < Components.Count; i++)
                    {
                        int indexOf = internalList.List.IndexOf(Components[i]);
                        int difference = indexOf - i;
                        if (difference <= 0) continue;

                        if (i == 0)
                        {
                            break;
                        }

                        for (int j = 0; j < Mathf.Abs(difference); j++)
                        {
                            ComponentUtility.MoveComponentDown(Components[i]);
                        }
                    }

                    //Move Up
                    Components = GetComponents();
                    for (int i = Components.Count - 1; i >= 0; i--)
                    {
                        int indexOf = internalList.List.IndexOf(Components[i]);
                        int difference = indexOf - i;
                        if (difference >= 0) continue;

                        if (i == 0)
                        {
                            break;
                        }

                        for (int j = 0; j < Mathf.Abs(difference); j++)
                        {
                            ComponentUtility.MoveComponentUp(Components[i]);
                        }
                    }
                };
            }

            public void DrawList()
            {
                _listDrawer.HasListElementTopPadding = false;
                _listDrawer.DoLayoutList(true, _foldout, false);
            }
        }

        private GameObject _gameObject;
        private List<TargetItem> _targetItems;

        protected override void OnEnable()
        {
            base.OnEnable();
            _gameObject = target as GameObject;
            _targetItems = new List<TargetItem>();
            foreach (var o in targets)
            {
                _targetItems.Add(new TargetItem((GameObject)o, this));
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            if (_targetItems.Count <= MaxTargetGameObjects)
            {
                foreach (var item in _targetItems)
                {
                    item.DrawList();
                }

                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.LabelField($"隐藏组件管理器(选中的GameObject超过了限制:{MaxTargetGameObjects})");
            }
        }

        private void DrawElement(Rect rect, Component component, int index)
        {
            if (component == null)
            {
                rect.x += 14;
                var iconRect = new Rect(rect);
                iconRect.width = iconRect.height = BlockWidth;

                EditorGUI.LabelField(iconRect, Icons.Warn);
                
                rect.x += (BlockWidth + 1) * 2;
                rect.y -= 2;
                //Name Handler
                GUIStyle guiStyle = new GUIStyle(EditorStyles.boldLabel);
                guiStyle.normal.textColor = guiStyle.onNormal.textColor = new Color32(209, 137, 24, 255);
                EditorGUI.LabelField(rect, "Mono脚本丢失！", guiStyle);
                return;
            }

            //Is the components common for every inspected GameObject?

            var foldoutRect = new Rect(rect);
            foldoutRect.y -= 2;

            var compIconRect = new Rect(rect);
            compIconRect.x += 14;
            compIconRect.width = compIconRect.height = BlockWidth;

            var enableToggleRect = new Rect(compIconRect);
            enableToggleRect.x += BlockWidth + 1;

            var componentLabelRect = new Rect(enableToggleRect)
            {
                width = rect.width - 125
            };
            componentLabelRect.x += BlockWidth + 1;

            var removeBtnRect = new Rect(rect)
            {
                x = rect.xMax
            };
            removeBtnRect.width = removeBtnRect.height = BlockWidth;
            removeBtnRect.x -= EditorGUIUtility.singleLineHeight;

            var editBtnRect = new Rect(removeBtnRect);
            editBtnRect.x -= EditorGUIUtility.singleLineHeight - 1;

            bool isCommon = targets.Length > 1 &&
                            _targetItems.Select(x => x.Components).ToList().TrueForAll(x =>
                                x.Exists(y => y != null &&
                                              y.GetType() == component.GetType()));


            // 先绘制Enable开关
            if (EditorUtility.GetObjectEnabled(component) != -1)
            {
                bool oldValue = EditorUtility.GetObjectEnabled(component) == 1;
                bool newValue = EditorGUI.Toggle(enableToggleRect, oldValue);
                if (oldValue != newValue)
                {
                    var targetComponents = GetTargetComponents(component);
                    foreach (var t in targetComponents)
                    {
                        Undo.RecordObject(t, (newValue ? "Enable " : "Disable ") + t.GetType().Name);
                        EditorUtility.SetObjectEnabled(t, newValue);
                    }
                }
            }

            // 删除按钮
            if (IsRemovableComponent(component) &&
                GUI.Button(removeBtnRect, new GUIContent(Icons.Remove.image, "删除组件"), Styles.Footer))
            {
                var targetComponents = GetTargetComponents(component);
                foreach (var t in targetComponents)
                {
                    if (IsDependantComponent(t, out var dependantTarget))
                    {
                        EditorUtility.DisplayDialog("错误", $"不能删除\"{t.GetType()}\"因为\"{dependantTarget.GetType()}\"依赖于它",
                            "确认");
                    }
                    else
                    {
                        Undo.SetCurrentGroupName("Remove " + t.GetType().Name);
                        Undo.DestroyObjectImmediate(t);
                    }
                }

                GUIUtility.ExitGUI();
            }

            var script = AllScripts.FirstOrDefault(s => s.GetClass() == component.GetType());
            if (script != null)
            {
                string path = AssetDatabase.GetAssetPath(script);

                if (path.EndsWith(".cs") || path.EndsWith(".js"))
                {
                    if (GUI.Button(editBtnRect, new GUIContent(Icons.Edit.image, "编辑脚本"), Styles.Footer))
                    {
                        if (Event.current.button == 0)
                            AssetDatabase.OpenAsset(script);
                        else
                            EditorGUIUtility.PingObject(script);
                    }
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var expand = EditorGUI.Foldout(foldoutRect, InternalEditorUtility.GetIsInspectorExpanded(component),
                    GUIContent.none, true);
                InternalEditorUtility.SetIsInspectorExpanded(component, expand);
                if (check.changed)
                {
                    bool value = InternalEditorUtility.GetIsInspectorExpanded(component);
                    var targetComponents = GetTargetComponents(component);
                    foreach (var c in targetComponents)
                        InternalEditorUtility.SetIsInspectorExpanded(c, value);

                    EditorHelper.ForceRebuildInspectors();
                }
            }

            Texture icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
            EditorGUI.LabelField(compIconRect, new GUIContent(icon));

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            if (isCommon)
                style.normal.textColor = style.onNormal.textColor = new Color(0f, 1f, 0.3f);
            EditorGUI.LabelField(componentLabelRect, component.GetType().Name, style);

            // //Reset Button
            // rect.x -= 18;
            // rect.width = 20;
            // rect.height += 4;
            // EditorGUI.LabelField(rect, new GUIContent(IconManager.ResetIcon.image, "Reset Component"));
            // if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            // {
            // 	var targetComponents = GetTargetComponents(component);
            // 	foreach (var t in targetComponents)
            // 	{
            // 		Undo.RecordObject(t, "Reset " + t.GetType().Name);
            // 		Unsupported.SmartReset(t);
            // 	}
            // }
            //
            // rect.height -= 4;
            //
            // //Copy Button
            // rect.x -= 17;
            // rect.height += 1;
            // EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image));
            // rect.x += 2;
            // rect.y += 2;
            // EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image, "Copy Component"));
            // rect.height -= 1;
            //
            // //Inspect
            // rect.x -= 18;
            // rect.y -= 2;

            //
            // rect.y += 2;
            //
            // //Paste Values Button
            // rect.height += 1;
            // rect.x -= 16;
            // if (CopiedComponent != null && CopiedComponent.GetType() == component.GetType())
            // {
            // 	rect.y -= 3;
            // 	EditorGUI.LabelField(rect, new GUIContent(IconManager.FolderIcon));
            // 	rect.x += 2;
            // 	rect.y += 3;
            // 	EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image, "Paste Component Values"));
            // 	if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            // 	{
            // 		var targetComponents = GetTargetComponents(component);
            // 		foreach (var t in targetComponents)
            // 			ComponentUtility.PasteComponentValues(t);
            // 	}
            //
            // 	rect.x -= 2;
            // }
        }

        private List<Component> GetTargetComponents(Component component)
        {
            var total = new List<Component>();

            if (component != null)
            {
                foreach (var c in _targetItems.SelectMany(i => i.Components))
                {
                    if (c == null)
                        continue;
                    if (c.GetType() == component.GetType())
                    {
                        total.Add(c);
                    }
                }
            }

            return total;
        }

        private static bool IsVisibleComponent(Component component)
        {
            if (component == null) return true;
            var t = component.GetType();
            if (t == typeof(ParticleSystemRenderer))
            {
                return false;
            }

            return true;
        }

        private static bool IsDependantComponent(Component component, out Component dependantTarget)
        {
            dependantTarget = null;
            if (component == null) return false;

            var components = component.GetComponents<Component>();
            var t = component.GetType();
            foreach (var c in components)
            {
                if (c == null) continue;
                var attrs = c.GetType().GetCustomAttributes<RequireComponent>();
                if (attrs.Any(attr => attr.m_Type0 == t || attr.m_Type1 == t || attr.m_Type2 == t))
                {
                    dependantTarget = c;
                    return true;
                }
            }

            return false;
        }

        private static bool IsRemovableComponent(Component component)
        {
            if (component == null) return true;

            var t = component.GetType();
            if (t == typeof(Transform) || t == typeof(RectTransform))
            {
                return false;
            }

            return true;
        }
    }
}
