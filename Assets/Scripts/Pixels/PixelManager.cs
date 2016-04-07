using System.Collections.Generic;
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

   private PixelConfig[,,] _pixels = null;
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
      //_audio = GetComponent<AudioSource>();

      _pixels = new PixelConfig[ColCount, RowCount, DepthCount];
      _placeablePixels = new List<PixelConfig>();

      var startZ = -1 * (((DepthCount * PixelScale) / 2.0f) - (PixelScale / 2.0f));
      var startX = -1 * (((ColCount * PixelScale) / 2.0f) - (PixelScale / 2.0f));
      var startY = -1 * (((RowCount * PixelScale) / 2.0f) - (PixelScale / 2.0f));

      if ( PixelPrefab )
      {
         for ( int y = 0; y < RowCount; y++ )
         {
            var itemsLeft = (ColCount) * (DepthCount);
            int x = 0;
            int z = 0;
            while ( itemsLeft > 0 )
            {
               int xStartIndex = x;
               int zStartIndex = z;

               while ( x < ColCount - xStartIndex )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  x++;
                  itemsLeft--;
               }
               x--;

               if ( itemsLeft <= 0 )
               {
                  break;
               }

               z++;
               while ( z < DepthCount - zStartIndex )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  z++;
                  itemsLeft--;
               }
               z--;

               if ( itemsLeft <= 0 )
               {
                  break;
               }

               x--;
               while ( x >= xStartIndex )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  x--;
                  itemsLeft--;
               }
               x++;

               if ( itemsLeft <= 0 )
               {
                  break;
               }

               z--;
               while ( z >= zStartIndex + 1 )
               {
                  AddPixelToPopIn( x, y, z, startX, startY, startZ );
                  z--;
                  itemsLeft--;
               }

               x = xStartIndex + 1;
               z = zStartIndex + 1;
            }
         }
      }

      _pixels[ColCount / 2, RowCount / 2, DepthCount / 2].Color = Color.red;

      //if ( !animatePopIn )
      //{
      for ( var x = 0; x < ColCount; x++ )
      {
         for ( var y = 0; y < RowCount; y++ )
         {
            for ( var z = 0; z < DepthCount; z++ )
            {
               if ( _pixels[x, y, z].Color.a > 0 )
               {
                  PopIn( _pixels[x, y, z] );
               }
            }
         }
      }
      //}

      DetectPlaceablePixels();
   }

   private void AddPixelToPopIn( int x, int y, int z, float startX, float startY, float startZ )
   {
      var pixelConfig = new PixelConfig()
      {
         XIndex = x,
         YIndex = y,
         ZIndex = z,
         Position = new Vector3( startX + (x * PixelScale), startY + (y * PixelScale), startZ + (z * PixelScale) ),
         Color = new Color( 0, 0, 0, 0 ),
         Prefab = null
      };

      _pixels[x, y, z] = pixelConfig;
   }

   private void Update()
   {
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
         for ( var x = 0; x < ColCount; x++ )
         {
            for ( var y = 0; y < RowCount; y++ )
            {
               for ( var z = 0; z < DepthCount; z++ )
               {
                  // Current pixel is visible
                  if ( _pixels[x, y, z].Color.a > 0 )
                  {
                     //Loop through all surrounding pixels
                     for ( var x2 = x - 1; x2 <= x + 1; x2++ )
                     {
                        for ( var y2 = y - 1; y2 <= y + 1; y2++ )
                        {
                           for ( var z2 = z - 1; z2 <= z + 1; z2++ )
                           {
                              // Pixel is within bounding box
                              if ( x2 >= 0 && y2 >= 0 & z2 >= 0 && x2 < ColCount && y2 < RowCount && z2 < DepthCount )
                              {
                                 var equalAmount = 0;
                                 equalAmount += x2 == x ? 1 : 0;
                                 equalAmount += y2 == y ? 1 : 0;
                                 equalAmount += z2 == z ? 1 : 0;

                                 // Pixel is not diagonal
                                 if ( equalAmount >= 2 )
                                 {
                                    var pixel = _pixels[x2, y2, z2];

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

   private PixelConfig GetPixelConfigFromPrefab( Pixel prefab )
   {
      return _pixels.Cast<PixelConfig>().FirstOrDefault( p => p.Prefab == prefab );
   }

   private IEnumerable<PixelConfig> ConvertToPixels( RaycastHit[] hits )
   {
      var hitList = hits.ToList();
      return hitList.Select( h =>
      {
         var pixel = h.transform.gameObject.GetComponent<Pixel>();
         return GetPixelConfigFromPrefab( pixel );
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
               if ( x2 >= 0 && y2 >= 0 & z2 >= 0 && x2 < ColCount && y2 < RowCount && z2 < DepthCount )
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