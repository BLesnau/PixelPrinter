﻿using System;
using System.Linq;
using System.Net;
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

   private static ILoginListener _loginListener;

   public static void SetListener( ILoginListener loginListener )
   {
      _loginListener = loginListener;
   }

   public static void Login()
   {
      var authToken = SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken );

      if ( IsLoggedIn() )
      {
         FinishLogin( authToken );
         return;
      }

      try
      {
         CheckTokenExpiration( authToken );
      }
      catch ( AuthenticationExpiredException )
      {
         if ( UnityHelper.IsEditor() )
         {
#if UNITY_EDITOR
            authToken = PixelPrinterPlugin.GetAuthToken();
            //DebugHelper.Log( "Auth Token", authToken );
            FinishLogin( authToken );
#endif
         }
         else if ( UnityHelper.IsUWP() )
         {
#if UNITY_WSA && !UNITY_EDITOR
            try
            {
               UnityEngine.WSA.Application.InvokeOnUIThread( async () =>
               {
                  authToken = await PixelPrinterPlugin.GetAuthToken( PixelPrinterPlugin.TargetEnvironment.Local );
                  //DebugHelper.Log( "Auth Token", authToken );
                  UnityEngine.WSA.Application.InvokeOnAppThread( async () =>
                  {
                     FinishLogin( authToken );
                  }, true );
               }, true );
            }
            catch ( Exception ex )
            {
               DebugHelper.Log( "Exception", ex.Message );
            }
#endif
         }
      }
   }

   public static bool IsLoggedIn()
   {
      try
      {
         CheckTokenExpiration( SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken ) );
         return true;
      }
      catch { }

      return false;
   }

   public static User GetUser()
   {
      try
      {
         if ( !IsLoggedIn() )
         {
            Login();
         }

         var client = new RestClient( _serviceUrl );

         var request = new RestRequest( "tables/User", Method.GET ) { RequestFormat = DataFormat.Json };

         request.AddHeader( "X-ZUMO-AUTH", SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken ) );

         var user = new User();
         bool threadDone = false;
         bool gotUser = false;
         bool notFound = false;
         client.ExecuteAsync( request, response =>
         {
            if ( response.StatusCode != HttpStatusCode.NotFound )
            {
               gotUser = user.Deserialize( response.Content );
            }
            else
            {
               notFound = true;
            }

            threadDone = true;
         } );

         while ( !threadDone )
         {
         }

         if ( notFound )
         {
            PostUser( new User() { UserName = "BrettBruh" } );

            threadDone = false;

            client.ExecuteAsync( request, response =>
            {
               if ( response.StatusCode != HttpStatusCode.NotFound )
               {
                  gotUser = user.Deserialize( response.Content );
               }

               threadDone = true;
            } );
         }

         while ( !threadDone )
         {
         }

         if ( gotUser )
         {
            return user;
         }
      }
      catch ( AuthenticationExpiredException )
      {
         throw new AzureErrorException();
      }

      return null;
   }

   private static void PostUser( User user )
   {
      try
      {
         if ( !IsLoggedIn() )
         {
            Login();
         }

         var client = new RestClient( _serviceUrl );

         var request = new RestRequest( "tables/User", Method.POST ) { RequestFormat = DataFormat.Json };

         request.AddHeader( "X-ZUMO-AUTH", SettingsManager.GetSetting( SettingsManager.StringSetting.AuthToken ) );

         request.AddObject( user );

         bool threadDone = false;

         client.ExecuteAsync( request, response =>
         {
            threadDone = true;
         } );

         while ( !threadDone )
         {
         }
      }
      catch ( AuthenticationExpiredException )
      {
         throw new AzureErrorException();
      }
   }

   private static void CheckTokenExpiration( string authToken )
   {
      if ( IsTokenExpired( authToken ) )
      {
         throw new AuthenticationExpiredException();
      }
   }

   private static JObject GetJToken( string token )
   {
      if ( string.IsNullOrEmpty( token ) )
      {
         return null;
      }

      var splitStr = token.Split( new Char[] { '.' } );
      if ( splitStr.Count() < 2 )
      {
         return null;
      }
      var jwt = splitStr[1];

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

   private static void FinishLogin( string authToken )
   {
      SettingsManager.SetSetting( SettingsManager.StringSetting.AuthToken, authToken );

      try
      {
         CheckTokenExpiration( authToken );
      }
      catch ( AuthenticationExpiredException )
      {
         authToken = string.Empty;
      }

      SettingsManager.SetSetting( SettingsManager.StringSetting.AuthToken, authToken );

      if ( _loginListener != null )
      {
         _loginListener.LoginCompleted();
      }
   }
}
