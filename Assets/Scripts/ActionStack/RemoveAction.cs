using UnityEngine;

public class RemoveAction : IEditAction
{
   private readonly PixelManager _pixelManager;
   private readonly PixelConfig _pixel;
   private readonly Color _originalColor;

   public RemoveAction( PixelManager pixelManager, PixelConfig pixel )
   {
      _pixelManager = pixelManager;
      _pixel = pixel;
      _originalColor = pixel.Color;
   }

   public void Undo()
   {
      _pixel.Color = _originalColor;
      _pixelManager.RemoveFromPlaceablePixels( _pixel );
      _pixelManager.DetectPlaceablePixels();     
   }

   public void Redo()
   {
      _pixelManager.PopOut( _pixel );
      _pixelManager.DetectPlaceablePixels();
   }
}