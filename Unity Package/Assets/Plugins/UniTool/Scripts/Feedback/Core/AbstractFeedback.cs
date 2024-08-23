using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    public class AddFeedbackMenuAttribute : Attribute
    {
        public string Path;
        public string Message;
        public AddFeedbackMenuAttribute(string path)
        {
            Path = path;
            Message = string.Empty;
        }

        public AddFeedbackMenuAttribute(string path, string message)
        {
            Path = path;
            Message = message;
        }
    }


    [Serializable]
    public abstract class AbstractFeedback
    {
        // [Information("@")]
        [FoldoutGroup("Feedback Settings")]
        public string Label;
        // public Color BarColor = Color.black;
        [FoldoutGroup("Feedback Settings")]
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
        public float TotalDuration => DelayBeforePlay + GetDuration();
        public float TimeSincePlay { get; protected set; }
        public bool Expanded { get; set; }

        public string TitleLabel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Label))
                    return "TODO";
                return Label;
            }
        }

        // private string _message;
        // private string Message
        // {
        //     get
        //     {
        //         if (string.IsNullOrEmpty(_message))
        //         {
        //         }
        //     }
        // }

        protected virtual IEnumerator Pause => null;

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
                if (DelayBeforePlay > 0f)
                {
                    yield return new WaitForSeconds(DelayBeforePlay);
                }
                if (!IsPlaying)
                    yield break;

                OnFeedbackPlay();

                var p = Pause;
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
}