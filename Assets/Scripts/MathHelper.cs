using UnityEngine;

public static class MathHelper
{
   public static Vector2 FindPoint( Vector2 origin, Vector2 end, float dist )
   {
      var mid = end - origin;
      mid.Normalize();
      mid = mid * dist;
      return mid;
   }
}