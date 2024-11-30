using UnityEngine;

namespace UniTool.Utilities
{
    public static class ComponentExtension
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }
    }
}
