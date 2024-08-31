using System.Linq;
using UnityEngine;

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

        public static bool HasParam(this Animator animator, string name)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name) != null;
        }

        public static bool HasParam(this Animator animator, string name, AnimatorControllerParameterType typeCheck)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name && x.type == typeCheck) != null;
        }

        public static Sprite ToSprite(this Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        public static void PlayWithChildren(this AudioSource audio)
        {
            foreach (var a in audio.GetComponentsInChildren<AudioSource>())
            {
                a.Play();
            }
        }
    }
}