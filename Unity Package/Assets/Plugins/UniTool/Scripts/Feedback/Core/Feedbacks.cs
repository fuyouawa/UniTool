using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Feedbacks
{
    public class Feedbacks : SerializedMonoBehaviour
    {
        public enum InitializationModes { Awake, Start }
        [FoldoutGroup("Settings")]
        [Title("Initialization")]
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
        [Title("Play Settings")]
        [Tooltip("是否可以Play")]
        public bool CanPlay = true;
        [FoldoutGroup("Settings")]
        [Tooltip("在当前Play还没结束时是否可以开始新的Play")]
        public bool CanPlayWhileAlreadyPlaying = true;
        [FoldoutGroup("Settings")]
        [ShowIf("CanPlayWhileAlreadyPlaying")]
        [Tooltip("在当前Play还没结束时, 如果有新的Play, 是否要结束当前Play")]
        public bool StopCurrentPlayIfNewPlay = true;

        [LabelText("Feedbacks")]
        [ValueDropdown("GetAbstractFeedbackDropdown", CopyValues = true)]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "TitleLabel")]
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
        private static Dictionary<string, Type> s_allFeedbackTypes;
        private static ValueDropdownList<AbstractFeedback> s_allFeedbackDropdownItems;

        static Feedbacks()
        {
            s_allFeedbackTypes = (
                from t in AppDomainHelper.GetCustomTypes(AppDomain.CurrentDomain)
                where typeof(AbstractFeedback).IsAssignableFrom(t) && !t.IsAbstract &&
                      t.HasCustomAttribute<AddFeedbackMenuAttribute>()
                select t).ToDictionary(x => x.FullName, y => y);
        }

        public static void GenerateDropdownItem()
        {
            s_allFeedbackDropdownItems = new ValueDropdownList<AbstractFeedback> { { "None", null } };

            foreach (var kv in s_allFeedbackTypes)
            {
                var inst = kv.Value.CreateInstance<AbstractFeedback>();
                Debug.Assert(inst != null, $"创建`{kv.Value.Name}`的实例失败");
                var path = string.Empty;

                if (FeedbacksConfig.Instance.CN)
                {
                    var attr = kv.Value.GetCustomAttribute<AddFeedbackMenuCNAttribute>();
                    if (attr != null)
                    {
                        path = attr.Path;
                    }
                }

                if (path.IsNullOrEmpty())
                {
                    var attr = kv.Value.GetCustomAttribute<AddFeedbackMenuAttribute>();
                    path = attr.Path;
                }

                inst.Label = path.Split('/').Last();
                s_allFeedbackDropdownItems.Add($"{path}", inst);
            }
        }

        private IEnumerable GetAbstractFeedbackDropdown()
        {
            if (s_allFeedbackDropdownItems.IsNullOrEmpty())
            {
                GenerateDropdownItem();
            }
            return s_allFeedbackDropdownItems;
        }
#endif
    }
    public class FeedbacksCoroutineHelper : MonoBehaviour { }
}
