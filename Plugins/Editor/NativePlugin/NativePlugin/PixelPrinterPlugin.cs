using System.IO;
using System.Linq;

namespace NativePlugin
{
   public static class PixelPrinterPlugin
   {
      private static readonly string _settingsFile = Path.Combine( @"C:\Users\lesna\Documents", "user.txt" );

      public static string GetAuthToken()
      {
         return GetLineOfSettingsFile( 0 );
      }

      private static string GetLineOfSettingsFile( int lineIndex )
      {
         if ( File.Exists( _settingsFile ) )
         {
            var fileText = File.ReadAllLines( _settingsFile ).ToList();
            if ( fileText.Count() > lineIndex )
            {
               if ( !string.IsNullOrEmpty( fileText[lineIndex] ) )
               {
                  return fileText[lineIndex];
               }
            }
         }

         return null;
      }
   }
}
