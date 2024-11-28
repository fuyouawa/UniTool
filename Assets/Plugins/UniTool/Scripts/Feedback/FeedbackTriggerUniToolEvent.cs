// using System;
// using System.Linq;
// using System.Reflection;
// using Sirenix.OdinInspector;
// using UniTool.Utilities;
// using UnityEngine;
//
// namespace UniTool.Tools
// {
//     [FeedbackHelper("使用UniTool的事件系统触发指定事件\n" +
//                     "在FullName中填写事件的类名(完整命名空间), " +
//                     "然后点击旁边的\"F\"(Find)按钮反射查找对应事件类型(可能会卡一下), " +
//                     "如果找到下方就会出现事件结构让你填写")]
//     [AddFeedbackMenu("事件/触发UniTool事件")]
//     public class FeedbackTriggerUniToolEvent : AbstractFeedback
//     {
//         [FoldoutGroup("UniTool Event")]
//         [InlineButton("FindEventObject", " F ")]
//         public string FullName;
//
//         [ShowIf("@EventObject != null")]
//         public object EventObject;
//
//         private MethodInfo _trigger;
//
//         protected override void OnFeedbackInit()
//         {
//             _trigger = typeof(EventManager).GetMethod(nameof(EventManager.TriggerEvent),
//                 BindingFlags.Public | BindingFlags.Instance);
//             Debug.Assert(_trigger != null);
//         }
//
//         protected override void OnFeedbackPlay()
//         {
//             var m = _trigger.MakeGenericMethod(EventObject.GetType());
//             m.Invoke(null, new[] { EventObject });
//         }
//
//         protected override void OnFeedbackStop()
//         {
//         }
//
// #if UNITY_EDITOR
//         private void FindEventObject()
//         {
//             var eventType = AppDomainHelper.GetCustomTypes(AppDomain.CurrentDomain)
//                 .FirstOrDefault(t => t.FullName == FullName);
//             if (eventType == null)
//             {
//                 UnityEditor.EditorUtility.DisplayDialog("错误", $"找不到事件类型({FullName})", "确定");
//                 return;
//             }
//
//             EventObject = eventType.CreateInstance();
//         }
// #endif
//     }
// }
