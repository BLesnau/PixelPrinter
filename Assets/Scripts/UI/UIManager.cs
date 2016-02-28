using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public GameObject SelectBackground;

   void Start()
   {

   }

   void Update()
   {
   }

   public void ButtonClicked( Transform trans )
   {
      var args = new Hashtable() { { "position", trans.position }, { "time", 1 } };
      iTween.MoveTo( SelectBackground, args );
   }
}
