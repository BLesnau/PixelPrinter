using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class PixelFigure
{
   public int ColCount = 0;
   public int RowCount = 0;
   public int DepthCount = 0;

   private Guid _id;
   private PixelConfig[,,] _pixels;
   public float PixelScale = 1;

   public static Vector3 GetSize( Guid id )
   {
      var filePath = Path.Combine( Application.persistentDataPath, id.ToString() );

      var allText = File.ReadAllText( filePath ).Split( ' ' );

      if ( allText.Count() >= 3 )
      {
         return new Vector3( Convert.ToInt16( allText[0] ), Convert.ToInt16( allText[1] ), Convert.ToInt16( allText[2] ) );
      }

      return Vector3.zero;
   }

   public PixelFigure( Guid id )
   {
      _id = id;

      var filePath = Path.Combine( Application.persistentDataPath, _id.ToString() );

      var allText = File.ReadAllText( filePath ).Split( ' ' );

      if ( allText.Count() >= 3 )
      {
         var c = Convert.ToInt16( allText[0] );
         var r = Convert.ToInt16( allText[1] );
         var d = Convert.ToInt16( allText[2] );

         Init( c, r, d );

         int currentIndex = 3;

         for ( var x = 0; x < ColCount; x++ )
         {
            for ( var y = 0; y < RowCount; y++ )
            {
               for ( var z = 0; z < DepthCount; z++ )
               {
                  try
                  {
                     var rgba = allText[currentIndex].Split( ',' );
                     _pixels[x, y, z].Color = new Color( float.Parse( rgba[0] ), float.Parse( rgba[1] ), float.Parse( rgba[2] ), float.Parse( rgba[3] ) );
                     currentIndex++;
                  }
                  catch ( IndexOutOfRangeException )
                  {
                     int i = 0;
                     i++;
                  }
               }
            }
         }
      }
   }

   public PixelFigure( int c, int r, int d )
   {
      //_id = Guid.NewGuid();
      _id = new Guid( "6590617e-4287-414f-a6a0-ae2c5d2aecb9" );

      Init( c, r, d );
   }

   public PixelConfig GetPixel( int c, int r, int d )
   {
      return _pixels[c, r, d];
   }

   public void SetPixel( int c, int r, int d, PixelConfig pixel )
   {
      if ( _pixels[c, r, d] != null && _pixels[c, r, d].Prefab != null )
      {
         UnityEngine.Object.Destroy( _pixels[c, r, d].Prefab );
      }
      _pixels[c, r, d] = pixel;
   }

   public PixelConfig GetPixelConfigFromPrefab( Pixel prefab )
   {
      return _pixels.Cast<PixelConfig>().FirstOrDefault( p => p.Prefab == prefab );
   }

   public void SaveState( PixelConfig pixel = null )
   {
      var filePath = Path.Combine( Application.persistentDataPath, _id.ToString() );

      using ( var file = File.Open( filePath, FileMode.Create ) )
      {
         if ( pixel == null )
         {
            using ( var stream = new StreamWriter( file ) )
            {
               stream.Write( ColCount );
               stream.Write( ' ' );
               stream.Write( RowCount );
               stream.Write( ' ' );
               stream.Write( DepthCount );
               stream.Write( ' ' );

               for ( var x = 0; x < ColCount; x++ )
               {
                  for ( var y = 0; y < RowCount; y++ )
                  {
                     for ( var z = 0; z < DepthCount; z++ )
                     {
                        stream.Write( _pixels[x, y, z].Color.r );
                        stream.Write( ',' );
                        stream.Write( _pixels[x, y, z].Color.g );
                        stream.Write( ',' );
                        stream.Write( _pixels[x, y, z].Color.b );
                        stream.Write( ',' );
                        stream.Write( _pixels[x, y, z].Color.a );
                        stream.Write( ' ' );
                     }
                  }
               }
            }
         }
      }
   }

   public void Unload()
   {
      foreach ( var pixel in _pixels )
      {
         pixel.Unload();
      }
   }

   private void Init( int c, int r, int d )
   {
      ColCount = c;
      RowCount = r;
      DepthCount = d;

      _pixels = new PixelConfig[c, r, d];

      InitializePixels();

      //for ( var x = 0; x < ColCount; x++ )
      //{
      //   for ( var y = 0; y < RowCount; y++ )
      //   {
      //      for ( var z = 0; z < DepthCount; z++ )
      //      {
      //         var pixelConfig = new PixelConfig()
      //         {
      //            XIndex = x,
      //            YIndex = y,
      //            ZIndex = z,
      //            Position = new Vector3( startX + (x * PixelScale), startY + (y * PixelScale), startZ + (z * PixelScale) ),
      //            Color = new Color( 0, 0, 0, 0 ),
      //            Prefab = null
      //         };

      //         SetPixel( x, y, z, pixelConfig );
      //      }
      //   }
      //}
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

      SetPixel( x, y, z, pixelConfig );
   }

   private void InitializePixels()
   {
      var startZ = -1 * (((DepthCount * PixelScale) / 2.0f) - (PixelScale / 2.0f));
      var startX = -1 * (((ColCount * PixelScale) / 2.0f) - (PixelScale / 2.0f));
      var startY = -1 * (((RowCount * PixelScale) / 2.0f) - (PixelScale / 2.0f));

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
}