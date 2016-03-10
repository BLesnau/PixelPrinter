using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
   public Image Image;
   public Color BackgroundColor = Color.white;
   public Image HueSelector;
   public Image SaturationValueSelector;
   public CanvasGroup CanvasGroup;

   private static float _upperCirclePercent = .95f;
   private static float _lowerCirclePercent = .60f;
   private static float _boxSizePercent = .40f;
   private static int _textureSize = 500;
   private static Vector2 _middle = new Vector2( ( _textureSize - 1 ) / 2f, ( _textureSize - 1 ) / 2f );

   private bool _dragHueStarted = false;
   private bool _dragSatValStarted = false;
   private bool _dragOtherStarted = false;

   private float _hueTheta = 0;

   private readonly ToggleEvent _pointerDown = new ToggleEvent( MouseToggleEvent.LeftDown, TouchToggleEvent.OneFingerDown );
   private readonly ValueEvent _pointerPosX = new ValueEvent( MouseValueEvent.XPos, TouchValueEvent.OneFingerXPos );
   private readonly ValueEvent _pointerPosY = new ValueEvent( MouseValueEvent.YPos, TouchValueEvent.OneFingerYPos );

   void Start()
   {
      MoveHueSelector( Vector2.up );
      MoveSatValSelector( Vector2.zero, new Rect( 0, 0, 0, 0 ) );

      FillBackground( BackgroundColor );
      DrawHueCircle();
      DrawSatValBox();
   }

   void Update()
   {
      if ( _pointerDown.IsActive() )
      {
         if ( !_dragOtherStarted )
         {
            var circleStartDistance = _lowerCirclePercent * Image.rectTransform.rect.width / 2f;
            var circleEndDistance = _upperCirclePercent * Image.rectTransform.rect.width / 2f;

            Vector2 localCursor;
            var pointerPos = new Vector2( _pointerPosX.GetValue(), _pointerPosY.GetValue() );
            var rect = Image.rectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle( rect, pointerPos, null, out localCursor );

            var dx = localCursor.x;
            var dy = localCursor.y;

            if ( !_dragSatValStarted )
            {
               var distanceFromCenter = Mathf.Sqrt( Mathf.Pow( dx, 2 ) + Mathf.Pow( dy, 2 ) );
               if ( _dragHueStarted || ( distanceFromCenter >= circleStartDistance && distanceFromCenter <= circleEndDistance ) )
               {
                  _dragHueStarted = true;
                  MoveHueSelector( localCursor );
                  DrawSatValBox();
               }
            }

            if ( !_dragHueStarted )
            {
               var boxSize = _boxSizePercent * Image.rectTransform.rect.width;
               var boxStart = Convert.ToInt16( -boxSize / 2f );
               var boxRect = new Rect( boxStart, boxStart, boxSize, boxSize );
               if ( _dragSatValStarted || ( ( dx >= boxRect.xMin ) && ( dx <= boxRect.xMax ) && ( dy >= boxRect.yMin ) && ( dy <= boxRect.yMax ) ) )
               {
                  _dragSatValStarted = true;

                  MoveSatValSelector( localCursor, boxRect );
               }
            }

            _dragOtherStarted = !( _dragHueStarted || _dragSatValStarted );
         }
      }
      else
      {
         _dragHueStarted = false;
         _dragSatValStarted = false;
         _dragOtherStarted = false;
      }
   }

   public void Show()
   {
      ToggleVisibility( true );
   }

   public void Hide()
   {
      ToggleVisibility( false );
   }

   public bool IsVisible()
   {
      return this.enabled;
   }

   private void ToggleVisibility( bool isVisible )
   {
      this.enabled = isVisible;
      CanvasGroup.alpha = isVisible ? 1 : 0;
      CanvasGroup.blocksRaycasts = isVisible;
      CanvasGroup.interactable = isVisible;
   }

   private void FillBackground( Color color )
   {
      var texture = new Texture2D( _textureSize, _textureSize );

      var colors = new List<Color>( _textureSize * _textureSize );
      for ( int i = 0; i < colors.Capacity; i++ )
      {
         colors.Add( BackgroundColor );
      }
      texture.SetPixels( colors.ToArray() );

      texture.Apply();
      Image.sprite = Sprite.Create( texture, new Rect( Image.sprite.rect.position, new Vector2( texture.width, texture.height ) ), Image.sprite.pivot );
   }

   private void DrawHueCircle()
   {
      var texture = Image.sprite.texture;
      var pixels = texture.GetPixels();

      var circleStartDistance = _lowerCirclePercent * texture.width / 2f;
      var circleEndDistance = _upperCirclePercent * texture.width / 2f;

      for ( int i = 0; i < texture.width * texture.height; i++ )
      {
         var y = i / texture.width;
         var x = i - ( y * texture.width );

         var dx = x - _middle.x;
         var dy = y - _middle.y;
         var distanceFromCenter = Mathf.Sqrt( Mathf.Pow( dx, 2 ) + Mathf.Pow( dy, 2 ) );
         if ( distanceFromCenter >= circleStartDistance && distanceFromCenter <= circleEndDistance )
         {
            var theta = Mathf.Atan2( dy, dx );

            var hue = ( theta + Mathf.PI ) / ( 2 * Mathf.PI );
            var sat = 1f;
            var val = 1f;
            var color = Color.HSVToRGB( hue, sat, val );

            pixels[y * texture.width + x] = color;
         }
      }

      texture.SetPixels( pixels );
      texture.Apply();
   }

   private void DrawSatValBox()
   {
      var texture = Image.sprite.texture;
      var hue = ( _hueTheta + Mathf.PI ) / ( 2 * Mathf.PI );
      var boxSize = Convert.ToInt16( _boxSizePercent * _textureSize );
      var boxStart = Convert.ToInt16( _middle.x - boxSize / 2f );

      var colors = new List<Color>( Convert.ToInt16( boxSize ) * Convert.ToInt16( boxSize ) );
      for ( int i = 0; i < colors.Capacity; i++ )
      {
         var y = i / boxSize;
         var x = i - ( y * boxSize );
         float yF = y;
         float xF = x;

         var sat = ( xF ) / ( boxSize + 1 );
         var val = ( yF ) / ( boxSize + 1 );
         var color = Color.HSVToRGB( hue, sat, val );

         colors.Add( color );
      }
      texture.SetPixels( boxStart, boxStart, boxSize, boxSize, colors.ToArray() );
      texture.Apply();
   }

   private void MoveHueSelector( Vector2 localCursor )
   {
      var selectorPosPercent = _lowerCirclePercent + ( ( _upperCirclePercent - _lowerCirclePercent ) / 2f );
      var selectorDistance = ( selectorPosPercent * Image.rectTransform.rect.width ) / 2f;

      var selectorPos = MathHelper.FindPoint( Vector2.zero, localCursor, selectorDistance );
      selectorPos.Scale( Image.transform.lossyScale );

      HueSelector.transform.position = Image.transform.position + new Vector3( selectorPos.x, selectorPos.y, 0 );

      _hueTheta = Mathf.Atan2( selectorPos.y, selectorPos.x );
   }

   private void MoveSatValSelector( Vector2 localCursor, Rect boxRect )
   {
      var selectorPos = new Vector3( localCursor.x, localCursor.y, 0 );
      selectorPos.x = Mathf.Max( selectorPos.x, boxRect.xMin );
      selectorPos.x = Mathf.Min( selectorPos.x, boxRect.xMax );
      selectorPos.y = Mathf.Max( selectorPos.y, boxRect.yMin );
      selectorPos.y = Mathf.Min( selectorPos.y, boxRect.yMax );
      selectorPos.Scale( Image.transform.lossyScale );

      SaturationValueSelector.transform.position = Image.transform.position + selectorPos;
   }
}