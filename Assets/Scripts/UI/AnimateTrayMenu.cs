using UnityEngine;

public class AnimateTrayMenu : MonoBehaviour
{
   public Animator TrayAnimator;

   private bool _isTrayOut = false;

   public void ToggleTray()
   {
      _isTrayOut = !_isTrayOut;

      if ( TrayAnimator )
      {
         if ( _isTrayOut )
         {
            TrayAnimator.SetTrigger("SlideOut");
         }
         else
         {
            TrayAnimator.SetTrigger( "SlideIn" );            
         }
      }
   }
}
