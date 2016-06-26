using System;
using System.Text;
using Newtonsoft.Json.Linq;
using RestSharp;
#if UNITY_EDITOR || UNITY_WSA
using NativePlugin;
#endif

public class AzureHelper
{
   private static string _serviceUrl =
      "http://desktop-or80phq/PixelPrinterService";
   //"https://pixelprinter.azurewebsites.net";

   public static void Login( ILoginListener loginListener )
   {
      var authToken = string.Empty;

      try
      {
         CheckTokenExpiration();
      }
      catch ( AuthenticationExpiredException )
      {
         if ( UnityHelper.IsEditor() )
         {
#if UNITY_EDITOR
            authToken = PixelPrinterPlugin.GetAuthToken();
            //DebugHelper.Log( "Auth Token", authToken );
            loginListener.LoginCompleted( true );
#endif
         }
         else if ( UnityHelper.IsUWP() )
         {
#if IS_UWP
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
         else
         {
            authToken = SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken );
         }
      }
   }

   public static User GetUser()
   {
      CheckTokenExpiration();

      var client = new RestClient( _serviceUrl );

      var request = new RestRequest( "tables/User", Method.GET ) { RequestFormat = DataFormat.Json };

      request.AddHeader( "X-ZUMO-AUTH", SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken ) );

      var user = new User();
      bool threadDone = false;
      bool gotUser = false;

      client.ExecuteAsync( request, response =>
      {
         gotUser = user.Deserialize( response.Content );
         threadDone = true;
      } );

      while ( !threadDone )
      { }

      if ( gotUser )
      {
         return user;
      }

      throw new AzureErrorException();
   }

   private static void CheckTokenExpiration()
   {
      if ( IsTokenExpired( SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken ) ) )
      {
         throw new AuthenticationExpiredException();
      }
   }

   private static JObject GetJToken( string token )
   {
      var jwt = token.Split( new Char[] { '.' } )[1];

      // Undo the URL encoding.
      jwt = jwt.Replace( '-', '+' );
      jwt = jwt.Replace( '_', '/' );
      switch ( jwt.Length % 4 )
      {
         case 0:
         {
            break;
         }
         case 2:
         {
            jwt += "==";
            break;
         }
         case 3:
         {
            jwt += "=";

            break;
         }
         default:
         {
            //throw new System.Exception(
            //   "The base64url string is not valid." );
            break;
         }
      }

      // Decode the bytes from base64 and write to a JSON string.
      var bytes = Convert.FromBase64String( jwt );
      string jsonString = UTF8Encoding.UTF8.GetString( bytes, 0, bytes.Length );

      // Parse as JSON object and get the exp field value, 
      // which is the expiration date as a JavaScript primative date.
      return JObject.Parse( jsonString );
   }

   private static bool IsTokenExpired( string token )
   {
      try
      {
         // Parse as JSON object and get the exp field value, 
         // which is the expiration date as a JavaScript primative date.
         var jsonObj = GetJToken( token );
         var exp = Convert.ToDouble( jsonObj["exp"].ToString() );

         // Calculate the expiration by adding the exp value (in seconds) to the 
         // base date of 1/1/1970.
         DateTime minTime = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
         var expire = minTime.AddSeconds( exp );

         // If the expiration date is less than now, the token is expired and we return true.
         return expire < DateTime.UtcNow ? true : false;
      }
      catch
      {
         return true;
      }
   }
}
