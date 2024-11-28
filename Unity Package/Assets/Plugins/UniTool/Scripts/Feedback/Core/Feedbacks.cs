using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UniTool.Attributes;
using UniTool.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UniTool.Editor.Utilities;
#endif

namespace UniTool.Tools
{
    public class Feedbacks : SerializedMonoBehaviour
    {
        public enum InitializationModes
        {
            Awake,
            Start
        }

        [FoldoutGroup("Settings")]
        [TitleCN("Initialization")]
        public InitializationModes InitializationMode = InitializationModes.Awake;

        [FoldoutGroup("Settings")]
        [Tooltip("确保Play前所有Feedbacks都初始化")]
        public bool AutoInitialization = true;

        [FoldoutGroup("Settings")]
        [Tooltip("在Start时自动Play一次")]
        public bool AutoPlayOnStart;

        [FoldoutGroup("Settings")]
        [Tooltip("在OnEnable时自动Play一次")]
        public bool AutoPlayOnEnable;

        [FoldoutGroup("Settings")]
        [TitleCN("Play Settings")]
        [Tooltip("是否可以Play")]
        public bool CanPlay = true;

        [FoldoutGroup("Settings")]
        [Tooltip("在当前Play还没结束时是否可以开始新的Play")]
        public bool CanPlayWhileAlreadyPlaying = true;

        [FoldoutGroup("Settings")]
        [ShowIf(nameof(CanPlayWhileAlreadyPlaying))]
        [Tooltip("在当前Play还没结束时, 如果有新的Play, 是否要结束当前Play")]
        public bool StopCurrentPlayIfNewPlay = true;

        [LabelText("Feedbacks")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(CustomAddFunction = "OnAddFeedback")]
        public List<AbstractFeedback> FeedbackList = new List<AbstractFeedback>();

        public bool IsInitialized { get; private set; }

        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }

        public float TimeSinceLastPlay { get; private set; }

        private List<Coroutine> _coroutines;

        public bool HasFeedbackPlaying()
        {
            foreach (var feedback in FeedbackList)
            {
                if (feedback.Enable && feedback.IsPlaying)
                {
                    return true;
                }
            }

            return false;
        }

        private void Awake()
        {
            if (!TryGetComponent(out FeedbacksCoroutineHelper coroutineHelper))
            {
                coroutineHelper = gameObject.AddComponent<FeedbacksCoroutineHelper>();
            }

            _coroutines = new List<Coroutine>();
            CoroutineHelper = coroutineHelper;
            if (InitializationMode == InitializationModes.Awake)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (InitializationMode == InitializationModes.Start)
            {
                Initialize();
            }

            if (AutoPlayOnStart)
            {
                Play();
            }
        }

        private void Update()
        {
        }

        private void OnEnable()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnEnable();
            }

            if (AutoPlayOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            foreach (var item in FeedbackList)
            {
                item.OnDisable();
            }
        }

        public void Play()
        {
            if (!IsInitialized && AutoInitialization)
            {
                Initialize();
            }

            if (!CanPlay) return;

            if (HasFeedbackPlaying())
            {
                if (!CanPlayWhileAlreadyPlaying)
                {
                    if (StopCurrentPlayIfNewPlay)
                    {
                        Stop();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            _coroutines.Add(StartCoroutine(PlayFeedbacksCo()));
        }

        private IEnumerator PlayFeedbacksCo()
        {
            TimeSinceLastPlay = Time.time;

            foreach (var feedback in FeedbackList)
            {
                if (feedback.Enable)
                {
                    yield return feedback.PlayCo();
                }
            }
        }

        public void Stop()
        {
            if (!IsInitialized)
            {
                return;
            }

            foreach (var co in _coroutines)
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }
            }

            _coroutines.Clear();
            foreach (var feedback in FeedbackList)
            {
                feedback.Stop();
            }
        }

        private void OnDestroy()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnDestroy();
            }
        }

        private void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            foreach (var feedback in FeedbackList)
            {
                feedback.Setup(this);
                feedback.Initialize();
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnDrawGizmosSelected();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnDrawGizmos();
            }
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.Setup(this);
                feedback.OnInspectorInit();
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnInspectorGUI();
            }
        }

        [ButtonGroup]
        [DisableIf("@IsInitialized || !UnityEditor.EditorApplication.isPlaying")]
        private void TestInit()
        {
            Initialize();
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

        public void OnSceneGUI()
        {
            foreach (var item in FeedbackList)
            {
                item.OnSceneGUI();
            }
        }

        private static Type[] s_allFeedbackTypes;

        static Feedbacks()
        {
            s_allFeedbackTypes = (
                from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(AbstractFeedback).IsAssignableFrom(t) && !t.IsAbstract &&
                      t.HasCustomAttribute<AddFeedbackMenuAttribute>()
                select t).ToArray();
        }

        private void OnAddFeedback()
        {
            SelectorUtility.ShowPopup("", false, t => FeedbackList.Add(t.CreateInstance<AbstractFeedback>()),
                t => t.GetCustomAttribute<AddFeedbackMenuAttribute>().Path,
                s_allFeedbackTypes);
        }
#endif
    }

    public class FeedbacksCoroutineHelper : MonoBehaviour
    {
    }
}
