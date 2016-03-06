﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
   public Image Image;
   public Color BackgroundColor = Color.white;
   public Image HueSelector;
   public Image SaturationValueSelector;

   private static float _upperCirclePercent = .95f;
   private static float _lowerCirclePercent = .60f;
   private static float _boxSizePercent = .40f;
   private static int _textureSize = 1000;
   private static Vector2 _middle = new Vector2( ( _textureSize - 1 ) / 2f, ( _textureSize - 1 ) / 2f );

   private bool _dragHueStarted = false;
   private bool _dragSatValStarted = false;
   private bool _dragOtherStarted = false;

   private readonly ToggleEvent _pointerDown = new ToggleEvent( MouseToggleEvent.LeftDown, TouchToggleEvent.OneFingerDown );
   private readonly ValueEvent _pointerPosX = new ValueEvent( MouseValueEvent.XPos, TouchValueEvent.OneFingerXPos );
   private readonly ValueEvent _pointerPosY = new ValueEvent( MouseValueEvent.YPos, TouchValueEvent.OneFingerYPos );

   void Start()
   {
      FillBackground( BackgroundColor );
      DrawHueCircle();
      DrawSatValBox();

      MoveHueSelector( Vector2.up );
      MoveSatValSelector( Vector2.zero, new Rect( 0, 0, 0, 0 ) );
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
            var theta = Mathf.Atan2( dy, dx );

            if ( !_dragSatValStarted )
            {
               var distanceFromCenter = Mathf.Sqrt( Mathf.Pow( dx, 2 ) + Mathf.Pow( dy, 2 ) );
               if ( _dragHueStarted || ( distanceFromCenter >= circleStartDistance && distanceFromCenter <= circleEndDistance ) )
               {
                  _dragHueStarted = true;
                  MoveHueSelector( localCursor );
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
      var texture = new Texture2D( Image.sprite.texture.width, Image.sprite.texture.height );
      texture.SetPixels( Image.sprite.texture.GetPixels() );

      var circleStartDistance = _lowerCirclePercent * _textureSize / 2f;
      var circleEndDistance = _upperCirclePercent * _textureSize / 2f;

      for ( int x = 0; x < _textureSize; x++ )
      {
         for ( int y = 0; y < _textureSize; y++ )
         {
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

               texture.SetPixel( x, y, color );
            }
         }
      }

      texture.Apply();
      Image.sprite = Sprite.Create( texture, new Rect( Image.sprite.rect.position, new Vector2( texture.width, texture.height ) ), Image.sprite.pivot );
   }

   private void DrawSatValBox()
   {
      var texture = new Texture2D( Image.sprite.texture.width, Image.sprite.texture.height );
      texture.SetPixels( Image.sprite.texture.GetPixels() );

      var boxSize = _boxSizePercent * _textureSize;
      var boxStart = Convert.ToInt16( _middle.x - boxSize / 2f );
      for ( int x = boxStart; x < boxStart + boxSize + 1; x++ )
      {
         for ( int y = boxStart; y < boxStart + boxSize + 1; y++ )
         {
            var theta = Mathf.PI / 2f;
            var hue = ( theta + Mathf.PI ) / ( 2 * Mathf.PI );
            var sat = ( x - boxStart ) / ( boxSize + 1 );
            var val = ( y - boxStart ) / ( boxSize + 1 );
            var color = Color.HSVToRGB( hue, sat, val );

            texture.SetPixel( x, y, color );
         }
      }

      texture.Apply();
      Image.sprite = Sprite.Create( texture, new Rect( Image.sprite.rect.position, new Vector2( texture.width, texture.height ) ), Image.sprite.pivot );
   }

   private void MoveHueSelector( Vector2 localCursor )
   {
      var selectorPosPercent = _lowerCirclePercent + ( ( _upperCirclePercent - _lowerCirclePercent ) / 2f );
      var selectorDistance = ( selectorPosPercent * Image.rectTransform.rect.width ) / 2f;

      var selectorPos = MathHelper.FindPoint( Vector2.zero, localCursor, selectorDistance );
      selectorPos.Scale( Image.transform.lossyScale );

      HueSelector.transform.position = Image.transform.position + new Vector3( selectorPos.x, selectorPos.y, 0 );
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