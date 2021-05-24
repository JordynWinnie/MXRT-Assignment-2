using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class Vector2Extension
    {
        public static bool IsPointOverUIObject(this Vector2 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            var eventPos = new PointerEventData(EventSystem.current);
            eventPos.position = new Vector2(pos.x, pos.y);

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventPos, results);

            return results.Count > 0;
        }
    }
}