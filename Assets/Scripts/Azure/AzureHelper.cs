using System;
using NativePlugin;

public class AzureHelper
{
   public static void Login( ILoginListener loginListener )
   {
      var authToken = string.Empty;

#if UNITY_EDITOR
      authToken = PixelPrinterPlugin.GetAuthToken();
      //DebugHelper.Log( "Auth Token", authToken );
      loginListener.LoginCompleted( true );
#endif
#if UNITY_WSA && !UNITY_EDITOR
      try
      {
         UnityEngine.WSA.Application.InvokeOnUIThread( async () =>
         {
            authToken = await PixelPrinterPlugin.GetAuthToken();
            //DebugHelper.Log( "Auth Token", authToken );
            loginListener.LoginCompleted( !string.IsNullOrEmpty( authToken ) ? true : false );
         }, true );
      }
      catch ( Exception ex )
      {
         DebugHelper.Log( "Exception", ex.Message );
      }
#endif
   }
}
