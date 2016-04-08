using System;
using System.Collections;
using System.Linq;
using NativePlugin;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   public PixelManager PixelManager;

   public GameObject ToolSelectBackground;
   public GameObject ColorSelectBackground;
   public ColorPicker ColorPicker;
   public ColorButton[] ColorButtons;
   public Button UndoButton;
   public Button RedoButton;

   public enum Buttons
   {
      Add, Remove, Change,
      Color1, Color2, Color3, Color4, Color5,
      ColorSelect1, ColorSelect2, ColorSelect3, ColorSelect4, ColorSelect5,
      Undo, Redo,
      Close
   }

   public enum Tools { Add, Remove, Change }
   public enum ColorSelect { Color1, Color2, Color3, Color4, Color5 }

   public Tools SelectedTool = Tools.Add;
   public ColorSelect SelectedColor = ColorSelect.Color1;

   private Color[] Colors = { Color.white, Color.red, new Color( .13f, .69f, .3f ), new Color( 0, .64f, .91f ), Color.black };

   void Start()
   {
      for ( int i = 0; i < ColorButtons.Count(); i++ )
      {
         ColorButtons[i].SetColor( Colors[i] );
      }
   }

   void Update()
   {
      UpdateButtonStates();
   }

   public void ButtonClicked( Transform trans, Buttons button )
   {
      var toolClicked = false;
      var colorClicked = false;
      var colorSelectClickedIndex = -1;

      switch ( button )
      {
         case Buttons.Add:
         {
            var plugin = new PixelPrinterPlugin();
            var authToken = string.Empty;

            if ( WSAComm.LoginCallback != null )
            {
               DebugHelper.Log( "WSA", WSAComm.LoginCallback() );
            }

#if UNITY_EDITOR
            authToken = plugin.GetAuthToken();
#endif
#if UNITY_WSA && !UNITY_EDITOR
            try
            {
               UnityEngine.WSA.Application.InvokeOnUIThread( () =>
               {
                  plugin.GetAuthToken();
               }, false );
            }
            catch ( Exception ex )
            {
               DebugHelper.Log( "Exception", ex.Message );
            }
#endif

            DebugHelper.Log( "Auth Token", authToken );

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
            SelectedColor = ColorSelect.Color1;
            colorClicked = true;
            break;
         }
         case Buttons.Color2:
         {
            SelectedColor = ColorSelect.Color2;
            colorClicked = true;
            break;
         }
         case Buttons.Color3:
         {
            SelectedColor = ColorSelect.Color3;
            colorClicked = true;
            break;
         }
         case Buttons.Color4:
         {
            SelectedColor = ColorSelect.Color4;
            colorClicked = true;
            break;
         }
         case Buttons.Color5:
         {
            SelectedColor = ColorSelect.Color5;
            colorClicked = true;
            break;
         }
         case Buttons.ColorSelect1:
         {
            SelectedColor = ColorSelect.Color1;
            colorSelectClickedIndex = 0;
            break;
         }
         case Buttons.ColorSelect2:
         {
            SelectedColor = ColorSelect.Color2;
            colorSelectClickedIndex = 1;
            break;
         }
         case Buttons.ColorSelect3:
         {
            SelectedColor = ColorSelect.Color3;
            colorSelectClickedIndex = 2;
            break;
         }
         case Buttons.ColorSelect4:
         {
            SelectedColor = ColorSelect.Color4;
            colorSelectClickedIndex = 3;
            break;
         }
         case Buttons.ColorSelect5:
         {
            SelectedColor = ColorSelect.Color5;
            colorSelectClickedIndex = 4;
            break;
         }
         case Buttons.Undo:
         {
            PixelManager.Undo();
            break;
         }
         case Buttons.Redo:
         {
            PixelManager.Redo();
            break;
         }
         case Buttons.Close:
         {
            ColorPicker.Hide();
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

         ColorPicker.SetColor( GetSelectedColor() );
      }

      if ( colorSelectClickedIndex >= 0 )
      {
         var args = new Hashtable() { { "position", ColorButtons[colorSelectClickedIndex].transform.position }, { "time", 1 } };
         iTween.MoveTo( ColorSelectBackground, args );

         ColorPicker.Show( GetSelectedColor() );
      }
   }

   public Color GetSelectedColor()
   {
      return Colors[Convert.ToInt16( SelectedColor )];
   }

   public void SetSelectedColor( Color color )
   {
      var colorIndex = Convert.ToInt16( SelectedColor );
      Colors[colorIndex] = color;
      ColorButtons[colorIndex].SetColor( color );
   }

   private void UpdateButtonStates()
   {
      UndoButton.GetComponent<Button>().interactable = PixelManager.CanUndo();
      RedoButton.GetComponent<Button>().interactable = PixelManager.CanRedo();
   }
}
