using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UniTool.Utilities
{
    public abstract class MemberPicker
    {
        [HideLabel]
        [HorizontalGroup("Picker")]
        public GameObject TargetObject;

        [HideLabel]
        [HorizontalGroup("Picker")]
        [ValueDropdown("GetTargetComponentNamesDropdown")]
        [OnValueChanged("OnTargetComponentNameChanged")]
        public string TargetComponentName;

        [HideLabel]
        [ValueDropdown("GetComponentMemberNamesDropdown")]
        [OnValueChanged("OnTargetMemberNameChanged")]
        public string TargetMemberName = string.Empty;

        private Component _targetComponent;

        public Component GetTargetComponent()
        {
            if (_targetComponent != null)
                return _targetComponent;

            if (string.IsNullOrWhiteSpace(TargetComponentName) || TargetObject == null)
                return null;
            _targetComponent = TargetObject.GetComponents<Component>()
                .FirstOrDefault(c => c.GetType().Name == TargetComponentName);
            return _targetComponent;
        }

        private MemberInfo _targetMember;

        public MemberInfo GetTargetMember()
        {
            if (_targetMember != null)
                return _targetMember;

            var c = GetTargetComponent();
            if (c == null)
                return null;
            _targetMember = c.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => ReflectionUtility.GetSignature(m) == TargetMemberName);

            return _targetMember;
        }

        // public override string ToString()
        // {
        //     if (TargetComponent == null || string.IsNullOrEmpty(TargetMemberName))
        //         return "None Member";
        //     var member = TargetMember;
        //     if (TargetMember == null)
        //     {
        //         if (!TryGetMember(out member))
        //         {
        //             return "None Member";
        //         }
        //     }
        //     return $"{TargetComponent.GetType().Name}.{member.Name}[{member.MemberType}]";
        // }
#if UNITY_EDITOR
        protected virtual void OnTargetComponentNameChanged()
        {
            _targetComponent = null;
            OnTargetMemberNameChanged();
        }

        protected virtual void OnTargetMemberNameChanged()
        {
            _targetMember = null;
        }

        private IEnumerable GetTargetComponentNamesDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (TargetObject == null)
                return total;
            total.AddRange(
                from c in TargetObject.GetComponents<Component>()
                let n = c.GetType().Name
                select new ValueDropdownItem<string>(n, n));
            return total;
        }

        private IEnumerable GetComponentMemberNamesDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (string.IsNullOrWhiteSpace(TargetComponentName))
                return total;

            var c = GetTargetComponent();
            if (c == null)
                return total;

            total.AddRange(c.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(MemberFilter)
                .Select(m => new ValueDropdownItem<string>(
                    GetMemberValueDropdownName(m),
                    ReflectionUtility.GetSignature(m))));
            return total;
        }

        protected virtual bool MemberFilter(MemberInfo member)
        {
            return true;
        }

        protected virtual string GetMemberValueDropdownName(MemberInfo member)
        {
            var name = $"{member.MemberType}/{member.Name}";
            if (member is MethodInfo m)
            {
                name += $"({ReflectionUtility.GetMethodParametersSignature(m)})";
            }
            return name;
        }
#endif
    }
}
