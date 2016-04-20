﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.WindowsAzure.MobileServices;
using NativePlugin;
using Newtonsoft.Json.Linq;
using PixelPrinter;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Harness
{
   /// <summary>
   /// An empty page that can be used on its own or navigated to within a Frame.
   /// </summary>
   public sealed partial class MainPage : Page
   {
      private PixelPrinterPlugin.TargetEnvironment _environment = PixelPrinterPlugin.TargetEnvironment.Local;
      private readonly MobileServiceClient _mobileService;
      private string _authToken;
      private string _userId;
      private string _uniqueId;
      private User _user;

      public MainPage()
      {
         this.InitializeComponent();

         _mobileService = new MobileServiceClient( PixelPrinterPlugin.GetServiceUrl( _environment ) );
         _mobileService.AlternateLoginHost = new Uri( PixelPrinterPlugin.GetServiceUrl( PixelPrinterPlugin.TargetEnvironment.Live ) );

         _user = null;
      }

      private async void LoginNewClick( object sender, RoutedEventArgs e )
      {
         await Login();
      }

      private async void LoginAgainClick( object sender, RoutedEventArgs e )
      {
         try
         {
            if ( IsTokenExpired( _authToken ) )
            {
               _authToken = _userId = _uniqueId = string.Empty;
            }
         }
         catch { }

         await Login();
      }

      private async Task Login()
      {
         try
         {
            if ( !string.IsNullOrWhiteSpace( _authToken ) && !string.IsNullOrWhiteSpace( _userId ) && !string.IsNullOrWhiteSpace( _uniqueId ) )
            {
               _mobileService.CurrentUser = new MobileServiceUser( _userId ) { MobileServiceAuthenticationToken = _authToken };

               var dlg = new MessageDialog( "ALREADY SIGNED IN" );
               await dlg.ShowAsync();
            }
            else
            {
               var strToSplit = await PixelPrinterPlugin.GetAuthToken( _environment );
               var splitStr = strToSplit.Split( ',' );
               _authToken = splitStr[0];
               _userId = splitStr[1];
               _uniqueId = GetJToken( _authToken )["stable_sid"].ToString();

               var dlg = new MessageDialog( "SIGNED IN" );
               await dlg.ShowAsync();
            }
         }
         catch { }

         var dlg2 = new MessageDialog( _authToken );
         await dlg2.ShowAsync();
         dlg2 = new MessageDialog( _userId );
         await dlg2.ShowAsync();
      }

      private async void GetUserClick( object sender, RoutedEventArgs e )
      {
         if ( !string.IsNullOrWhiteSpace( _authToken ) )
         {
            var users = await _mobileService.GetTable<User>().WithParameters( new Dictionary<string, string>() { { "id", _uniqueId } } ).ToCollectionAsync();
            var dlg = new MessageDialog( _authToken );
            await dlg.ShowAsync();
         }
         else
         {
            var dlg = new MessageDialog( "NOT LOGGED IN" );
            await dlg.ShowAsync();
         }
      }

      private JObject GetJToken( string token )
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

      private bool IsTokenExpired( string token )
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
   }
}