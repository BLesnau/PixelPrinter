using UnityEngine;

[AddComponentMenu( "Camera-Control/Mouse" )]
public class MouseCameraControl : MonoBehaviour
{
   [System.Serializable]
   public class Modifiers
   {
      public bool leftAlt;
      public bool leftControl;
      public bool leftShift;

      public bool checkModifiers()
      {
         return ( !leftAlt ^ Input.GetKey( KeyCode.LeftAlt ) ) &&
             ( !leftControl ^ Input.GetKey( KeyCode.LeftControl ) ) &&
             ( !leftShift ^ Input.GetKey( KeyCode.LeftShift ) );
      }
   }

   [System.Serializable]
   public class MouseControlConfiguration
   {
      public bool activate;
      public InputManager.MouseButton mouseButton;
      public Modifiers modifiers;
      public float sensitivity;

      public bool isActivated()
      {
         return activate && Input.GetMouseButton( (int)mouseButton ) && modifiers.checkModifiers();
      }
   }

   [System.Serializable]
   public class MouseScrollConfiguration
   {
      public bool activate;
      public Modifiers modifiers;
      public float sensitivity;

      public bool isActivated()
      {
         return activate && modifiers.checkModifiers();
      }
   }

   public MouseControlConfiguration yaw = new MouseControlConfiguration { mouseButton = InputManager.MouseButton.Right, sensitivity = 10F };
   public MouseControlConfiguration pitch = new MouseControlConfiguration { mouseButton = InputManager.MouseButton.Right, modifiers = new Modifiers{ leftControl = true }, sensitivity = 10F };
   public MouseControlConfiguration roll = new MouseControlConfiguration();
   public MouseControlConfiguration verticalTranslation = new MouseControlConfiguration { mouseButton = InputManager.MouseButton.Middle, sensitivity = 2F };
   public MouseControlConfiguration horizontalTranslation = new MouseControlConfiguration { mouseButton = InputManager.MouseButton.Middle, sensitivity = 2F };
   public MouseControlConfiguration depthTranslation = new MouseControlConfiguration { mouseButton = InputManager.MouseButton.Left, sensitivity = 2F };
   public MouseScrollConfiguration scroll = new MouseScrollConfiguration { sensitivity = 2F };

   // Default unity names for mouse axes
   public string mouseHorizontalAxisName = "Mouse X";
   public string mouseVerticalAxisName = "Mouse Y";
   public string scrollAxisName = "Mouse ScrollWheel";

   void LateUpdate()
   {
      if ( yaw.isActivated() && pitch.isActivated() )
      {
         float rotationX = Input.GetAxis( mouseHorizontalAxisName ) * yaw.sensitivity;
         Debug.Log( rotationX );
         float rotationY = Input.GetAxis( mouseVerticalAxisName ) * pitch.sensitivity;
         transform.Rotate( -rotationY, rotationX, 0 );
      }
      else
      {
         if ( yaw.isActivated() )
         {
            float rotationX = Input.GetAxis( mouseHorizontalAxisName ) * yaw.sensitivity;
            transform.Rotate( 0, rotationX, 0 );
         }

         if ( pitch.isActivated() )
         {
            float rotationY = Input.GetAxis( mouseVerticalAxisName ) * pitch.sensitivity;
            transform.Rotate( -rotationY, 0, 0 );
         }
      }
      if ( roll.isActivated() )
      {
         float rotationZ = Input.GetAxis( mouseHorizontalAxisName ) * roll.sensitivity;
         transform.Rotate( 0, 0, rotationZ );
      }

      if ( verticalTranslation.isActivated() )
      {
         float translateY = Input.GetAxis( mouseVerticalAxisName ) * verticalTranslation.sensitivity;
         transform.Translate( 0, translateY, 0 );
      }

      if ( horizontalTranslation.isActivated() )
      {
         float translateX = Input.GetAxis( mouseHorizontalAxisName ) * horizontalTranslation.sensitivity;
         transform.Translate( translateX, 0, 0 );
      }

      if ( depthTranslation.isActivated() )
      {
         float translateZ = Input.GetAxis( mouseVerticalAxisName ) * depthTranslation.sensitivity;
         transform.Translate( 0, 0, translateZ );
      }

      if ( scroll.isActivated() )
      {
         float translateZ = Input.GetAxis( scrollAxisName ) * scroll.sensitivity;

         transform.Translate( 0, 0, translateZ );
      }
   }

}