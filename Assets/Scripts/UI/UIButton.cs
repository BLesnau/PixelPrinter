using UnityEngine;

public class UIButton : MonoBehaviour
{
   public UIManager UIManager;
   public UIManager.Buttons ButtonType;

   void Start()
   {
   }

   void Update()
   {
   }

   public void Click()
   {
      UIManager.ButtonClicked( transform, ButtonType );
   }
}