using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PixelManager : MonoBehaviour
{
   public UIManager UIManager;

   public Pixel PixelPrefab = null;

   public int DepthCount = 21;
   public int ColCount = 21;
   public int RowCount = 21;
   public float PixelScale = 1;
   public float GridScale = .1f;
   //public float popInDelaySeconds = .1f;
   //public bool animatePopIn = true;

   private PixelFigure _pixelFigure = null;
   private List<PixelConfig> _placeablePixels = null;
   private ActionStack _actionStack = null;
   //private int _poppedInCount = 0;
   //private TimeSpan _popInTimeElapsed = TimeSpan.Zero;
   //private AudioSource _audio;

   private readonly ToggleEvent _placePixel = new ToggleEvent( MouseToggleEvent.LeftClick, TouchToggleEvent.OneFingerTap );
   private readonly ValueEvent _pointerPosX = new ValueEvent( MouseValueEvent.XPos, TouchValueEvent.OneFingerXPos );
   private readonly ValueEvent _pointerPosY = new ValueEvent( MouseValueEvent.YPos, TouchValueEvent.OneFingerYPos );

   private void Start()
   {
      _actionStack = new ActionStack();
      _placeablePixels = new List<PixelConfig>();
   }

   private void Reset()
   {
      _actionStack = new ActionStack();
      //_audio = GetComponent<AudioSource>();

      if ( _pixelFigure != null )
      {
         _pixelFigure.Unload();
      }

      foreach ( var pixel in _placeablePixels )
      {
         pixel.Unload();
      }
      _placeablePixels = new List<PixelConfig>();
   }

   public void New()
   {
      Reset();

      _pixelFigure = new PixelFigure( ColCount, RowCount, DepthCount );

      //_pixelFigure.InitializePixels();

      _pixelFigure.GetPixel( _pixelFigure.ColCount / 2, _pixelFigure.RowCount / 2, _pixelFigure.DepthCount / 2 ).Color = Color.red;

      PopInAllPixels();

      DetectPlaceablePixels();
   }

   public void Import()
   {
      Reset();

      var files = Directory.GetFiles( Application.persistentDataPath );
      if ( files.Count() > 0 )
      {
         if ( files[0] != null )
         {
            //var filePath = Path.GetFileNameWithoutExtension( files[0] );
            var filePath = new Guid( "6590617e-4287-414f-a6a0-ae2c5d2aecb9" );
            if ( FileHelper.FileExists( filePath.ToString() ) )
            {

               var size = PixelFigure.GetSize( filePath );
               ColCount = Convert.ToInt16( size.x );
               RowCount = Convert.ToInt16( size.y );
               DepthCount = Convert.ToInt16( size.z );

               _pixelFigure = new PixelFigure( filePath );

               // InitializePixels();

               PopInAllPixels();

               DetectPlaceablePixels();
            }
         }
      }
   }

   private void PopInAllPixels()
   {
      //if ( !animatePopIn )
      //{
      for ( var x = 0; x < _pixelFigure.ColCount; x++ )
      {
         for ( var y = 0; y < _pixelFigure.RowCount; y++ )
         {
            for ( var z = 0; z < _pixelFigure.DepthCount; z++ )
            {
               if ( _pixelFigure.GetPixel( x, y, z ).Color.a > 0 )
               {
                  PopIn( _pixelFigure.GetPixel( x, y, z ) );
               }
            }
         }
      }
      //}
   }

   private void Update()
   {
      if ( UIManager.IsModalActive() )
      {
         return;
      }

      //if ( _pixelsToPopIn.Any() )
      //{
      //   _popInTimeElapsed += TimeSpan.FromSeconds( Time.deltaTime );
      //   if ( ( _popInTimeElapsed.TotalSeconds / popInDelaySeconds ) > _poppedInCount )
      //   {
      //      PopIn( _pixelsToPopIn[0] );

      //      if ( _audio )
      //      {
      //         if ( !_audio.isPlaying )
      //         {
      //            _audio.Play();
      //         }

      //         if ( !_pixelsToPopIn.Any() )
      //         {
      //            _audio.Stop();
      //         }
      //      }
      //   }
      //}

      var pointerPos = new Vector3( _pointerPosX.GetValue(), _pointerPosY.GetValue(), 0 );
      var ray = Camera.main.ScreenPointToRay( pointerPos );
      Debug.DrawRay( ray.origin, ray.direction * 100, Color.green );

      PixelConfig selectedPixel = null;
      if ( _placePixel.IsActive() )
      {
         var hits = Physics.RaycastAll( ray );
         var sortedPixels = SortClosestPixels( ConvertToPixels( hits ) );
         if ( sortedPixels.Any( p => p.Color.a > 0 ) )
         {
            var pix = sortedPixels.First( p => p.Color.a > 0 );
            if ( UIManager.SelectedTool == UIManager.Tools.Add )
            {
               var sortedSurroundingPixels = SortClosestPixels( GetSurroundingPixels( sortedPixels, pix ) );
               try
               {
                  selectedPixel = sortedSurroundingPixels.First( p => _placeablePixels.Contains( p ) );
               }
               catch { }
            }
            else if ( UIManager.SelectedTool == UIManager.Tools.Remove || UIManager.SelectedTool == UIManager.Tools.Change )
            {
               selectedPixel = pix;
            }
         }
         else if ( UIManager.SelectedTool == UIManager.Tools.Add )
         {
            try
            {
               selectedPixel = sortedPixels.First( p => _placeablePixels.Contains( p ) );
            }
            catch { }
         }


         if ( selectedPixel != null )
         {
            IEditAction action = null;
            if ( UIManager.SelectedTool == UIManager.Tools.Add )
            {
               selectedPixel.Color = UIManager.GetSelectedColor();
               RemoveFromPlaceablePixels( selectedPixel );
               action = new AddAction( this, selectedPixel );
            }
            else if ( UIManager.SelectedTool == UIManager.Tools.Remove )
            {
               action = new RemoveAction( this, selectedPixel );
               PopOut( selectedPixel );
            }
            else if ( UIManager.SelectedTool == UIManager.Tools.Change )
            {
               if ( selectedPixel.Color != UIManager.GetSelectedColor() )
               {
                  action = new ChangeAction( this, selectedPixel, UIManager.GetSelectedColor() );
                  selectedPixel.Color = UIManager.GetSelectedColor();
               }
            }

            DetectPlaceablePixels();

            if ( action != null )
            {
               _actionStack.AddAction( action );
               _pixelFigure.SaveState();
            }
         }
      }
   }

   public void PopIn( PixelConfig config )
   {
      var tmpEuler = transform.localRotation.eulerAngles;
      var tmpPostion = transform.position;
      transform.localRotation = Quaternion.identity;
      transform.position = new Vector3( 0, 0, 0 );

      var pixel = (Pixel)Instantiate( PixelPrefab, config.Position, Quaternion.identity );
      if ( pixel )
      {
         pixel.Color = config.Color;
         pixel.transform.localScale = new Vector3( PixelScale, PixelScale, PixelScale );
         config.Prefab = pixel;

         pixel.transform.parent = this.transform;
      }

      transform.localRotation = Quaternion.Euler( tmpEuler );
      transform.position = tmpPostion;
   }

   public void PopOut( PixelConfig config )
   {
      if ( config.Prefab )
      {
         Destroy( config.Prefab.gameObject );

         var currentColor = config.Color;
         currentColor.a = 0;
         config.Color = currentColor;
      }
   }

   public void DetectPlaceablePixels()
   {
      _placeablePixels.ForEach( PopOut );
      _placeablePixels.Clear();

      var doIt = true;
      if ( doIt )
      {
         for ( var x = 0; x < _pixelFigure.ColCount; x++ )
         {
            for ( var y = 0; y < _pixelFigure.RowCount; y++ )
            {
               for ( var z = 0; z < _pixelFigure.DepthCount; z++ )
               {
                  // Current pixel is visible
                  if ( _pixelFigure.GetPixel( x, y, z ).Color.a > 0 )
                  {
                     //Loop through all surrounding pixels
                     for ( var x2 = x - 1; x2 <= x + 1; x2++ )
                     {
                        for ( var y2 = y - 1; y2 <= y + 1; y2++ )
                        {
                           for ( var z2 = z - 1; z2 <= z + 1; z2++ )
                           {
                              // Pixel is within bounding box
                              if ( x2 >= 0 && y2 >= 0 & z2 >= 0 && x2 < _pixelFigure.ColCount && y2 < _pixelFigure.RowCount && z2 < _pixelFigure.DepthCount )
                              {
                                 var equalAmount = 0;
                                 equalAmount += x2 == x ? 1 : 0;
                                 equalAmount += y2 == y ? 1 : 0;
                                 equalAmount += z2 == z ? 1 : 0;

                                 // Pixel is not diagonal
                                 if ( equalAmount >= 2 )
                                 {
                                    var pixel = _pixelFigure.GetPixel( x2, y2, z2 );

                                    // Already was a placeable pixel
                                    if ( !_placeablePixels.Contains( pixel ) )
                                    {
                                       // Pixel is not already visible
                                       if ( pixel.Color.a <= 0f )
                                       {
                                          _placeablePixels.Add( pixel );
                                          PopIn( pixel );
                                       }
                                    }
                                 }
                              }
                           }
                        }
                     }
                  }
               }
            }
         }
      }
   }

   private IEnumerable<PixelConfig> ConvertToPixels( RaycastHit[] hits )
   {
      var hitList = hits.ToList();
      return hitList.Select( h =>
      {
         var pixel = h.transform.gameObject.GetComponent<Pixel>();
         return _pixelFigure.GetPixelConfigFromPrefab( pixel );
      } );
   }

   private IEnumerable<PixelConfig> SortClosestPixels( IEnumerable<PixelConfig> pixels )
   {
      var sortedList = pixels.ToList();
      sortedList.Sort( ( p1, p2 ) =>
      {
         var distance1 = Vector3.Distance( p1.Prefab.transform.position, Camera.main.transform.position );
         var distance2 = Vector3.Distance( p2.Prefab.transform.position, Camera.main.transform.position );
         return distance1.CompareTo( distance2 );
      } );

      return sortedList;
   }

   private IEnumerable<PixelConfig> GetSurroundingPixels( IEnumerable<PixelConfig> pixels, PixelConfig pixel )
   {
      var surroundingPixels = new List<PixelConfig>();

      var x = pixel.XIndex;
      var y = pixel.YIndex;
      var z = pixel.ZIndex;

      //Loop through all surrounding pixels
      for ( var x2 = x - 1; x2 <= x + 1; x2++ )
      {
         for ( var y2 = y - 1; y2 <= y + 1; y2++ )
         {
            for ( var z2 = z - 1; z2 <= z + 1; z2++ )
            {
               // Pixel is within bounding box
               if ( x2 >= 0 && y2 >= 0 & z2 >= 0 && x2 < _pixelFigure.ColCount && y2 < _pixelFigure.RowCount && z2 < _pixelFigure.DepthCount )
               {
                  var equalAmount = 0;
                  equalAmount += x2 == x ? 1 : 0;
                  equalAmount += y2 == y ? 1 : 0;
                  equalAmount += z2 == z ? 1 : 0;

                  // Pixel is not diagonal
                  if ( equalAmount >= 2 )
                  {
                     var correctPixel = pixels.Where( p => p.XIndex == x2 && p.YIndex == y2 && p.ZIndex == z2 );
                     if ( correctPixel.Count() == 1 )
                     {
                        surroundingPixels.Add( correctPixel.First() );
                     }
                  }
               }
            }
         }
      }

      return surroundingPixels;
   }

   public void RemoveFromPlaceablePixels( PixelConfig pixel )
   {
      _placeablePixels.Remove( pixel );
   }

   public void Undo()
   {
      _actionStack.Undo();
   }

   public void Redo()
   {
      _actionStack.Redo();
   }

   public bool CanUndo()
   {
      return _actionStack.CanUndo();
   }

   public bool CanRedo()
   {
      return _actionStack.CanRedo();
   }
}