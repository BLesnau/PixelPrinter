using UnityEngine;

public class HideableUIElement : MonoBehaviour
{
   public CanvasGroup CanvasGroup;
   public UIManager UIManager;
   public bool StartHidden = true;
   public bool Modal = true;

   private Vector3 _originalPosition;
   private float _originalAlpha;

   private void Start()
   {
      _originalPosition = transform.localPosition;
      _originalAlpha = CanvasGroup.alpha;

      if ( StartHidden )
      {
         Hide();
      }
      else
      {
         Show();
      }
   }

   public void Hide()
   {
      transform.localPosition = _originalPosition;
      ToggleVisibility( false );

      if ( Modal )
      {
         UIManager.StopModal();
      }
   }

   public void Show()
   {
      if ( Modal )
      {
         UIManager.StartModal();
      }

      transform.localPosition = new Vector3( 0, 0, _originalPosition.z );
      transform.SetAsLastSibling();
      ToggleVisibility( true );
   }

   public bool IsVisible()
   {
      return this.enabled;
   }

   private void ToggleVisibility( bool isVisible )
   {
      this.enabled = isVisible;
      CanvasGroup.alpha = isVisible ? _originalAlpha : 0;
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
