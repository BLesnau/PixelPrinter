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

      if ( _placePixel.IsActive() )
      {
         var hits = Physics.RaycastAll( ray );
         Pixel closestPixel = null;
         var closestDistance = 0f;
         foreach ( var h in hits )
         {
            var pixel = h.transform.gameObject.GetComponent<Pixel>();
            var config = GetPixelConfigFromPrefab( pixel );
            if ( pixel != null && config != null && config.color.a > 0 )
            {
               var distance = Vector3.Distance( pixel.transform.position, Camera.main.transform.position );

               if ( closestPixel == null || distance < closestDistance )
               {
                  closestPixel = pixel;
                  closestDistance = distance;
               }
               else
               {
                  if ( distance < closestDistance )
                  {
                     closestPixel = pixel;
                     closestDistance = distance;
                  }
               }
            }
         }

         if ( closestPixel != null )
         {
            var pix = GetPixelConfigFromPrefab( closestPixel );

            closestPixel = null;
            closestDistance = 0f;

            var x = pix.xIndex;
            var y = pix.yIndex;
            var z = pix.zIndex;

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
                           foreach ( var h in hits )
                           {
                              var hitPixel = h.transform.gameObject.GetComponent<Pixel>();
                              var config = GetPixelConfigFromPrefab( hitPixel );
                              if ( hitPixel != null && config.xIndex == x2 && config.yIndex == y2 && config.zIndex == z2 )
                              {
                                 var distance = Vector3.Distance( hitPixel.transform.position, Camera.main.transform.position );

                                 if ( closestPixel == null || distance < closestDistance )
                                 {
                                    closestPixel = hitPixel;
                                    closestDistance = distance;
                                 }
                                 else
                                 {
                                    if ( distance < closestDistance )
                                    {
                                       closestPixel = hitPixel;
                                       closestDistance = distance;
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
         else
         {
            foreach ( var h in hits )
            {
               var pixel = h.transform.gameObject.GetComponent<Pixel>();
               if ( pixel != null )
               {
                  var distance = Vector3.Distance( pixel.transform.position, Camera.main.transform.position );

                  if ( closestPixel == null || distance < closestDistance )
                  {
                     closestPixel = pixel;
                     closestDistance = distance;
                  }
                  else
                  {
                     if ( distance < closestDistance )
                     {
                        closestPixel = pixel;
                        closestDistance = distance;
                     }
                  }
               }
            }
         }

         var pixelConfig = GetPlaceablePixelConfigFromPrefab( closestPixel );
         if ( pixelConfig != null )
         {
            pixelConfig.color = new Color( Random.value, Random.value, Random.value, 1 );
            placeablePixels.Remove( pixelConfig );
            DetectPlaceablePixels();
         }
      }
   }

   private void PopIn( PixelConfig config )
   {
      var tmpEuler = transform.parent.localRotation.eulerAngles;
      var tmpPostion = transform.parent.position;
      transform.parent.localRotation = Quaternion.identity;
      transform.parent.position = new Vector3( 0, 0, 0 );

      var pixel = (Pixel)Instantiate( pixelPrefab, config.position, Quaternion.identity );
      if ( pixel )
      {
         pixel.Color = config.color;
         pixel.transform.localScale = new Vector3( pixelScale, pixelScale, pixelScale );
         config.prefab = pixel;

         pixel.transform.parent = this.transform;
      }

      transform.parent.localRotation = Quaternion.Euler( tmpEuler );
      transform.parent.position = tmpPostion;
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

   private PixelConfig GetPlaceablePixelConfigFromPrefab( Pixel prefab )
   {
      try
      {
         return placeablePixels.First( p => p.prefab == prefab );
      }
      catch ( Exception ) { }

      return null;
   }

   private PixelConfig GetPixelConfigFromPrefab( Pixel prefab )
   {
      foreach ( var p in _pixels )
      {
         if ( p.prefab == prefab )
         {
            return p;
         }
      }

      return null;
   }
}
