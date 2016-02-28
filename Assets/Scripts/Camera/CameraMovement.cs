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

   private readonly ToggleEvent _startRotateXY = new ToggleEvent( MouseToggleEvent.LeftDown, TouchToggleEvent.OneFingerDown );
   private readonly ValueEvent _dragX = new ValueEvent( MouseValueEvent.XAxis, TouchValueEvent.OneFingerXAxis, 5, .5f );
   private readonly ValueEvent _dragY = new ValueEvent( MouseValueEvent.YAxis,  TouchValueEvent.OneFingerYAxis, 5, .5f );
   private readonly ValueEvent _zoom = new ValueEvent( MouseValueEvent.Scroll, TouchValueEvent.PinchStretch, 5, .02f );

   private float _totalYRot = 0;

   void Start()
   {
   }

   private void Update()
   {
      if ( _startRotateXY.IsActive() )
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

         var tmpTotalRot = _totalYRot + rotY;
         if ( ClampRoation.ClampRoation )
         {
            if ( tmpTotalRot > ClampRoation.UpperClamp )
            {
               rotY = ClampRoation.UpperClamp - _totalYRot;
            }

            if ( tmpTotalRot < ClampRoation.LowerClamp )
            {
               rotY = ClampRoation.LowerClamp - _totalYRot;
            }
         }
         _totalYRot += rotY;

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
         Target.transform.localRotation = targetRot * Target.transform.localRotation;
      }

      var zoom = _zoom.GetValue();
      Target.transform.parent.Translate( 0, 0, -zoom, Space.World );
   }
}
