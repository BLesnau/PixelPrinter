using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public GameObject SelectBackground;

   public enum Buttons { Add, Remove, Change }
   public enum Tools { Add, Remove, Change }

   public Tools SelectedTool;

   void Start()
   {
      SelectedTool = Tools.Add;
   }

   void Update()
   {
   }

   public void ButtonClicked( Transform trans, Buttons button )
   {
      switch ( button )
      {
         case Buttons.Add:
         {
            SelectedTool = Tools.Add;
            break;
         }
         case Buttons.Remove:
         {
            SelectedTool = Tools.Remove;
            break;
         }
         case Buttons.Change:
         {
            SelectedTool = Tools.Change;
            break;
         }
      }

      var args = new Hashtable() { { "position", trans.position }, { "time", 1 } };
      iTween.MoveTo( SelectBackground, args );
   }
}
