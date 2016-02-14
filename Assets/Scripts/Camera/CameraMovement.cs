using UnityEngine;

public class CameraMovement : MonoBehaviour
{
   public GameObject Target;
   public bool AutoLockRotation = false;

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

         var targetRot = Quaternion.AngleAxis( -rotX, this.transform.up );
         targetRot *= Quaternion.AngleAxis( rotY, this.transform.right );

         Target.transform.localRotation = targetRot * Target.transform.localRotation;
      }

      var zoom = _zoom.GetValue();
      Target.transform.Translate( 0, 0, -zoom, Space.World );
   }
}
