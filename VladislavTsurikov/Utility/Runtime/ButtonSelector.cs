using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace QuestsSystem.IntegrationActionFlow.Pointer
{
    public class ButtonSelector : FieldSelector
    {
        public override bool IsValidFieldType(Type fieldType)
        {
            return fieldType == typeof(Button);
        }
        
        public Button ResolveButton(DiContainer diContainer)
        {
            if (DeclaringType == null)
            {
                Debug.LogError("ButtonSelector: DeclaringType is not set.");
                return null;
            }

            object view = diContainer.Resolve(DeclaringType);
            return ResolveButtonFromView(view);
        }

        public Button ResolveButtonFromView(object view)
        {
            if (view == null)
            {
                Debug.LogError($"ButtonSelector: Unable to resolve {DeclaringType}.");
                return null;
            }

            Button button = GetButton(view);
            if (button == null)
            {
                Debug.LogError("ButtonSelector: Button not found in the target View.");
            }

            return button;
        }

        private Button GetButton(object target)
        {
            return GetFieldValue(target) as Button;
        }
    }
}