using UnityEngine;

namespace UniTool.Utils
{
    public static class LayerHelper
    {
        public static LayerMask FromLayers(params int[] layers)
        {
            int mask = 0;
            foreach (var layer in layers)
            {
                mask |= (1 << layer);
            }
            return mask;
        }
        public static LayerMask FromLayers(params string[] layers)
        {
            int mask = 0;
            foreach (var layer in layers)
            {
                mask |= (1 << LayerMask.NameToLayer(layer));
            }
            return mask;
        }
    }
}