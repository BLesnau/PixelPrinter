using UnityEngine;

public class CameraMovement : MonoBehaviour
{
   public GameObject Target;

   private ToggleEvent StartRotateXY = new ToggleEvent() {MouseEvent = MouseToggleEvent.LeftClick, TouchEvent = TouchToggleEvent.OneFingerDown};
   private ValueEvent DragX = new ValueEvent() {MouseEvent = MouseValueEvent.XAxis, TouchEvent = TouchValueEvent.OneFingerXAxis, MouseMultiplier = 5, TouchMultiplier = .5f};
   private ValueEvent DragY = new ValueEvent() {MouseEvent = MouseValueEvent.YAxis, TouchEvent = TouchValueEvent.OneFingerYAxis, MouseMultiplier = 5, TouchMultiplier = .5f};
   private ValueEvent Zoom = new ValueEvent() {MouseEvent = MouseValueEvent.Scroll, TouchEvent = TouchValueEvent.PinchStretch, MouseMultiplier = 5, TouchMultiplier = .02f};

   void Start()
   {
   }

   // Update is called once per frame
   private void Update()
   {
      //if ( InputManager.IsMouseButtonPressed( InputManager.MouseButton.Left ) )
      //{
      //   var rotX = Input.GetAxis( "Mouse X" ) * Sensitivity;
      //   var rotY = Input.GetAxis( "Mouse Y" ) * Sensitivity;

      //   var targetRot = Quaternion.AngleAxis( -rotX, this.transform.up );
      //   targetRot *= Quaternion.AngleAxis( rotY, this.transform.right );

      //   Target.transform.localRotation = targetRot * Target.transform.localRotation;
      //}

      if ( InputManager.IsActive( StartRotateXY ) )
      {
         var rotX = InputManager.GetValue( DragX );
         var rotY = InputManager.GetValue( DragY );

         var targetRot = Quaternion.AngleAxis( -rotX, this.transform.up );
         targetRot *= Quaternion.AngleAxis( rotY, this.transform.right );

         Target.transform.localRotation = targetRot * Target.transform.localRotation;
      }

      var zoom = InputManager.GetValue( Zoom );
      Target.transform.Translate( 0, 0, -zoom, Space.World );
   }

   public static float ClampAngle( float angle, float min, float max )
   {
      angle = angle % 360;
      if ( ( angle >= -360F ) && ( angle <= 360F ) )
      {
         if ( angle < -360F )
         {
            angle += 360F;
         }
         if ( angle > 360F )
         {
            angle -= 360F;
         }
      }

      return Mathf.Clamp( angle, min, max );
   }
}
