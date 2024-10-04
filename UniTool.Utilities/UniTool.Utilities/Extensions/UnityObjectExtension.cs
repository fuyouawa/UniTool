using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniTool.Utilities
{
    public static class UnityObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent(out T component))
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }

        public static void DestroyAfterSeconds(this Object go, float seconds)
        {
            UnityInvoker.Invoke(DestroyAfterSecondsCo(go, seconds));
        }

        public static bool HasComponent<T>(this GameObject go)
        where T : Component
        {
            return go.TryGetComponent<T>(out _);
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