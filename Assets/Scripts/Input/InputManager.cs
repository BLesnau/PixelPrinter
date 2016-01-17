using System;
using System.Linq;
using UnityEngine;

public enum MouseToggleEvent { LeftClick, RightClick }
public enum MouseValueEvent { XAxis, YAxis, Scroll }
public enum TouchToggleEvent { OneFingerTap, TwoFingersTap, OneFingerDown, TwoFingersDown }
public enum TouchValueEvent { OneFingerXAxis, OneFingerYAxis, PinchStretch }

public class ToggleEvent
{
   public MouseToggleEvent MouseEvent;
   public TouchToggleEvent TouchEvent;
}

public class ValueEvent
{
   public float MouseMultiplier = 1;
   public float TouchMultiplier = 1;
   public MouseValueEvent MouseEvent;
   public TouchValueEvent TouchEvent;
}

public class InputManager : MonoBehaviour
{
   public enum MouseButton { Left = 0, Right = 1, Middle = 2 }

   static InputManager()
   {
      Input.simulateMouseWithTouches = false;
   }

   public static bool IsActive( ToggleEvent toggleEvent )
   {
      DebugHelper.Log( "Multi-touch Enabled", Input.multiTouchEnabled.ToString() );
      DebugHelper.Log( "Simulate Mouse With Touch", Input.simulateMouseWithTouches.ToString() );
      DebugHelper.Log( "Touch Supported", Input.touchSupported.ToString() );
      DebugHelper.Log( "Touch Pressure Supported", Input.touchPressureSupported.ToString() );
      DebugHelper.Log( "Touch Count", Input.touchCount.ToString() );
      for ( var i = 0; i < Input.touchCount; i++ )
      {
         DebugHelper.Log( string.Format( "Touch {0} Tap Count", i ), Input.touches[i].tapCount.ToString() );
      }

      if ( toggleEvent == null )
      {
         return false;
      }

      if ( UseTouchControls() )
      {
         switch ( toggleEvent.TouchEvent )
         {
            case TouchToggleEvent.OneFingerTap:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.tapCount == 1 && x.phase == TouchPhase.Ended ) )
               {
                  return true;
               }
               break;
            }
            case TouchToggleEvent.TwoFingersTap:
            {
               if ( Input.touchCount == 2 && Input.touches.All( x => x.tapCount == 1 && x.phase == TouchPhase.Ended ) )
               {
                  return true;
               }
               break;
            }
            case TouchToggleEvent.OneFingerDown:
            {
               if ( Input.touchCount == 1 &&
                    Input.touches.All( x => x.phase != TouchPhase.Ended && x.phase != TouchPhase.Canceled ) )
               {
                  return true;
               }
               break;
            }
            case TouchToggleEvent.TwoFingersDown:
            {
               if ( Input.touchCount == 2 &&
                    Input.touches.All( x => x.phase != TouchPhase.Ended && x.phase != TouchPhase.Canceled ) )
               {
                  return true;
               }
               break;
            }
         }
      }
      else
      {
         switch ( toggleEvent.MouseEvent )
         {
            case MouseToggleEvent.LeftClick:
            {
               if ( IsMouseButtonPressed( MouseButton.Left ) )
               {
                  return true;
               }
               break;
            }
            case MouseToggleEvent.RightClick:
            {
               if ( IsMouseButtonPressed( MouseButton.Right ) )
               {
                  return true;
               }
               break;
            }
         }
      }

      return false;
   }

   public static float GetValue( ValueEvent valueEvent )
   {
      if ( UseTouchControls() )
      {
         switch ( valueEvent.TouchEvent )
         {
            case TouchValueEvent.OneFingerXAxis:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.phase == TouchPhase.Moved ) )
               {
                  return Input.touches[0].deltaPosition.x * valueEvent.TouchMultiplier;
               }
               break;
            }
            case TouchValueEvent.OneFingerYAxis:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.phase == TouchPhase.Moved ) )
               {
                  return Input.touches[0].deltaPosition.y * valueEvent.TouchMultiplier;
               }
               break;
            }
            case TouchValueEvent.PinchStretch:
            {
               if ( Input.touchCount == 2 && Input.touches.Any( x => x.phase == TouchPhase.Moved ) )
               {
                  var t1 = Input.touches[0];
                  var t2 = Input.touches[1];
                  var prevDistance = (float)Math.Sqrt( Math.Pow( ( t1.position.x - t1.deltaPosition.x ) - ( t2.position.x - t2.deltaPosition.x ), 2 ) + Math.Pow( ( t1.position.y - t1.deltaPosition.y ) - ( t2.position.y - t2.deltaPosition.y ), 2 ) );
                  var currDistance = (float)Math.Sqrt( Math.Pow( t1.position.x - t2.position.x, 2 ) + Math.Pow( t1.position.y - t2.position.y, 2 ) );
                  return ( currDistance - prevDistance ) * valueEvent.TouchMultiplier;
               }
               break;
            }
         }
      }
      else
      {
         switch ( valueEvent.MouseEvent )
         {
            case MouseValueEvent.XAxis:
            {
               return Input.GetAxis( "Mouse X" ) * valueEvent.MouseMultiplier;
            }
            case MouseValueEvent.YAxis:
            {
               return Input.GetAxis( "Mouse Y" ) * valueEvent.MouseMultiplier;
            }
            case MouseValueEvent.Scroll:
            {
               return Input.GetAxis( "Mouse ScrollWheel" ) * valueEvent.MouseMultiplier;
            }
         }
      }

      return 0;
   }

   public static bool IsMouseButtonPressed( MouseButton mouseButton )
   {
      return Input.GetMouseButton( Convert.ToInt16( mouseButton ) );
   }

   private static bool UseTouchControls()
   {
      return Input.touchCount != 0;
   }
}
