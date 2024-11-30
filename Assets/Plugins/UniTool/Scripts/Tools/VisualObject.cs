using System;
using Sirenix.OdinInspector;
using UniTool.Utilities;

namespace UniTool.Tools
{
    [Serializable]
    public class VisualObject
    {
        public static bool IsAcceptedType(Type type)
        {
            return type.IsVisualType();
        }

        [LabelText("@Label")]
        [ShowIf(nameof(IsInteger))]
        public int IntegralValue;
        [LabelText("@Label")]
        [ShowIf(nameof(IsFloat))]
        public float FloatingPointValue;
        [LabelText("@Label")]
        [ShowIf(nameof(IsBool))]
        public bool BooleanValue;
        [LabelText("@Label")]
        [ShowIf(nameof(IsString))]
        public string StringValue;
        [LabelText("@Label")]
        [ShowIf(nameof(IsUnityObject))]
        public UnityEngine.Object UnityObjectValue;

        public string Label { get; private set; }
        public Type ObjectType { get; private set; }

        public void Setup(Type type, string label)
        {
            Label = label ?? GetDefaultLabel();
            ObjectType = type;
        }

        private bool IsInteger => ObjectType.IsIntegerType();
        private bool IsFloat => ObjectType.IsFloatType();
        private bool IsBool => ObjectType.IsBoolType();
        private bool IsString => ObjectType.IsStringType();
        private bool IsUnityObject => ObjectType.IsUnityObjectType();

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
