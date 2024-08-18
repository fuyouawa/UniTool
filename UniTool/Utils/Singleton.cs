using System;
using System.Reflection;
using UniTool.Config;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace UniTool.Utils
{
    internal class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class
        {
            var type = typeof(T);
            var ctorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            var ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0)
                       ?? throw new Exception($"单例({type})必须要有一个非public的无参构造函数!");
            if (UniToolConfig.DebugMode)
            {
                var publicCtorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                if (publicCtorInfos.Length != 0)
                    throw new Exception($"单例({type})不能有public构造函数!");
            }
            var inst = ctor.Invoke(null) as T;
            System.Diagnostics.Debug.Assert(inst != null);
            return inst;
        }
    }

    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(SingletonCreator.CreateSingleton<T>);

        public static T Instance => _instance.Value;
    }


    public abstract class MonoSingleton<T> : MonoBehaviour
        where T : MonoSingleton<T>, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject(nameof(T));
                    SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
                    _instance = obj.AddComponent<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this as T;
        }
    }
}