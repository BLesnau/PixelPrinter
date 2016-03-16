using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
   public UIManager UIManager;
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

   private readonly ToggleEvent _pointerDown = new ToggleEvent( MouseToggleEvent.LeftDown,
      TouchToggleEvent.OneFingerDown );

   private readonly ValueEvent _pointerPosX = new ValueEvent( MouseValueEvent.XPos, TouchValueEvent.OneFingerXPos );
   private readonly ValueEvent _pointerPosY = new ValueEvent( MouseValueEvent.YPos, TouchValueEvent.OneFingerYPos );

   private void Start()
   {
      MoveHueSelector( Vector2.up );
      MoveSatValSelector( Vector2.zero, new Rect( 0, 0, 0, 0 ) );
      UpdateSelectedColor();

      FillBackground( BackgroundColor );
      DrawHueCircle();
      DrawSatValBox();

      Hide();
   }

   private void Update()
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
               if ( _dragHueStarted ||
                    ( distanceFromCenter >= circleStartDistance && distanceFromCenter <= circleEndDistance ) )
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
               if ( _dragSatValStarted ||
                    ( ( dx >= boxRect.xMin ) && ( dx <= boxRect.xMax ) && ( dy >= boxRect.yMin ) &&
                      ( dy <= boxRect.yMax ) ) )
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

      UpdateSelectedColor();
   }

   public void Show( Color color )
   {
      ToggleVisibility( true );
      SetColor( color );
   }

   public void SetColor( Color color )
   {
      float hue, sat, val;
      Color.RGBToHSV( color, out hue, out sat, out val );

      var boxSize = _boxSizePercent * Image.rectTransform.rect.width;
      var boxSizeDim = new Vector2( boxSize, boxSize );
      boxSizeDim.Scale( Image.rectTransform.lossyScale );

      var boxStart = Convert.ToInt16( -boxSize / 2f );
      var boxStartPosLocal = new Vector3( boxStart, boxStart );
      boxStartPosLocal.Scale( Image.rectTransform.lossyScale );
      var boxStartPos = Image.rectTransform.position + boxStartPosLocal;

      var localSatValPos = new Vector3( boxSizeDim.x * sat, boxSizeDim.y * val );
      var satValPos = localSatValPos + boxStartPos;

      var hueTheta = ( hue * ( 2 * Mathf.PI ) ) - Mathf.PI;
      MoveHueSelector( new Vector2( Mathf.Cos( hueTheta ), Mathf.Sin( hueTheta ) ), true );

      var boxRect = new Rect( boxStart, boxStart, boxSize, boxSize );
      MoveSatValSelector( satValPos, boxRect );

      DrawSatValBox();
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

   private void MoveHueSelector( Vector2 localCursor, bool animate = false )
   {
      iTween.StopByName("HueMove");

      var selectorPosPercent = _lowerCirclePercent + ( ( _upperCirclePercent - _lowerCirclePercent ) / 2f );
      var selectorDistance = ( selectorPosPercent * Image.rectTransform.rect.width ) / 2f;

      var selectorPos = MathHelper.FindPoint( Vector2.zero, localCursor, selectorDistance );
      selectorPos.Scale( Image.transform.lossyScale );

      if ( animate )
      {
         var path = new List<Vector3>();
         path.Add( HueSelector.transform.position );

         var currentRelativePos = HueSelector.transform.position - Image.transform.position;
         var currentTheta = Mathf.Atan2( currentRelativePos.y, currentRelativePos.x );
         currentTheta = MathHelper.GetPositiveTheta( currentTheta );
         var newTheta = Mathf.Atan2( selectorPos.y, selectorPos.x );
         newTheta = MathHelper.GetPositiveTheta( newTheta );

         var thetaInc = .2f;
         while ( currentTheta != newTheta )
         {
            var origTheta = currentTheta;

            var point = MathHelper.FindPoint( Vector2.zero, new Vector2( Mathf.Cos( currentTheta ), Mathf.Sin( currentTheta ) ), selectorDistance );
            point.Scale( Image.transform.lossyScale );
            point = Image.transform.position + new Vector3( point.x, point.y, 0 );
            path.Add( point );

            currentTheta -= thetaInc;
            currentTheta = currentTheta >= 0 ? currentTheta : Mathf.PI * 2 - Mathf.Abs( currentTheta );

            if ( origTheta >= newTheta && currentTheta <= newTheta )
            {
               currentTheta = newTheta;
            }
         } 

         var newPos = Image.transform.position + new Vector3( selectorPos.x, selectorPos.y, 0 );
         path.Add( newPos );

         iTween.MoveTo( HueSelector.gameObject, iTween.Hash("name", "HueMove", "path", path.ToArray(), "time", 2, /*"easetype", iTween.EaseType.linear,*/ "looptype", iTween.LoopType.none, "movetopath", true ) );
      }
      else
      {
         HueSelector.transform.position = Image.transform.position + new Vector3( selectorPos.x, selectorPos.y, 0 );
      }

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

   private void UpdateSelectedColor()
   {
      var boxSize = _boxSizePercent * Image.rectTransform.rect.width;
      var boxSizeDim = new Vector2( boxSize, boxSize );
      boxSizeDim.Scale( Image.rectTransform.lossyScale );

      var boxStart = Convert.ToInt16( -boxSize / 2f );
      var boxStartPosLocal = new Vector3( boxStart, boxStart );
      boxStartPosLocal.Scale( Image.rectTransform.lossyScale );
      var boxStartPos = Image.rectTransform.position + boxStartPosLocal;

      var satValPos = SaturationValueSelector.transform.position - boxStartPos;

      var sat = satValPos.x / boxSizeDim.x;
      var val = satValPos.y / boxSizeDim.y;
      var hue = ( _hueTheta + Mathf.PI ) / ( 2 * Mathf.PI );
      var color = Color.HSVToRGB( hue, sat, val );

      UIManager.SetSelectedColor( color );
   }
}