using Sirenix.OdinInspector;
using System.Diagnostics;
using System;
using UnityEngine;

namespace UniTool.Attributes
{
    /// <summary>
    /// 扩展自Sirenix.OdinInspector.LabelTextAttribute
    /// 优化中文字体的显示
    /// 使用方法和Sirenix.OdinInspector.LabelTextAttribute一样
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class LabelTextCNAttribute : PropertyAttribute
    {   /// <summary>
        /// The new text of the label.
        /// </summary>
        public string Text;

        /// <summary>
        /// Whether the label text should be nicified before it is displayed, IE, "m_someField" becomes "Some Field".
        /// If the label text is resolved via a member reference, an expression, or the like, then the evaluated result
        /// of that member reference or expression will be nicified.
        /// </summary>
        public bool NicifyText;

        /// <summary>
        /// The icon to be displayed.
        /// </summary>
        public SdfIconType Icon;

        /// <summary> Supports a variety of color formats, including named colors (e.g. "red", "orange", "green", "blue"), hex codes (e.g. "#FF0000" and "#FF0000FF"), and RGBA (e.g. "RGBA(1,1,1,1)") or RGB (e.g. "RGB(1,1,1)"), including Odin attribute expressions (e.g "@this.MyColor"). Here are the available named colors: black, blue, clear, cyan, gray, green, grey, magenta, orange, purple, red, transparent, transparentBlack, transparentWhite, white, yellow, lightblue, lightcyan, lightgray, lightgreen, lightgrey, lightmagenta, lightorange, lightpurple, lightred, lightyellow, darkblue, darkcyan, darkgray, darkgreen, darkgrey, darkmagenta, darkorange, darkpurple, darkred, darkyellow. </summary>
        public string IconColor;

        /// <summary>
        /// Give a property a custom label.
        /// </summary>
        /// <param name="text">The new text of the label.</param>
        public LabelTextCNAttribute(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Give a property a custom icon.
        /// </summary>
        /// <param name="icon">The icon to be shown next to the property.</param>
        public LabelTextCNAttribute(SdfIconType icon)
        {
            Icon = icon;
        }

        /// <summary>
        /// Give a property a custom label.
        /// </summary>
        /// <param name="text">The new text of the label.</param>
        /// <param name="nicifyText">Whether to nicify the label text.</param>
        public LabelTextCNAttribute(string text, bool nicifyText)
        {
            Text = text;
            NicifyText = nicifyText;
        }

        /// <summary>
        /// Give a property a custom label with a custom icon.
        /// </summary>
        /// <param name="text">The new text of the label.</param>
        /// <param name="icon">The icon to be displayed.</param>
        public LabelTextCNAttribute(string text, SdfIconType icon)
        {
            Text = text;
            Icon = icon;
        }

        /// <summary>
        /// Give a property a custom label with a custom icon.
        /// </summary>
        /// <param name="text">The new text of the label.</param>
        /// <param name="nicifyText">Whether to nicify the label text.</param>
        /// <param name="icon">The icon to be displayed.</param>
        public LabelTextCNAttribute(string text, bool nicifyText, SdfIconType icon)
        {
            Text = text;
            NicifyText = nicifyText;
            Icon = icon;
        }
    }
}
