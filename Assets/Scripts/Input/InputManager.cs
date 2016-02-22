using System;
using System.Linq;
using UnityEngine;

public enum MouseToggleEvent { LeftClick, RightClick, LeftDown, RightDown }
public enum MouseValueEvent { XAxis, YAxis, XPos, YPos, Scroll }
public enum TouchToggleEvent { OneFingerTap, OneFingerDown, TwoFingersDown }
public enum TouchValueEvent { OneFingerXAxis, OneFingerYAxis, OneFingerXPos, OneFingerYPos, PinchStretch }

public class ToggleEvent
{
   public MouseToggleEvent MouseEvent { get; private set; }
   public TouchToggleEvent TouchEvent { get; private set; }

   private float _actionDelay = .2f;
   private float _actionStart = -1f;
   private Vector2 _actionPosition = Vector2.zero;
   private float _actionPositionChangeAllowed = 5f;

   public ToggleEvent( MouseToggleEvent mouseEvent, TouchToggleEvent touchEvent )
   {
      MouseEvent = mouseEvent;
      TouchEvent = touchEvent;
   }

   public bool IsActive()
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

      if ( InputManager.UseTouchControls() )
      {
         switch ( TouchEvent )
         {
            case TouchToggleEvent.OneFingerTap:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.tapCount == 1 && x.phase == TouchPhase.Began ) )
               {
                  if ( _actionStart < 0 )
                  {
                     _actionStart = Time.unscaledTime;
                     _actionPosition = Input.touches[0].position;
                  }
               }
               else if ( Input.touchCount == 1 && Input.touches.All( x => x.tapCount == 1 && x.phase == TouchPhase.Ended ) )
               {
                  if ( _actionStart >= 0 )
                  {
                     var distance = Vector3.Distance( _actionPosition, Input.touches[0].position );
                     if ( ( ( Time.unscaledTime - _actionStart ) < _actionDelay ) && ( distance <= _actionPositionChangeAllowed ) )
                     {
                        _actionStart = -1f;
                        return true;
                     }
                     _actionStart = -1f;
                  }
               }
               break;
            }
            case TouchToggleEvent.OneFingerDown:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.phase != TouchPhase.Ended && x.phase != TouchPhase.Canceled ) )
               {
                  return true;
               }
               break;
            }
            case TouchToggleEvent.TwoFingersDown:
            {
               if ( Input.touchCount == 2 && Input.touches.All( x => x.phase != TouchPhase.Ended && x.phase != TouchPhase.Canceled ) )
               {
                  return true;
               }
               break;
            }
         }
      }
      else
      {
         switch ( MouseEvent )
         {
            case MouseToggleEvent.LeftClick:
            {
               if ( InputManager.IsMouseButtonPressed( InputManager.MouseButton.Left ) )
               {
                  if ( _actionStart < 0 )
                  {
                     _actionStart = Time.unscaledTime;
                     _actionPosition = Input.mousePosition;
                  }
               }
               else
               {
                  if ( _actionStart >= 0 )
                  {
                     var distance = Vector3.Distance( _actionPosition, Input.mousePosition );
                     if ( ( ( Time.unscaledTime - _actionStart ) < _actionDelay ) && ( distance <= _actionPositionChangeAllowed ) )
                     {
                        _actionStart = -1f;
                        return true;
                     }
                     _actionStart = -1f;
                  }
               }
               break;
            }
            case MouseToggleEvent.RightClick:
            {
               if ( InputManager.IsMouseButtonPressed( InputManager.MouseButton.Right ) )
               {
                  if ( _actionStart < 0 )
                  {
                     _actionStart = Time.unscaledTime;
                     _actionPosition = Input.mousePosition;
                  }
               }
               else
               {
                  if ( _actionStart >= 0 )
                  {
                     var distance = Vector3.Distance( _actionPosition, Input.mousePosition );
                     if ( ( ( Time.unscaledTime - _actionStart ) < _actionDelay ) && (distance <= _actionPositionChangeAllowed) )
                     {
                        _actionStart = -1f;
                        return true;
                     }
                     _actionStart = -1f;
                  }
               }
               break;
            }
            case MouseToggleEvent.LeftDown:
            {
               if ( InputManager.IsMouseButtonPressed( InputManager.MouseButton.Left ) )
               {
                  return true;
               }
               break;
            }
            case MouseToggleEvent.RightDown:
            {
               if ( InputManager.IsMouseButtonPressed( InputManager.MouseButton.Right ) )
               {
                  return true;
               }
               break;
            }
         }
      }

      return false;
   }
}

public class ValueEvent
{
   public MouseValueEvent MouseEvent { get; private set; }
   public TouchValueEvent TouchEvent { get; private set; }
   public float MouseMultiplier { get; private set; }
   public float TouchMultiplier { get; private set; }

   public ValueEvent( MouseValueEvent mouseEvent, TouchValueEvent touchEvent, float mouseMult = 1, float touchMult = 1 )
   {
      MouseEvent = mouseEvent;
      TouchEvent = touchEvent;
      MouseMultiplier = mouseMult;
      TouchMultiplier = touchMult;
   }

   public float GetValue()
   {
      if ( InputManager.UseTouchControls() )
      {
         switch ( TouchEvent )
         {
            case TouchValueEvent.OneFingerXAxis:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.phase == TouchPhase.Moved ) )
               {
                  return Input.touches[0].deltaPosition.x * TouchMultiplier;
               }
               break;
            }
            case TouchValueEvent.OneFingerYAxis:
            {
               if ( Input.touchCount == 1 && Input.touches.All( x => x.phase == TouchPhase.Moved ) )
               {
                  return Input.touches[0].deltaPosition.y * TouchMultiplier;
               }
               break;
            }
            case TouchValueEvent.OneFingerXPos:
            {
               if ( Input.touchCount > 0 )
               {
                  return Input.touches[0].position.x * TouchMultiplier;
               }
               break;
            }
            case TouchValueEvent.OneFingerYPos:
            {
               if ( Input.touchCount > 0 )
               {
                  return Input.touches[0].position.y * TouchMultiplier;
               }
               break;
            }
            case TouchValueEvent.PinchStretch:
            {
               if ( Input.touchCount == 2 && Input.touches.Any( x => x.phase == TouchPhase.Moved ) )
               {
                  var t1 = Input.touches[0];
                  var t2 = Input.touches[1];
                  var prevDistance =
                     (float)
                        Math.Sqrt(
                           Math.Pow( ( t1.position.x - t1.deltaPosition.x ) - ( t2.position.x - t2.deltaPosition.x ), 2 ) +
                           Math.Pow( ( t1.position.y - t1.deltaPosition.y ) - ( t2.position.y - t2.deltaPosition.y ), 2 ) );
                  var currDistance =
                     (float)
                        Math.Sqrt( Math.Pow( t1.position.x - t2.position.x, 2 ) +
                                   Math.Pow( t1.position.y - t2.position.y, 2 ) );
                  return ( currDistance - prevDistance ) * TouchMultiplier;
               }
               break;
            }
         }
      }
      else
      {
         switch ( MouseEvent )
         {
            case MouseValueEvent.XAxis:
            {
               return Input.GetAxis( "Mouse X" ) * MouseMultiplier;
            }
            case MouseValueEvent.YAxis:
            {
               return Input.GetAxis( "Mouse Y" ) * MouseMultiplier;
            }
            case MouseValueEvent.XPos:
            {
               return Input.mousePosition.x;
            }
            case MouseValueEvent.YPos:
            {
               return Input.mousePosition.y;
            }
            case MouseValueEvent.Scroll:
            {
               return Input.GetAxis( "Mouse ScrollWheel" ) * MouseMultiplier;
            }
         }
      }

      return 0;
   }
}

public class InputManager : MonoBehaviour
{
   public enum MouseButton { Left = 0, Right = 1, Middle = 2 }

   static InputManager()
   {
      Input.simulateMouseWithTouches = false;
   }

   public static bool IsMouseButtonPressed( MouseButton mouseButton )
   {
      return Input.GetMouseButton( Convert.ToInt16( mouseButton ) );
   }

   public static bool UseTouchControls()
   {
      return Input.touchCount != 0;
   }
}
