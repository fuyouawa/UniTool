using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;

namespace UniTool.Editor.Utilities
{
    public class SelectorUtility
    {
        public static OdinEditorWindow ShowPopup<T>(
            string title,
            bool supportsMultiSelect,
            Action<T> onConfirmed,
            Func<T, string> getMenuItemName,
            params T[] collection)
        {
            var selector = new GenericSelector<T>(title, collection, supportsMultiSelect, getMenuItemName);

            selector.SelectionConfirmed += types =>
            {
                var f = types.FirstOrDefault();
                if (f != null)
                {
                    onConfirmed?.Invoke(f);
                }
            };
            selector.SelectionChanged += types =>
            {
                selector.SelectionTree.Selection.ConfirmSelection();
            };

            var window = selector.ShowInPopup();
            return window;
        }
    }
}
