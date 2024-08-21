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
        public string Comment;

        public AddFeedbackMenuAttribute(string path, string comment = null)
        {
            Path = path;
            Comment = comment;
        }
    }

    public class FeedbackHelpAttribute : Attribute
    {
        public string Message;

        public FeedbackHelpAttribute(string message)
        {
            Message = message;
        }
    }


    [Serializable]
    public abstract class AbstractFeedback
    {
        public enum InitializeModes
        {
            Awake,
            Start,
            FirstPlay
        }

        [FoldoutGroup("Feedback Settings")]
        [Title("Time")]
        [Tooltip("在正式Play前经过多少时间的延迟(s)")]
        public float DelayBeforePlay;

        [FoldoutGroup("Feedback Settings")]
        [Tooltip("循环Play的次数")]
        public int AmountOfLoop = 1;

        [FoldoutGroup("Feedback Settings")]
        [Tooltip("每次循环Play的间隔")]
        public float DelayBetweenLoop = 0f;
        [FoldoutGroup("Feedback Settings")]
        public bool StopAfterDuration = true;

        public float TotalDuration => DelayBeforePlay + GetDuration();
        public float TimeSincePlay { get; private set; }

        public FeedbacksManager Owner { get; private set; }
        public bool IsPlaying { get; private set; }

        protected bool IsInitialized = false;
        private List<Coroutine> _coroutines;
        private int _feedbackPlayCoCount = 0;

        public virtual void Reset()
        {
            OnFeedbackReset();
        }

        public virtual void Play()
        {
            Reset();
            IsPlaying = true;
            TimeSincePlay = Time.time;

            _coroutines.Add(StartCoroutine(FeedbackPlayCo()));
        }

        public virtual void Stop()
        {
            if (!IsPlaying)
                return;
            IsPlaying = false;

            foreach (var co in _coroutines)
            {
                StopCoroutine(co);
            }
            _coroutines.Clear();

            OnFeedbackStop();
        }

        public virtual void Setup(FeedbacksManager owner)
        {
            Owner = owner;
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

            var loop = AmountOfLoop;
            while (loop > 0 && IsPlaying)
            {
                yield return new WaitForSeconds(DelayBeforePlay);
                if (!IsPlaying)
                    yield break;

                OnFeedbackPlay();

                yield return new WaitForSeconds(GetDuration());

                loop--;
            }

            _feedbackPlayCoCount--;
            if (StopAfterDuration && _feedbackPlayCoCount == 0)
            {
                Stop();
            }
        }

        public virtual void OnDrawGizmos() {}

        public virtual void OnDrawGizmosSelected() {}

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

        public virtual void OnValidate() {}
        public virtual void OnInspectorInit() {}
        public virtual void OnInspectorGUI() {}
        public virtual void OnSceneGUI() {}
    }
}
