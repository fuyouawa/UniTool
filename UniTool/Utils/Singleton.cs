using System;
using System.Reflection;
using UniTool.Internal;
using UnityEngine;

namespace UniTool.Utils
{
    public interface ISingleton
    {
        /// <summary>
        /// 单例初始化(继承当前接口的类都需要实现该方法)
        /// </summary>
        void OnSingletonInit();
    }

    internal class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class
        {
            var type = typeof(T);
            var ctorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            var ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0)
                       ?? throw new Exception($"单例({type})必须要有一个非public的无参构造函数!");

            DebugHelper.AssertCall(() =>
            {
                var publicCtorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                return publicCtorInfos.Length == 0;
            }, $"单例({type})不能有public构造函数!");

            var inst = ctor.Invoke(null) as T;
            DebugHelper.Assert(inst != null);
            return inst;
        }

        public static T CreateMonoSingleton<T>() where T : Component, ISingleton
        {
            if (!Application.isPlaying)
                return null;

            var instances = UnityEngine.Object.FindObjectsOfType<T>();
            if (instances.Length > 1)
            {
                throw new Exception($"Mono单例({nameof(T)})在场景中存在的实例只能有1个!");
            }
            if (instances.Length == 1)
            {
                instances[0].OnSingletonInit();
                return instances[0];
            }

            var obj = new GameObject(nameof(T));
            UnityEngine.Object.DontDestroyOnLoad(obj);
            var inst = obj.AddComponent<T>();

            inst.OnSingletonInit();
            return inst;
        }
    }

    public class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(SingletonCreator.CreateSingleton<T>);

        public static T Instance => _instance.Value;
    }


    public class MonoSingleton<T> : MonoBehaviour, ISingleton
        where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return _instance;
            }
        }

        public virtual void OnSingletonInit()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}