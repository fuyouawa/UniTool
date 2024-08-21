using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniTool.Helper;
using UnityEngine;

namespace UniTool.Event
{
    // public interface IEventListenerBase
    // {
    // }
    //
    // public interface IEventListener<in TEvent> : IEventListenerBase
    // {
    //     void OnEvent(TEvent e);
    // }

    // public class EventManager
    // {
    //
    //     private static Dictionary<Type, List<IEventListenerBase>> _subscribersList;
    //
    //     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    //     static void InitializeStatics()
    //     {
    //         _subscribersList = new Dictionary<Type, List<IEventListenerBase>>();
    //     }
    //
    //     static EventManager()
    //     {
    //         _subscribersList = new Dictionary<Type, List<IEventListenerBase>>();
    //     }
    //
    //     public static void AddListener<TEvent>(IEventListener<TEvent> listener)
    //     {
    //         var eventType = typeof(TEvent);
    //
    //         if (!_subscribersList.ContainsKey(eventType))
    //         {
    //             _subscribersList[eventType] = new List<IEventListenerBase>();
    //         }
    //
    //         if (!SubscriptionExists(eventType, listener))
    //         {
    //             _subscribersList[eventType].Add(listener);
    //         }
    //     }
    //
    //     public static void RemoveListener<TEvent>(IEventListener<TEvent> listener)
    //     {
    //         var eventType = typeof(TEvent);
    //
    //         if (UniToolConfig.DebugMode)
    //         {
    //             if (!_subscribersList.ContainsKey(eventType))
    //             {
    //                 Debug.LogWarning(($"删除没有注册过事件({eventType})的监听器({listener})!"));
    //             }
    //         }
    //
    //         var subscriberList = _subscribersList[eventType];
    //
    //         bool listenerFound = false;
    //
    //         for (int i = subscriberList.Count - 1; i >= 0; i--)
    //         {
    //             if (subscriberList[i] == listener)
    //             {
    //                 subscriberList.RemoveAt(i);
    //                 listenerFound = true;
    //
    //                 if (subscriberList.Count == 0)
    //                 {
    //                     _subscribersList.Remove(eventType);
    //                 }
    //
    //                 return;
    //             }
    //         }
    //
    //         if (UniToolConfig.DebugMode)
    //         {
    //             if (!listenerFound)
    //             {
    //                 Debug.LogWarning($"删除一个没有注册过的监听器({listener}), 事件是:{eventType}");
    //             }
    //         }
    //     }
    //
    //
    //     public static void TriggerEvent<TEvent>(TEvent arg)
    //     {
    //         var eventType = typeof(TEvent);
    //         if (_subscribersList.TryGetValue(eventType, out var list))
    //         {
    //             for (int i = list.Count - 1; i >= 0; i--)
    //             {
    //                 ((IEventListener<TEvent>)list[i]).OnEvent(arg);
    //             }
    //         }
    //     }
    //
    //
    //     private static bool SubscriptionExists(Type type, IEventListenerBase receiver)
    //     {
    //         if (!_subscribersList.TryGetValue(type, out var receivers)) return false;
    //
    //         bool exists = false;
    //
    //         for (int i = receivers.Count - 1; i >= 0; i--)
    //         {
    //             if (receivers[i] == receiver)
    //             {
    //                 exists = true;
    //                 break;
    //             }
    //         }
    //
    //         return exists;
    //     }
    // }

    public class EventHandlerAttribute : Attribute
    {

    }

    public class EventManager
    {
        private static Dictionary<Type, Delegate> _handlers;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeStatics()
        {
            _handlers = new Dictionary<Type, Delegate>();
        }
    
        static EventManager()
        {
            _handlers = new Dictionary<Type, Delegate>();
        }

        private static Action<TEvent> GetHandler<TEvent>()
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = null;
            }

            return _handlers[eventType] as Action<TEvent>;
        }

        public static IUnRegister RegisterSubscriber<T>(T obj)
        {
            var type = typeof(T);
            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var handlers = type.GetMethods(bf);
            // 排除基类
            if (type.BaseType != null)
            {
                handlers = handlers.Except(type.BaseType.GetMethods(bf)).ToArray();
            }

            handlers = handlers.Where(h => h.GetCustomAttribute<EventHandlerAttribute>() != null).ToArray();

            Action unregister = null;
            foreach (var h in handlers)
            {
                var p = h.GetParameters();
                if (p.Length != 1)
                {
                    throw new ArgumentException($"事件处理器({ReflectHelper.GetSignature(h)})的参数数量必须是1!");
                }

                var et = p[0].ParameterType;
                var func = h.CreateDelegate(obj);

                AddTypeListener(et, func);
                unregister += () => RemoveTypeListener(et, func);
            }

            return new CustomUnRegister(unregister);
        }

        private static MethodInfo _addListener;
        private static void AddTypeListener(Type eventType, Delegate handler)
        {
            if (_addListener == null)
            {
                _addListener = typeof(EventManager).GetMethod("AddListener", BindingFlags.Static | BindingFlags.NonPublic);
            }
            System.Diagnostics.Debug.Assert(_addListener != null);

            var m = _addListener.MakeGenericMethod(eventType);
            m.Invoke(null, new object[] { handler });
        }

        private static void AddListener<TEvent>(Action<TEvent> handler)
        {
            _handlers[typeof(TEvent)] = GetHandler<TEvent>() + handler;
        }


        private static MethodInfo _removeListener;
        private static void RemoveTypeListener(Type eventType, Delegate handler)
        {
            if (_removeListener == null)
            {
                _removeListener = typeof(EventManager).GetMethod("RemoveListener", BindingFlags.Static | BindingFlags.NonPublic);
            }
            System.Diagnostics.Debug.Assert(_removeListener != null);

            var m = _removeListener.MakeGenericMethod(eventType);
            m.Invoke(null, new object[] { handler });
        }

        private static void RemoveListener<TEvent>(Action<TEvent> handler)
        {
            _handlers[typeof(TEvent)] = GetHandler<TEvent>() - handler;
        }
    
        public static void TriggerEvent<TEvent>(TEvent arg)
        {
            GetHandler<TEvent>()?.Invoke(arg);
        }
    }

    public interface IUnRegister
    {
        void UnRegister();
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action _onUnRegister { get; set; }
        public CustomUnRegister(Action onUnRegister) => _onUnRegister = onUnRegister;

        public void UnRegister()
        {
            _onUnRegister.Invoke();
            _onUnRegister = null;
        }
    }

    public abstract class UnRegisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnRegister> _unRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister) => _unRegisters.Add(unRegister);

        public void RemoveUnRegister(IUnRegister unRegister) => _unRegisters.Remove(unRegister);

        public void UnRegister()
        {
            foreach (var unRegister in _unRegisters)
            {
                unRegister.UnRegister();
            }

            _unRegisters.Clear();
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDisable()
        {
            UnRegister();
        }
    }

    public static class UnRegisterExtension
    {
        public static IUnRegister UnRegisterWhenDestroyed(
            this IUnRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDestroyTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }

        public static IUnRegister UnRegisterWhenDisabled(
            this IUnRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDisableTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }
    }

    // public static class EventRegister
    // {
    //     private static readonly MethodInfo _addListenerMethod;
    //     private static readonly MethodInfo _removeListenerMethod;
    //
    //     static EventRegister()
    //     {
    //         _addListenerMethod =
    //             typeof(EventManager).GetMethod("AddListener", BindingFlags.Public | BindingFlags.Static);
    //         _removeListenerMethod =
    //             typeof(EventManager).GetMethod("RemoveListener", BindingFlags.Public | BindingFlags.Static);
    //     }
    //
    //     public static IUnRegister EventRegisterListener<TEvent>(this IEventListener<TEvent> caller)
    //     {
    //         EventManager.AddListener(caller);
    //         return new CustomUnRegister(() => EventManager.RemoveListener(caller));
    //     }
    //     
    //     public static void EventUnRegisterListener<TEvent>(this IEventListener<TEvent> caller)
    //     {
    //         EventManager.RemoveListener(caller);
    //     }
    //     
    //     public static IUnRegister EventAutoRegisterListeners<T>(this T obj)
    //     {
    //         var type = typeof(T);
    //         var interfaces = type.GetInterfaces();
    //         // 排除基类
    //         if (type.BaseType != null)
    //         {
    //             interfaces = interfaces.Except(type.BaseType.GetInterfaces()).ToArray();
    //         }
    //     
    //         interfaces = interfaces.Where(t =>
    //                 t.IsInherit<IEventListenerBase>() &&
    //                 t.IsGenericType &&
    //                 t.GetGenericTypeDefinition() == typeof(IEventListener<>))
    //             .ToArray();
    //     
    //         Action unregister = null;
    //         foreach (var i in interfaces)
    //         {
    //             var e = i.GetGenericArguments()[0];
    //             var m = _addListenerMethod.MakeGenericMethod(e);
    //             m.Invoke(obj, new object[]{ obj });
    //     
    //             unregister += () =>
    //             {
    //                 var m2 = _removeListenerMethod.MakeGenericMethod(e);
    //                 m2.Invoke(obj, new object[]{ obj });
    //             };
    //         }
    //     
    //         return new CustomUnRegister(unregister);
    //     }
    // }
}