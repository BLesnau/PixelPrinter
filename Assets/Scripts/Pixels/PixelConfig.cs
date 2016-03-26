using UnityEngine;

public class PixelConfig
{
   public int XIndex;
   public int YIndex;
   public int ZIndex;

   public Vector3 Position;

   private Color _color;
   public Color Color
   {
      get { return _color; }
      set
      {
         _color = value;
         if ( Prefab != null )
         {
            Prefab.Color = value;
         }
      }
   }

   public Pixel Prefab;
}