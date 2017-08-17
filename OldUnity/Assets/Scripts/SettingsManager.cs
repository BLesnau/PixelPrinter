using System;
using UnityEngine;

public static class SettingsManager
{
   public enum StringSetting { UserId, AuthToken }
   public enum BoolSetting { TestBool }
   public enum IntSetting { TestInt1, TestInt2 }

   public static void SetSetting( StringSetting key, string value )
   {
      PlayerPrefs.SetString( Enum.GetName( key.GetType(), key ), value );
      PlayerPrefs.Save();
   }

   public static void SetSetting( BoolSetting key, bool value )
   {
      PlayerPrefs.SetInt( Enum.GetName( key.GetType(), key ), Convert.ToInt16( value ) );
      PlayerPrefs.Save();
   }

   public static void SetSetting( IntSetting key, int value )
   {
      PlayerPrefs.SetInt( Enum.GetName( key.GetType(), key ), value );
      PlayerPrefs.Save();
   }

   public static string GetSetting( StringSetting key )
   {
      DebugHelper.Log( "Key Name", Enum.GetName( key.GetType(), key ) );
      DebugHelper.Log( "Value", PlayerPrefs.GetString( Enum.GetName( key.GetType(), key ) ) );
      return PlayerPrefs.GetString( Enum.GetName( key.GetType(), key ) );
   }

   public static bool GetSetting( BoolSetting key )
   {
      return Convert.ToBoolean( PlayerPrefs.GetInt( Enum.GetName( key.GetType(), key ) ) );
   }

   public static int GetSetting( IntSetting key )
   {
      return PlayerPrefs.GetInt( Enum.GetName( key.GetType(), key ) );
   }
}