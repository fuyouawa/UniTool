using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniTool.Attributes;
using UniTool.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UniTool.Editor.Utilities;
#endif

namespace UniTool.Tools
{
    public class AddFeedbackMenuAttribute : Attribute
    {
        public string Path { get; }

        public AddFeedbackMenuAttribute(string path)
        {
            Path = path;
        }
    }

    public class FeedbackHelperAttribute : Attribute
    {
        public string Message { get; }

        public FeedbackHelperAttribute(string message)
        {
            Message = message;
        }
    }


    [Serializable]
    public abstract class AbstractFeedback
    {
        [InfoBoxCN("@FeedbackHelpMessage", VisibleIf = "VisibleFeedbackHelper")]
        public string Label;
        // public Color BarColor = Color.black;
        public bool Enable = true;

        [FoldoutGroup("Feedback Settings")]
        [Title("Time")]
        [Tooltip("在正式Play前经过多少时间的延迟(s)")]
        public float DelayBeforePlay;
        [FoldoutGroup("Feedback Settings")]
        [Tooltip("是否会阻塞Feedbacks运行")]
        public bool Blocking;

        [FoldoutGroup("Feedback Settings")]
        [Title("Repeat")]
        [Tooltip("无限循环播放")]
        public bool RepeatForever = false;
        [FoldoutGroup("Feedback Settings")]
        [Tooltip("循环Play的次数")]
        public int AmountOfRepeat = 1;
        [FoldoutGroup("Feedback Settings")]
        [Tooltip("每次循环Play的间隔")]
        public float DelayBetweenRepeats = 0f;

        [FoldoutGroup("Feedback Settings")]
        [Title("Stop")]
        public bool StopAfterDuration = true;

        public Feedbacks Owner { get; private set; }
        public bool IsPlaying { get; protected set; }
        public float TimeSincePlay { get; protected set; }

        protected virtual IEnumerator FeedbackPlayPauser => null;

        protected bool IsInitialized = false;
        private List<Coroutine> _coroutines;
        private int _feedbackPlayCoCount = 0;

        public virtual void Setup(Feedbacks owner)
        {
            Owner = owner;
        }

        public virtual void Reset()
        {
            OnFeedbackReset();
        }

        public virtual IEnumerator PlayCo()
        {
            Reset();
            IsPlaying = true;
            TimeSincePlay = Time.time;

            if (!Blocking)
            {
                _coroutines.Add(StartCoroutine(FeedbackPlayCo()));
            }
            else
            {
                yield return FeedbackPlayCo();
            }
        }

        public virtual void Stop()
        {
            if (!IsPlaying)
                return;
            IsPlaying = false;

            foreach (var co in _coroutines)
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }
            }
            _coroutines.Clear();

            OnFeedbackStop();
        }

        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            _coroutines = new List<Coroutine>();

            OnFeedbackInit();
        }

        protected virtual IEnumerator FeedbackPlayCo()
        {
            _feedbackPlayCoCount++;

            var loop = Mathf.Min(AmountOfRepeat, 1);
            while (loop > 0 && IsPlaying)
            {
                // 如果不是第一次循环, 执行循环间隔
                if (loop < AmountOfRepeat && DelayBetweenRepeats > 0f)
                {
                    yield return new WaitForSeconds(DelayBetweenRepeats);
                }

                if (DelayBeforePlay > 0f)
                {
                    yield return new WaitForSeconds(DelayBeforePlay);
                }
                if (!IsPlaying)
                    yield break;

                OnFeedbackPlay();

                var p = FeedbackPlayPauser;
                if (p != null)
                {
                    yield return p;
                }

                var d = GetDuration();
                if (d > 0f)
                {
                    yield return new WaitForSeconds(d);
                }

                if (!RepeatForever)
                {
                    loop--;
                }
            }

            _feedbackPlayCoCount--;
            if (StopAfterDuration && _feedbackPlayCoCount == 0)
            {
                Stop();
            }
        }

        protected virtual void OnFeedbackInit() { }

        protected virtual void OnFeedbackReset() { }

        protected abstract void OnFeedbackPlay();

        protected abstract void OnFeedbackStop();

        public virtual void OnDestroy() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        public virtual float GetDuration()
        {
            return 0;
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            return Owner.CoroutineHelper.StartCoroutine(routine);
        }

        protected void StopCoroutine(IEnumerator routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopCoroutine(Coroutine routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopAllCoroutine()
        {
            Owner.CoroutineHelper.StopAllCoroutines();
        }

        // protected T InstantiateTemp<T>(T original, Transform parent) where T : Component
        // {
        //     var target = GameObject.Instantiate(original, parent);
        //     target.transform.SetPositionAndRotation(
        //         Owner.transform.position + original.transform.localPosition,
        //         Owner.transform.rotation * original.transform.rotation);
        //     var lifeTime = target.gameObject.AddComponent<LifeTime>();
        //     lifeTime.EnableLifeTime = StopAfterDuration;
        //     lifeTime.DestroyLifeTime = GetDuration();
        //     return target;
        // }

#if UNITY_EDITOR
        private string _message;
        private string FeedbackHelpMessage
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    var attr = GetType().GetCustomAttribute<FeedbackHelperAttribute>();
                    if (attr != null)
                    {
                        _message = attr.Message;
                    }
                }
                return _message;
            }
        }

        private bool VisibleFeedbackHelper => FeedbackHelpMessage.IsNotNullOrEmpty();

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
        public virtual void OnValidate() { }
        public virtual void OnInspectorInit() { }
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }


        [ButtonGroup]
        [DisableInEditorMode]
        private void TestPlay()
        {
            StartCoroutine(PlayCo());
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestStop()
        {
            Stop();
        }
#endif
    }

#if UNITY_EDITOR
    public class AbstractFeedbackDrawer : FoldableObjectDrawer<AbstractFeedback>
    {
        private static readonly float IconWidth = EditorGUIUtility.singleLineHeight;

        protected override void OnTitleBarGUI(Rect rect)
        {
            base.OnTitleBarGUI(rect);

            var value = ValueEntry.SmartValue;

            var buttonRect = new Rect(rect)
            {
                x = rect.x + 17,
                width = IconWidth,
                height = IconWidth
            };

            value.Enable = EditorGUI.Toggle(buttonRect, value.Enable);
        }

        protected override string GetRightLabel(GUIContent label)
        {
            var attr = ValueEntry.SmartValue.GetType().GetCustomAttribute<AddFeedbackMenuAttribute>();
            if (attr != null)
            {
                return $"[{attr.Path.Split("/").Last()}]";
            }
            return base.GetRightLabel(label);
        }

        protected override string GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return "      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label);
        }
    }
#endif
}
