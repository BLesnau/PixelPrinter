using UnityEngine;

public class ChangeAction : IEditAction
{
   private readonly PixelManager _pixelManager;
   private readonly PixelConfig _pixel;
   private readonly Color _originalColor;
   private readonly Color _newColor;

   public ChangeAction( PixelManager pixelManager, PixelConfig pixel, Color newColor )
   {
      _pixelManager = pixelManager;
      _pixel = pixel;
      _originalColor = pixel.Color;
      _newColor = newColor;
   }

   public void Undo()
   {
      _pixel.Color = _originalColor;
   }

   public void Redo()
   {
      _pixel.Color = _newColor;
   }
}