using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
   public Image Image;
   public Color BackgroundColor = Color.white;

   private readonly ValueEvent _pointerPosX = new ValueEvent( MouseValueEvent.XPos, TouchValueEvent.OneFingerXPos );
   private readonly ValueEvent _pointerPosY = new ValueEvent( MouseValueEvent.YPos, TouchValueEvent.OneFingerYPos );

   void Start()
   {
      var upperCirclePercent = .95f;
      var lowerCirclePercent = .60f;
      var textureSize = 1000;
      var circleStartDistance = lowerCirclePercent * textureSize / 2f;
      var circleEndDistance = upperCirclePercent * textureSize / 2f;

      var middle = new Vector2( ( textureSize - 1 ) / 2f, ( textureSize - 1 ) / 2f );

      var hueCircle = new Texture2D( textureSize, textureSize );

      // Fill with background color first
      var colors = new List<Color>( textureSize * textureSize );
      for ( int i = 0; i < colors.Capacity; i++ )
      {
         colors.Add( BackgroundColor );
      }
      hueCircle.SetPixels( colors.ToArray() );

      for ( int x = 0; x < textureSize; x++ )
      {
         for ( int y = 0; y < textureSize; y++ )
         {
            var dx = x - middle.x;
            var dy = y - middle.y;
            var distanceFromCenter = Mathf.Sqrt( Mathf.Pow( dx, 2 ) + Mathf.Pow( dy, 2 ) );
            if ( distanceFromCenter >= circleStartDistance && distanceFromCenter <= circleEndDistance )
            {
               var theta = Mathf.Atan2( dy, dx );

               var hue = ( theta + Mathf.PI ) / ( 2 * Mathf.PI );
               var sat = 1f;
               var val = 1f;
               var color = Color.HSVToRGB( hue, sat, val );

               hueCircle.SetPixel( x, y, color );
            }
         }
      }

      var boxSizePercent = .40f;
      var boxSize = boxSizePercent * textureSize;
      var boxStart = Convert.ToInt16( middle.x - boxSize / 2f );
      for ( int x = boxStart; x < boxStart + boxSize + 1; x++ )
      {
         for ( int y = boxStart; y < boxStart + boxSize + 1; y++ )
         {
            var theta = Mathf.PI / 2f;
            var hue = ( theta + Mathf.PI ) / ( 2 * Mathf.PI );
            var sat = ( x - boxStart ) / ( boxSize + 1 );
            var val = ( y - boxStart ) / ( boxSize + 1 );
            var color = Color.HSVToRGB( hue, sat, val );

            hueCircle.SetPixel( x, y, color );
         }
      }

      hueCircle.Apply();

      Image.sprite = Sprite.Create( hueCircle, new Rect( Image.sprite.rect.position, new Vector2( hueCircle.width, hueCircle.height ) ), Image.sprite.pivot );
   }

   void Update()
   {
      Vector2 localCursor;
      var pointerPos = new Vector2( _pointerPosX.GetValue(), _pointerPosY.GetValue() );
      var rect = Image.rectTransform;

      RectTransformUtility.ScreenPointToLocalPointInRectangle( rect, pointerPos, null, out localCursor );

      DebugHelper.Log( "Color Picker Pos", localCursor.ToString() );
   }

   //public EventSystem EventSystem { get; set; }

   //// Use this for initialization
   //void Start()
   //{
   //   EventSystem = GameObject.Find( "EventSystem" ).GetComponent<EventSystem>();
   //}

   //// Update is called once per frame
   //void Update()
   //{
   //   if ( EventSystem )
   //   {   
   //      if ( EventSystem.RaycastAll(  )IsPointerOverGameObject() )
   //      {
   //         DebugHelper.Log( "Event System", "YEAH" );
   //      }
   //   }
   //}
}
