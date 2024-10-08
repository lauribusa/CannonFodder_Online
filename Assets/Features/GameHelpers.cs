using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Features
{
    public static class GameHelpers
    {
        public static float DetectionRange = 3.5f;

        public static List<T> SortByDistance<T>(List<T> objects, Vector3 entityPosition) 
            where T : MonoBehaviour {
        
            var sorted = objects.OrderBy(
               x => Vector3.Distance(entityPosition, x.transform.position)
              ).ToList();
            return sorted;
        }
    }
}
