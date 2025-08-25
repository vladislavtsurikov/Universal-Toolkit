using VladislavTsurikov.ReflectionUtility;
using UnityEngine.EventSystems;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.UnityUI
{
    [Name("Unity UI/On Select")]
    public class OnSelectPointerEvent : UISelectEvent
    {
        protected internal override void OnSelect(BaseEventData eventData)
        {
            Trigger.Run();
        }
    }
}