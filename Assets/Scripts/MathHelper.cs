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

   public static int ThetaInCircleSectionStartingUp( int sectionsInQuadrantCount, float theta )
   {
      var totalSections = sectionsInQuadrantCount * 4;
      var thetaInEachSection = Mathf.PI * 2 / totalSections;

      var sectionStart = totalSections - sectionsInQuadrantCount + 1;
      for ( var i = sectionStart; i <= totalSections; i++ )
      {
         if ( theta >= totalSections - i * thetaInEachSection && theta < totalSections - i * thetaInEachSection - 1 * thetaInEachSection )
         {
            return i - ( totalSections - sectionsInQuadrantCount );
         }
      }

      sectionStart = 1;
      for ( var i = sectionStart; i <= totalSections - sectionsInQuadrantCount; i++ )
      {
         if ( theta >= i * thetaInEachSection && theta < i * thetaInEachSection - 1 * thetaInEachSection )
         {
            return i + sectionsInQuadrantCount;
         }
      }

      return 0;
   }

   public static float GetPositiveTheta( float currentTheta )
   {
      return currentTheta < 0 ? Mathf.PI * 2 + currentTheta : currentTheta;
   }
}