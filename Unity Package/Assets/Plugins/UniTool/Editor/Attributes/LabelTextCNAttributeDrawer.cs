using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using System.Reflection;
using System;
using UniTool.Attributes;
using UniTool.Editor.Helper;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace UniTool.Editor.Attributes
{
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class LabelTextCNAttributeDrawer : OdinAttributeDrawer<LabelTextCNAttribute>
    {
        private ValueResolver<string> textProvider;

        private ValueResolver<Color> iconColorResolver;

        private GUIContent overrideLabel;

        protected override void Initialize()
        {
            //IL_0048: Unknown result type (might be due to invalid IL or missing references)
            //IL_0052: Expected O, but got Unknown
            textProvider = ValueResolver.GetForString(base.Property, base.Attribute.Text);
            iconColorResolver = ValueResolver.Get(base.Property, base.Attribute.IconColor, EditorStyles.label.normal.textColor);
            overrideLabel = new GUIContent();
        }

        /// <summary>
        /// Draws the attribute.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (textProvider.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(textProvider.ErrorMessage);
                CallNextDrawer(label);
                return;
            }
            if (iconColorResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(iconColorResolver.ErrorMessage);
                CallNextDrawer(label);
                return;
            }
            string str = textProvider.GetValue();
            GUIContent useLabel;
            if (str == null && base.Attribute.Icon == SdfIconType.None)
            {
                useLabel = label;
            }
            else
            {
                string lbl = str ?? label.text;
                if (base.Attribute.NicifyText)
                {
                    lbl = ObjectNames.NicifyVariableName(lbl);
                }
                overrideLabel.text = lbl;
                useLabel = overrideLabel;
                if (base.Attribute.Icon != 0)
                {
                    Color iconColor = iconColorResolver.GetValue();
                    useLabel.image = SdfIcons.CreateTransparentIconTexture(base.Attribute.Icon, iconColor, 16, 16, 0);
                }
            }

            CallNextDrawer2(useLabel);
        }


        private bool CallNextDrawer2(GUIContent label)
        {
            OdinDrawer nextDrawer = null;
            BakedDrawerChain chain = Property.GetActiveDrawerChain();
            if (chain.MoveNext())
            {
                nextDrawer = chain.Current;
            }
            if (nextDrawer != null)
            {
                var value = Property.ValueEntry;
                switch (value)
                {
                    case IPropertyValueEntry<string> strEntry:
                    {
                        strEntry.SmartValue = EditorGUILayout.TextField(label, strEntry.SmartValue, EditorStyles.textField);
                        break;
                    }
                    default:
                        return OdinDrawerHelper.CallNextDrawer(this, nextDrawer, label);
                }
                return true;
            }

            return OdinDrawerHelper.CallNextDrawer(this, nextDrawer, label);
        }
    }
}
