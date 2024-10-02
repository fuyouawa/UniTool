using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniTool.Utilities
{
    public static class UnityExtension
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent(out T component))
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }

        public static void CallInNextFrame(this MonoBehaviour mono, Action callback)
        {
            mono.StartCoroutine(CallInNextFrameCo(callback));
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        private static IEnumerator CallInNextFrameCo(Action callback)
        {
            yield return null;
            callback();
        }

        public static void DestroyAfterSeconds(this Object go, float seconds)
        {
            UnityInvoker.Invoke(DestroyAfterSecondsCo(go, seconds));
        }

        private static IEnumerator DestroyAfterSecondsCo(Object go, float seconds)
        {
            if (seconds > 0)
            {
                yield return new WaitForSeconds(seconds);
            }
            Object.Destroy(go);
        }
    }
}