using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SettingsManager
{
   private static readonly string _settingsFile;
   private static readonly List<string> _strings;
   private static readonly List<bool> _bools;
   private static readonly List<int> _ints;

   public enum StringSetting { UserId, AuthToken }
   public enum BoolSetting { TestBool }
   public enum IntSetting { TestInt1, TestInt2 }

   static SettingsManager()
   {
#if !UNITY_WSA && !UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS
      _settingsFile = Path.Combine( @"C:\Users\lesna\AppData\Local\Packages\b65e477c-6255-4ac2-9130-228d0e221b1a_a5p0ghy48hknt\LocalState", "user.txt" );
      if ( !File.Exists( _settingsFile ) )
      {
         File.Create( _settingsFile ).Close();
      }

      if ( File.Exists( _settingsFile ) )
      {
         _strings = new List<string>( Enum.GetNames( typeof( StringSetting ) ).Count() ) { string.Empty };
         _bools = new List<bool>( Enum.GetNames( typeof( BoolSetting ) ).Count() ) { false };
         _ints = new List<int>( Enum.GetNames( typeof( IntSetting ) ).Count() ) { 0 };
         var lines = File.ReadAllLines( _settingsFile ).ToList();

         int phase = 0;
         int phaseCount = 0;
         foreach ( var line in lines )
         {
            if ( line != "~" )
            {
               if ( phase == 0 )
               {
                  _strings[phaseCount] = line;
               }
               if ( phase == 1 )
               {
                  _bools[phaseCount] = Convert.ToBoolean( line );
               }
               if ( phase == 2 )
               {
                  _ints[phaseCount] = Convert.ToInt16( line );
               }
               phaseCount++;
            }
            else
            {
               phase++;
               phaseCount = 0;
            }
         }
      }
#endif
   }

   public static void SetSetting( StringSetting key, string value )
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      int i = 0;
#else
      _strings[(int)key] = value;
      SaveSettings();
#endif
   }

   public static void SetSetting( BoolSetting key, bool value )
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      int i = 0;
#else
      _bools[(int)key] = value;
      SaveSettings();
#endif
   }

   public static void SetSetting( IntSetting key, int value )
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      int i = 0;
#else
      _ints[(int)key] = value;
      SaveSettings();
#endif
   }

   public static string GetSetting( StringSetting key )
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      return string.Empty;
#else
      return _strings[(int)key];
#endif
   }

   public static bool GetSetting( BoolSetting key )
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      return false;
#else
      return _bools[(int)key];
#endif
   }

   public static int GetSetting( IntSetting key )
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      return 0;
#else
      return _ints[(int)key];
#endif
   }

   private static void SaveSettings()
   {
      var lines = new List<string>();
      foreach ( var s in _strings )
      {
         lines.Add( s );
      }
      lines.Add( "~" );
      foreach ( var b in _bools )
      {
         lines.Add( b.ToString() );
      }
      lines.Add( "~" );
      foreach ( var i in _ints )
      {
         lines.Add( i.ToString() );
      }

      File.WriteAllLines( _settingsFile, lines.ToArray() );
   }
}