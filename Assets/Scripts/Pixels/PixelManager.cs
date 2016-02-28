using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PixelManager : MonoBehaviour
{
   private class PixelConfig
   {
      public int xIndex;
      public int yIndex;
      public int zIndex;

      public Vector3 position;

      private Color _color;
      public Color color
      {
         get { return _color; }
         set
         {
            _color = value;
            if ( prefab != null )
            {
               prefab.Color = value;
            }
         }
      }

      public Pixel prefab;
   }

   public Pixel pixelPrefab = null;
   public int depthCount = 501;
   public int colCount = 501;
   public int rowCount = 501;
   public float pixelScale = 1;
   public float gridScale = .1f;
   //public float popInDelaySeconds = .1f;
   //public bool animatePopIn = true;

   private PixelConfig[,,] _pixels = null;
   private List<PixelConfig> placeablePixels = null;
   //private int _poppedInCount = 0;
   //private TimeSpan _popInTimeElapsed = TimeSpan.Zero;
   //private AudioSource _audio;

   private readonly ToggleEvent _placePixel = new ToggleEvent( MouseToggleEvent.LeftClick, TouchToggleEvent.OneFingerTap );
   private readonly ValueEvent _pointerPosX = new ValueEvent( MouseValueEvent.XPos, TouchValueEvent.OneFingerXPos );
   private readonly ValueEvent _pointerPosY = new ValueEvent( MouseValueEvent.YPos, TouchValueEvent.OneFingerYPos );

   private void Start()
   {
      //_audio = GetComponent<AudioSource>();

      _pixels = new PixelConfig[colCount, rowCount, depthCount];
      placeablePixels = new List<PixelConfig>();

      var startZ = -1 * ( ( ( depthCount * pixelScale ) / 2.0f ) - ( pixelScale / 2.0f ) );
      var startX = -1 * ( ( ( colCount * pixelScale ) / 2.0f ) - ( pixelScale / 2.0f ) );
      var startY = -1 * ( ( ( rowCount * pixelScale ) / 2.0f ) - ( pixelScale / 2.0f ) );

      if ( pixelPrefab )
      {
         for ( int y = 0; y < rowCount; y++ )
         {
            var itemsLeft = ( colCount ) * ( depthCount );
            int x = 0;
            int z = 0;
            while ( itemsLeft > 0 )
            {
               int xStartIndex = x;
               int zStartIndex = z;

               while ( x < colCount - xStartIndex )
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
               while ( z < depthCount - zStartIndex )
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

      _pixels[colCount / 2, rowCount / 2, depthCount / 2].color = Color.red;

      //if ( !animatePopIn )
      //{
      for ( var x = 0; x < colCount; x++ )
      {
         for ( var y = 0; y < rowCount; y++ )
         {
            for ( var z = 0; z < depthCount; z++ )
            {
               if ( _pixels[x, y, z].color.a > 0 )
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
         xIndex = x,
         yIndex = y,
         zIndex = z,
         position = new Vector3( startX + ( x * pixelScale ), startY + ( y * pixelScale ), startZ + ( z * pixelScale ) ),
         color = new Color( 0, 0, 0, 0 ),
         prefab = null
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
         if ( sortedPixels.Any( p => p.color.a > 0 ) )
         {
            var pix = sortedPixels.First( p => p.color.a > 0 );
            var sortedSurroundingPixels = SortClosestPixels( GetSurroundingPixels( sortedPixels, pix ) );
            try
            {
               selectedPixel = sortedSurroundingPixels.First( p => placeablePixels.Contains( p ) );
            }
            catch { }
         }
         else
         {
            try
            {
               selectedPixel = sortedPixels.First( p => placeablePixels.Contains( p ) );
            }
            catch { }
         }

         if ( selectedPixel != null )
         {
            selectedPixel.color = new Color( Random.value, Random.value, Random.value, 1 );
            placeablePixels.Remove( selectedPixel );
            DetectPlaceablePixels();
         }
      }
   }

   private void PopIn( PixelConfig config )
   {
      var tmpEuler = transform.localRotation.eulerAngles;
      var tmpPostion = transform.position;
      transform.localRotation = Quaternion.identity;
      transform.position = new Vector3( 0, 0, 0 );

      var pixel = (Pixel)Instantiate( pixelPrefab, config.position, Quaternion.identity );
      if ( pixel )
      {
         pixel.Color = config.color;
         pixel.transform.localScale = new Vector3( pixelScale, pixelScale, pixelScale );
         config.prefab = pixel;

         pixel.transform.parent = this.transform;
      }

      transform.localRotation = Quaternion.Euler( tmpEuler );
      transform.position = tmpPostion;
   }

   private void DetectPlaceablePixels()
   {
      var newPlaceablePixels = new List<PixelConfig>();

      for ( var x = 0; x < colCount; x++ )
      {
         for ( var y = 0; y < rowCount; y++ )
         {
            for ( var z = 0; z < depthCount; z++ )
            {
               // Current pixel is visible
               if ( _pixels[x, y, z].color.a > 0 )
               {
                  //Loop through all surrounding pixels
                  for ( var x2 = x - 1; x2 <= x + 1; x2++ )
                  {
                     for ( var y2 = y - 1; y2 <= y + 1; y2++ )
                     {
                        for ( var z2 = z - 1; z2 <= z + 1; z2++ )
                        {
                           // Pixel is within bounding box
                           if ( x2 >= 0 && y2 >= 0 & z2 >= 0 && x2 < colCount && y2 < rowCount && z2 < depthCount )
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
                                 if ( !placeablePixels.Contains( pixel ) )
                                 {
                                    // Pixel is not already visible
                                    if ( pixel.color.a <= 0f )
                                    {
                                       newPlaceablePixels.Add( pixel );
                                       placeablePixels.Add( pixel );
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

      newPlaceablePixels.ForEach( PopIn );
   }

   private PixelConfig GetPixelConfigFromPrefab( Pixel prefab )
   {
      return _pixels.Cast<PixelConfig>().FirstOrDefault( p => p.prefab == prefab );
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
         var distance1 = Vector3.Distance( p1.prefab.transform.position, Camera.main.transform.position );
         var distance2 = Vector3.Distance( p2.prefab.transform.position, Camera.main.transform.position );
         return distance1.CompareTo( distance2 );
      } );

      return sortedList;
   }

   private IEnumerable<PixelConfig> GetSurroundingPixels( IEnumerable<PixelConfig> pixels, PixelConfig pixel )
   {
      var surroundingPixels = new List<PixelConfig>();

      var x = pixel.xIndex;
      var y = pixel.yIndex;
      var z = pixel.zIndex;

      //Loop through all surrounding pixels
      for ( var x2 = x - 1; x2 <= x + 1; x2++ )
      {
         for ( var y2 = y - 1; y2 <= y + 1; y2++ )
         {
            for ( var z2 = z - 1; z2 <= z + 1; z2++ )
            {
               // Pixel is within bounding box
               if ( x2 >= 0 && y2 >= 0 & z2 >= 0 && x2 < colCount && y2 < rowCount && z2 < depthCount )
               {
                  var equalAmount = 0;
                  equalAmount += x2 == x ? 1 : 0;
                  equalAmount += y2 == y ? 1 : 0;
                  equalAmount += z2 == z ? 1 : 0;

                  // Pixel is not diagonal
                  if ( equalAmount >= 2 )
                  {
                     var correctPixel = pixels.Where( p => p.xIndex == x2 && p.yIndex == y2 && p.zIndex == z2 );
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
}
