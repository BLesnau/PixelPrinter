using System.Xml.Schema;
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

   public static float GetPositiveTheta( float currentTheta )
   {
      return currentTheta < 0 ? Mathf.PI * 2 + currentTheta : currentTheta;
   }
}