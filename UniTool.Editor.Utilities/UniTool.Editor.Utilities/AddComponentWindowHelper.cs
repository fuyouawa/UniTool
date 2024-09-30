using System;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public static class AddComponentWindowHelper
    {
        private static Type _typeOfAddComponentWindow;

        public static Type TypeOfAddComponentWindow
        {
            get
            {
                if (_typeOfAddComponentWindow == null)
                {
                    _typeOfAddComponentWindow = Type.GetType("UnityEditor.AddComponent.AddComponentWindow, UnityEditor");
                }
                return _typeOfAddComponentWindow;
            }
        }
           

        public static void Show(Rect rect, GameObject[] gos)
        {
            TypeOfAddComponentWindow.InvokeMethod("Show", null, rect, gos);
        }
    }
}