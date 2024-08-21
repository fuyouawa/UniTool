using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UniTool.Helper;

namespace MMORPG.Tool
{
    [Serializable]
    public class ValueGetter : MemberPicker
    {
        public object GetRawValue()
        {
            var c = GetTargetComponent();
            var m = GetTargetMember();
            switch (m)
            {
                case FieldInfo field:
                    return field.GetValue(c);
                case PropertyInfo property:
                    return property.GetValue(c);
                case MethodInfo method:
                    return method.Invoke(c, null);
                default:
                    throw new NotSupportedException();
            }
        }

        protected override bool MemberFilter(MemberInfo member)
        {
            if (!ReflectHelper.TryGetValueType(member, out var type))
                return false;
            if (type == typeof(void))
                return false;
            if (member is MethodInfo method)
                return method.GetParameters().Length == 0 && !method.IsSpecialName;
            return true;
        }
    }

    [Serializable]
    public class ValueGetter<TReturn> : ValueGetter
    {
        public new TReturn GetRawValue()
        {
            return (TReturn)base.GetRawValue();
        }

        protected override bool MemberFilter(MemberInfo member)
        {
            if (base.MemberFilter(member) && ReflectHelper.TryGetValueType(member, out var type))
            {
                return type == typeof(TReturn);
            }
            return false;
        }
    }
}
