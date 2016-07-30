using UnityEngine;

public class HideableUIElement : MonoBehaviour
{
   public CanvasGroup CanvasGroup;

   private Vector3 _originalPosition;

   private void Start()
   {
      _originalPosition = transform.localPosition;
   }

   public void Hide()
   {
      transform.localPosition = _originalPosition;
      ToggleVisibility( false );
   }

   public void Show()
   {
      transform.localPosition = new Vector3( 0, 0, _originalPosition.z );
      ToggleVisibility( true );
   }

   public bool IsVisible()
   {
      return this.enabled;
   }

   private void ToggleVisibility( bool isVisible )
   {
      this.enabled = isVisible;
      CanvasGroup.alpha = isVisible ? 1 : 0;
      CanvasGroup.blocksRaycasts = isVisible;
      CanvasGroup.interactable = isVisible;
   }

   public static void Hide( GameObject obj )
   {
      var s = obj.GetComponent<HideableUIElement>();
      obj.GetComponent<HideableUIElement>().Hide();
   }

   public static void Show( GameObject obj )
   {
      obj.GetComponent<HideableUIElement>().Show();
   }
}
