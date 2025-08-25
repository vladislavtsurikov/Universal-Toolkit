using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    public static class EventCallbacksUtility
    {
        public static void SetupEventCallbacksForce(GameObject gameObject, Event triggerEvent)
        {
            if (triggerEvent == null)
            {
                return;
            }
        
            if (SetupEventCallbacksIfPossible(gameObject, triggerEvent))
            {
                return;
            }
        
            if (CreateEventCallbacksIfPossible(gameObject, triggerEvent, out EventCallbacks eventCallbacks))
            {
                eventCallbacks.Setup(triggerEvent);
            }
        }
        
        public static bool CreateEventCallbacksIfPossible(GameObject gameObject, Event triggerEvent, out EventCallbacks eventCallbacks)
        {
            if (triggerEvent == null)
            {
                eventCallbacks = null;
                return false;
            }
            
            if (GetEventCallbackType(triggerEvent, out var callbackType))
            {
                eventCallbacks = null;
                return false;
            }

            MonoBehaviour existingCallback = gameObject.GetComponent(callbackType) as MonoBehaviour;

            if (existingCallback == null)
            {
                eventCallbacks = gameObject.AddComponent(callbackType) as EventCallbacks;
                return true;
            }
            
            eventCallbacks = null;
            return false;
        }

        public static bool SetupEventCallbacksIfPossible(GameObject gameObject, Event triggerEvent)
        {
            if (GetEventCallbackType(triggerEvent, out var callbackType))
            {
                return false;
            }
            
            EventCallbacks eventCallbacks = gameObject.GetComponent(callbackType) as EventCallbacks;

            if (eventCallbacks != null)
            {
                eventCallbacks.Setup(triggerEvent);
                return true;
            }

            return false;
        }

        public static bool GetEventCallbackType(Event triggerEvent, out Type callbackType)
        {
            EventCallbacksTypeAttribute eventCallbacksTypeAttribute = triggerEvent.GetType().GetAttribute<EventCallbacksTypeAttribute>();

            if (eventCallbacksTypeAttribute == null)
            {
                callbackType = null;
                return true;
            }

            callbackType = eventCallbacksTypeAttribute.Type;
            return false;
        }
    }
}