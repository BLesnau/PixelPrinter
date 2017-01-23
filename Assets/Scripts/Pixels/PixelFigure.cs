using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using JsonSerializer = RestSharp.Serializers.JsonSerializer;

//[JsonObject]
public class PixelFigure
{
   private int _colCount = 0;
   private int _rowCount = 0;
   private int _depthCount = 0;

   private Guid _id;
   private readonly PixelConfig[,,] _pixels;

   public PixelFigure( int c, int r, int d, Guid id )
   {
      _colCount = c;
      _rowCount = r;
      _depthCount = d;

      _id = id;
      _pixels = new PixelConfig[c, r, d];
   }

   public PixelConfig GetPixel( int c, int r, int d )
   {
      return _pixels[c, r, d];
   }

   public void SetPixel( int c, int r, int d, PixelConfig pixel )
   {
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
            var stream = new StreamWriter( file );
            stream.Write( _colCount );
            stream.Write( ' ' );
            stream.Write( _rowCount );
            stream.Write( ' ' );
            stream.Write( _depthCount );
            stream.Write( ' ' );

            for ( var x = 0; x < _colCount; x++ )
            {
               for ( var y = 0; y < _rowCount; y++ )
               {
                  for ( var z = 0; z < _depthCount; z++ )
                  {
                     stream.Write( _pixels[x, y, z].Color );
                     stream.Write( ' ' );
                  }
               }
            }
         }
      }
   }
}