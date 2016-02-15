using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
   [Serializable]
   public class RotationClamps
   {
      public bool ClampRoation = false;
      public float UpperClamp = 0;
      public float LowerClamp = 0;
   }

   public GameObject Target;
   public bool AutoLockRotation = false;
   public bool LimitRotation = false;
   public RotationClamps ClampRoation = new RotationClamps() {ClampRoation = false, UpperClamp = 0, LowerClamp = 0};

   private readonly ToggleEvent _startRotateXy = new ToggleEvent( MouseToggleEvent.LeftClick, TouchToggleEvent.OneFingerDown );
   private readonly ValueEvent _dragX = new ValueEvent( MouseValueEvent.XAxis, TouchValueEvent.OneFingerXAxis, 5, .5f );
   private readonly ValueEvent _dragY = new ValueEvent( MouseValueEvent.YAxis,  TouchValueEvent.OneFingerYAxis, 5, .5f );
   private readonly ValueEvent _zoom = new ValueEvent( MouseValueEvent.Scroll, TouchValueEvent.PinchStretch, 5, .02f );

   void Start()
   {
   }

   private void Update()
   {
      if ( _startRotateXy.IsActive() )
      {
         var rotX = _dragX.GetValue();
         var rotY = _dragY.GetValue();

         if ( AutoLockRotation )
         {
            if ( Mathf.Abs( rotX ) >= Mathf.Abs( rotY ) )
            {
               rotY = 0;
            }
            if ( Mathf.Abs( rotY ) >= Mathf.Abs( rotX ) )
            {
               rotX = 0;
            }
         }

         Quaternion targetRot;
         if ( LimitRotation )
         {
            targetRot = Quaternion.AngleAxis( -rotX, Target.transform.up );
            targetRot *= Quaternion.AngleAxis( rotY, this.transform.right );
         }
         else
         {
            targetRot = Quaternion.AngleAxis( -rotX, this.transform.up );
            targetRot *= Quaternion.AngleAxis( rotY, this.transform.right );
         }
         targetRot = targetRot * Target.transform.localRotation;
         if ( ClampRoation.ClampRoation )
         {
            var euler = targetRot.eulerAngles;
            if ( !( ( euler.x < ClampRoation.UpperClamp ) || ( euler.x > 360 - ClampRoation.LowerClamp ) ) )
            {
               if ( Mathf.Abs( euler.x - ClampRoation.UpperClamp ) <= Mathf.Abs( euler.x - ClampRoation.LowerClamp ) )
               {
                  euler.x = ClampRoation.UpperClamp;
               }
               else
               {
                  euler.x = ClampRoation.LowerClamp;
               }
               targetRot = Quaternion.Euler( euler );
            }
         }
         Target.transform.localRotation = targetRot;
      }

      var zoom = _zoom.GetValue();
      Target.transform.Translate( 0, 0, -zoom, Space.World );
   }

   public void ResetRotationY()
   {
      //var euler = Target.transform.localRotation.eulerAngles;
      //euler.z = 0;
      //Target.transform.localRotation = Quaternion.Euler( euler );
      //Target.transform.Rotate( new Vector3( 10, 0, 0 ), Space.World );

      var euler = Target.transform.localRotation.eulerAngles;
      //Target.transform.localRotation = Quaternion.Euler( -euler.x, -euler.y, -euler.z );
      Target.transform.localRotation = Quaternion.Euler( 0, euler.y, euler.z ) * Quaternion.LookRotation( Camera.main.transform.forward, Camera.main.transform.up );
   }
}
