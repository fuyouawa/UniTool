using UnityEngine;

namespace UniTool.Extension
{
    public static class LayerExtension
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
        public static bool Contains(this LayerMask mask, string layer)
        {
            return mask.Contains(LayerMask.NameToLayer(layer));
        }
    }
}