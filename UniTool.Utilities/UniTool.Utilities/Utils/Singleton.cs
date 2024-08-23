using System;
using System.Reflection;

namespace UniTool.Utilities
{
    public interface ISingleton
    {
        /// <summary>
        /// 单例初始化(继承当前接口的类都需要实现该方法)
        /// </summary>
        void OnSingletonInit();
    }

    internal partial class SingletonCreator
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
    }

    public class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(SingletonCreator.CreateSingleton<T>);

        public static T Instance => _instance.Value;
    }
}