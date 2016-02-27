using UnityEngine;

public class UIManager : MonoBehaviour
{
   public Transform SelectBackground;

   void Start()
   {

   }

   void Update()
   {
   }

   public void ButtonClicked( Transform trans )
   {
      SelectBackground.position = trans.position;
   }
}
