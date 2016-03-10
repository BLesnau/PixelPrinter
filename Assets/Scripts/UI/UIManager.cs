﻿using System;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public GameObject ToolSelectBackground;
   public GameObject ColorSelectBackground;
   public ColorPicker ColorPicker;

   public enum Buttons
   {
      Add, Remove, Change,
      Color1, Color2, Color3, Color4, Color5,
      ColorSelect1, ColorSelect2, ColorSelect3, ColorSelect4, ColorSelect5,
      Close
   }

   public enum Tools { Add, Remove, Change }
   public enum Color { Color1, Color2, Color3, Color4, Color5, }

   public Tools SelectedTool = Tools.Add;
   public Color SelectedColor = Color.Color1;

   void Start()
   {
      ColorPicker.Hide();
   }

   void Update()
   {
   }

   public void ButtonClicked( Transform trans, Buttons button )
   {
      var toolClicked = false;
      var colorClicked = false;
      var colorSelectClicked = false;

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
            SelectedColor = Color.Color1;
            colorClicked = true;
            break;
         }
         case Buttons.Color2:
         {
            SelectedColor = Color.Color2;
            colorClicked = true;
            break;
         }
         case Buttons.Color3:
         {
            SelectedColor = Color.Color3;
            colorClicked = true;
            break;
         }
         case Buttons.Color4:
         {
            SelectedColor = Color.Color4;
            colorClicked = true;
            break;
         }
         case Buttons.Color5:
         {
            SelectedColor = Color.Color5;
            colorClicked = true;
            break;
         }
         case Buttons.ColorSelect1:
         {
            colorSelectClicked = true;
            break;
         }
         case Buttons.ColorSelect2:
         {
            colorSelectClicked = true;
            break;
         }
         case Buttons.ColorSelect3:
         {
            colorSelectClicked = true;
            break;
         }
         case Buttons.ColorSelect4:
         {
            colorSelectClicked = true;
            break;
         }
         case Buttons.ColorSelect5:
         {
            colorSelectClicked = true;
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
      }

      if ( colorSelectClicked && !ColorPicker.IsVisible() )
      {
         ColorPicker.Show();
      }
   }
}
