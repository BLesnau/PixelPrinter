using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.WindowsAzure.MobileServices;

namespace NativePlugin
{
   public static class PixelPrinterPlugin
   {
      public static async Task<string> GetAuthToken()
      {
         try
         {
            var mobileService = new MobileServiceClient( "https://pixelprinter.azurewebsites.net" );
            var user = await mobileService.LoginAsync( MobileServiceAuthenticationProvider.Google );

            return user.MobileServiceAuthenticationToken;
         }
         catch ( Exception ex )
         {
            return null;
         }
      }
   }
}
