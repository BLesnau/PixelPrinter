using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PixelFigure
{
   private int DepthCount = 0;
   private int ColCount = 0;
   private int RowCount = 0;

   private PixelConfig[,,] _pixels = null;

   public PixelFigure( int c, int r, int d )
   {
      _pixels = new PixelConfig[c, r, d];
      ColCount = c;
      RowCount = r;
      DepthCount = d;
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
}
