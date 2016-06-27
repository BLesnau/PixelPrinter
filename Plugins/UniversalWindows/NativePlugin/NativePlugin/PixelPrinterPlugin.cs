using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace NativePlugin
{
   public static class PixelPrinterPlugin
   {
      public enum TargetEnvironment { Local, Live };

      public static string GetServiceUrl( TargetEnvironment env = TargetEnvironment.Live )
      {
         switch ( env )
         {
            case TargetEnvironment.Local:
            {
               return "http://desktop-or80phq/PixelPrinterService";
            }
            case TargetEnvironment.Live:
            default:
            {
               return "https://pixelprinter.azurewebsites.net";
            }
         }
      }

      public static async Task<string> GetAuthToken( TargetEnvironment env = TargetEnvironment.Live )
      {
         try
         {
            var mobileService = new MobileServiceClient( GetServiceUrl( env ) );
            mobileService.AlternateLoginHost = new Uri( GetServiceUrl( TargetEnvironment.Live ) );
            var user = await mobileService.LoginAsync( MobileServiceAuthenticationProvider.Google );

            return user.MobileServiceAuthenticationToken;
            //return string.Join( ",", new string[] { user.MobileServiceAuthenticationToken, user.UserId } );
         }
         catch ( Exception ex )
         {
            return null;
         }
      }
   }
}
