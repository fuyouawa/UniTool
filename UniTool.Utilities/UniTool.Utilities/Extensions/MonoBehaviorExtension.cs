using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTool.Utilities
{
    public static class MonoBehaviorExtension
    {
        public static void CallInNextFrame(this MonoBehaviour mono, Action callback)
        {
            mono.StartCoroutine(CallInNextFrameCo(callback));
        }


        private static IEnumerator CallInNextFrameCo(Action callback)
        {
            yield return null;
            callback();
        }
    }
}