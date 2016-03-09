using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public GameObject ToolSelectBackground;
   public GameObject ColorSelectBackground;

   public enum Buttons { Add, Remove, Change, Color1, Color2, Color3, Color4, Color5 }
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
      var toolClicked = false;
      var colorClicked = false;

      switch ( button )
      {
         case Buttons.Add:
         {
            SelectedTool = Tools.Add;
            toolClicked = true;
            break;
         }
         case Buttons.Remove:
         {
            SelectedTool = Tools.Remove;
            toolClicked = true;
            break;
         }
         case Buttons.Change:
         {
            SelectedTool = Tools.Change;
            toolClicked = true;
            break;
         }
         case Buttons.Color1:
         {
            colorClicked = true;
            break;
         }
         case Buttons.Color2:
         {
            colorClicked = true;
            break;
         }
         case Buttons.Color3:
         {
            colorClicked = true;
            break;
         }
         case Buttons.Color4:
         {
            colorClicked = true;
            break;
         }
         case Buttons.Color5:
         {
            colorClicked = true;
            break;
         }
      }

      if ( toolClicked )
      {
         var args = new Hashtable() { { "position", trans.position }, { "time", 1 } };
         iTween.MoveTo( ToolSelectBackground, args );
      }

      if ( colorClicked )
      {
         var args = new Hashtable() { { "position", trans.position }, { "time", 1 } };
         iTween.MoveTo( ColorSelectBackground, args );
      }
   }
}
