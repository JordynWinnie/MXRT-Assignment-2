using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
/// <summary>
/// This is an Extension class for the Built-in Unity Vector2
/// This code is a modified version of the code provided in this video:
///
///  (1) Unity3d AR Foundation - How To Block AR Raycast From UI Touch Events? (For ARCore And ARKIT)
///  https://www.youtube.com/watch?v=NdrvihZhVqs
/// </summary>


//Encapsulate these extensions to their own Namespace:
namespace Vector2Extensions
{
    //Create a static class instance that can be accessed anywhere,
    
    public static class Vector2Extension
    {
        //Create an Extension by using the this keyword along with the type,
        //which allows this to extend Vector2,
        //this will return True if the pointer is over a UI object:
        public static bool IsPointOverUIObject(this Vector2 pos)
        {
            //Use the current event system to get UI/gameobject click data
            //through the position supplied:
            var eventPos = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(pos.x, pos.y)
            };

            //Call a RaycastAll on the supplied position,
            //which will return all the objects hit by the ray
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventPos, results);

            //If there are more than 1 objects hit, it means that a UI element +
            //real world game location has been hit, so the Pointer is currently over
            //a UI element:
            return results.Count > 1;
        }
    }
}