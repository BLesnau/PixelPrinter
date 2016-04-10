using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NativePlugin
{
   public class PixelPrinterPlugin
   {
      public async Task<string> GetAuthToken()
      {
         return "Universal Windows Auth Token";
      }
   }
}
