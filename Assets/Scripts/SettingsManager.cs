using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
      if ( !UnityHelper.IsUnity() )
      {
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
      }
   }

   public static void SetSetting( StringSetting key, string value )
   {
      if ( !UnityHelper.IsUnity() )
      {
         _strings[(int)key] = value;
         SaveSettings();
      }
      else
      {

      }
   }

   public static void SetSetting( BoolSetting key, bool value )
   {
      if ( !UnityHelper.IsUnity() )
      {
         _bools[(int)key] = value;
         SaveSettings();
      }
      else
      {

      }
   }

   public static void SetSetting( IntSetting key, int value )
   {
      if ( !UnityHelper.IsUnity() )
      {
         _ints[(int)key] = value;
         SaveSettings();
      }
      else
      {

      }
   }

   public static string GetSetting( StringSetting key )
   {
      if ( !UnityHelper.IsUnity() )
      {
         return _strings[(int)key];
      }
      else
      {
         return string.Empty;
      }
   }

   public static bool GetSetting( BoolSetting key )
   {
      if ( !UnityHelper.IsUnity() )
      {
         return _bools[(int)key];
      }
      else
      {
         return false;
      }
   }

   public static int GetSetting( IntSetting key )
   {
      if ( !UnityHelper.IsUnity() )
      {
         return _ints[(int)key];
      }
      else
      {
         return 0;
      }
   }

   private static void SaveSettings()
   {
      var lines = _strings.ToList();
      lines.Add( "~" );
      lines.AddRange(_bools.Select(b => b.ToString()));
      lines.Add( "~" );
      lines.AddRange(_ints.Select(i => i.ToString()));

      File.WriteAllLines( _settingsFile, lines.ToArray() );
   }
}