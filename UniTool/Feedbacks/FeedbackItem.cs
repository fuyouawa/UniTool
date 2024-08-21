using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections;
using UniTool.Attributes;
using UniTool.Extension;

namespace UniTool.Feedbacks
{
    public partial class Feedbacks
    {
        [Serializable]
        public class FeedbackItem
        {
            [ShowIf("@Feedback == null")]
            [HideLabel]
            [ValueDropdown("GetFeedbackNamesDropdown")]
            [Information("请选择一个Feedback", InfoMessageType.Error, VisibleIf = "NoneFeedbackName")]
            public string FeedbackName = string.Empty;

            [Information("@_help.Message", VisibleIf = "ShowInfoBox")]
            [Tooltip("是否有效")]
            public bool Enable = true;

            [Tooltip("只在编辑器中有用, 指定Feedback的名称")]
            public string Label;

            [BoxGroup("@FeedbackName"), HideLabel]
            [SerializeReference]
            [HideReferenceObjectPicker]
            public AbstractFeedback Feedback;

            public Feedbacks Owner { get; private set; }


            public void Setup(Feedbacks owner)
            {
                Owner = owner;
                Feedback?.Setup(owner);
            }

            public void Initialize()
            {
                if (!Enable) return;
                Feedback?.Initialize();
            }

            public void Play()
            {
                if (!Enable) return;
                Feedback?.Play();
            }

            public void Stop()
            {
                if (!Enable) return;
                Feedback?.Stop();
            }

            public void OnDestroy()
            {
                Feedback?.OnDestroy();
            }

            public void OnEnable()
            {
                if (!Enable) return;
                Feedback?.OnEnable();
            }

            public void OnDisable()
            {
                if (!Enable) return;
                Feedback?.OnDisable();
            }

            private static Dictionary<string, Type> s_allFeedbackTypes;
            private static ValueDropdownList<string> s_allFeedbackDropdownItems;

            private bool NoneFeedbackName => string.IsNullOrEmpty(FeedbackName);

            private IEnumerable GetFeedbackNamesDropdown()
            {
                if (s_allFeedbackTypes == null)
                {
                    s_allFeedbackTypes = (
                        from a in AppDomain.CurrentDomain.GetAssemblies()
                        where !a.FullName.StartsWith("UnityEngine.") &&
                              !a.FullName.StartsWith("UnityEditor.") &&
                              !a.FullName.StartsWith("Unity.") &&
                              !a.FullName.StartsWith("System.")
                        from t in a.GetTypes()
                        where typeof(AbstractFeedback).IsAssignableFrom(t) && t != typeof(AbstractFeedback) && t.HasCustomAttribute<AddFeedbackMenuAttribute>()
                        select t).ToDictionary(x => x.FullName, y => y);

                    s_allFeedbackDropdownItems = new ValueDropdownList<string>();

                    s_allFeedbackDropdownItems.Clear();
                    s_allFeedbackDropdownItems.Add("None", string.Empty);
                    foreach (var kv in s_allFeedbackTypes)
                    {
                        var attr = kv.Value.GetCustomAttribute<AddFeedbackMenuAttribute>();
                        var comment = string.IsNullOrEmpty(attr.Comment) ? string.Empty : $"\t## {attr.Comment}";
                        s_allFeedbackDropdownItems.Add($"{attr.Path}{comment}", kv.Value.FullName);
                    }
                }
                return s_allFeedbackDropdownItems;
            }

            private FeedbackHelpAttribute _help;

            private bool ShowInfoBox()
            {
                return Feedback != null && _help != null;
            }

            public void OnDrawGizmosSelected()
            {
                if (!Enable) return;
                Feedback?.OnDrawGizmosSelected();
            }

            public void OnDrawGizmos()
            {
                if (!Enable) return;
                Feedback?.OnDrawGizmos();
            }

            public void OnSceneGUI()
            {
                Feedback?.OnSceneGUI();
            }

            public void OnValidate()
            {
                Feedback?.OnValidate();
            }

            public void OnInspectorInit()
            {
                Feedback?.OnInspectorInit();
            }

            public void OnInspectorGUI()
            {
                if (!string.IsNullOrEmpty(FeedbackName))
                {
                    if ((Feedback == null || Feedback.GetType().FullName != FeedbackName) && Owner != null)
                    {
                        var type = s_allFeedbackTypes[FeedbackName];
                        Feedback = (AbstractFeedback)Activator.CreateInstance(type);
                        Feedback.Setup(Owner);
                        if (string.IsNullOrEmpty(Label))
                        {
                            var attr = type.GetCustomAttribute<AddFeedbackMenuAttribute>();
                            Label = attr.Path.Split('/').Last();
                        }
                    }
                }
                else
                {
                    Feedback = null;
                }

                if (_help == null)
                {
                    _help = Feedback?.GetType().GetCustomAttribute<FeedbackHelpAttribute>();
                }

                Feedback?.OnInspectorGUI();
            }

            [ButtonGroup]
            [DisableInEditorMode]
            private void TestPlay()
            {
                Play();
            }

            [ButtonGroup]
            [DisableInEditorMode]
            private void TestStop()
            {
                Stop();
            }
        }
    }
}
