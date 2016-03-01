using UnityEngine;
using System.Collections.Generic;

public class DebugHelper : MonoBehaviour
{
   public bool ShowDebug = false;

   private static Dictionary<string, string> _debugInfo;

   private void Start()
   {
   }

   private void Update()
   {

   }

   public DebugHelper()
   {
      _debugInfo = new Dictionary<string, string>();
   }

   private void OnGUI()
   {
      if ( !ShowDebug )
      {
         return;
      }

      var rect = new Rect( 5, 5, 500, 25 );
      foreach ( var info in _debugInfo )
      {
         GUI.Label( rect, info.Key + " = " + info.Value );
         rect.y += 25;
      }
   }

   public static void Log( string name, string value )
   {
      _debugInfo[name] = value;
   }

   public static void Unlog( string name )
   {
      _debugInfo.Remove( name );
   }
}
