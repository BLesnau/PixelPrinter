using System.IO;
using System.Linq;

namespace NativePlugin
{
    public static class PixelPrinterPlugin
    {
      private static readonly string _settingsFile = Path.Combine( @"C:\Users\lesna\AppData\Local\Packages\b65e477c-6255-4ac2-9130-228d0e221b1a_a5p0ghy48hknt\LocalState", "user.txt" );

      public static string GetAuthToken()
      {
         return GetLineOfSettingsFile( 1 );
      }

      private static string GetLineOfSettingsFile( int lineIndex )
      {
         if ( File.Exists( _settingsFile ) )
         {
            var fileText = File.ReadAllLines( _settingsFile ).ToList();
            if ( fileText.Count() >= 2 )
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
