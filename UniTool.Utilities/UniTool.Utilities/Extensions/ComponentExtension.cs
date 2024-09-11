using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTool.Utilities
{
    public static class ComponentExtension
    {
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
    }
}