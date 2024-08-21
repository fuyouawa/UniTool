using System;
using Sirenix.OdinInspector;
using UniTool.Helper;

namespace MMORPG.Tool
{
    [Serializable]
    public class VisualObject
    {
        public static bool IsAcceptedType(Type type)
        {
            return ReflectHelper.IsVisualType(type);
        }

        [LabelText("@Label")]
        [ShowIf("IsInteger")]
        public int IntegralValue;
        [LabelText("@Label")]
        [ShowIf("IsFloat")]
        public float FloatingPointValue;
        [LabelText("@Label")]
        [ShowIf("IsBool")]
        public bool BooleanValue;
        [LabelText("@Label")]
        [ShowIf("IsString")]
        public string StringValue;
        [LabelText("@Label")]
        [ShowIf("IsUnityObject")]
        public UnityEngine.Object UnityObjectValue;

        public string Label { get; private set; }
        public Type ObjectType { get; private set; }

        public void Setup(Type type, string label)
        {
            Label = label ?? GetDefaultLabel();
            ObjectType = type;
        }

        private bool IsInteger => ReflectHelper.IsIntegerType(ObjectType);
        private bool IsFloat => ReflectHelper.IsFloatType(ObjectType);
        private bool IsBool => ReflectHelper.IsBoolType(ObjectType);
        private bool IsString => ReflectHelper.IsStringType(ObjectType);
        private bool IsUnityObject => ReflectHelper.IsUnityObjectType(ObjectType);

        public object GetRawValue()
        {
            if (IsInteger)
                return IntegralValue;
            if (IsFloat)
                return FloatingPointValue;
            if (IsBool)
                return BooleanValue;
            if (IsString)
                return StringValue;
            if (IsUnityObject)
                return UnityObjectValue;
            return null;
        }

        private string GetDefaultLabel()
        {
            var val = GetRawValue();
            return $"`{val.GetType().GetAliases()}`";
        }
    }
}
