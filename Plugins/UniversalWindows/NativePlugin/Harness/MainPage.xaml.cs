using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using NativePlugin;
using Newtonsoft.Json.Linq;

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
      private User _user;

      public MainPage()
      {
         this.InitializeComponent();

         _mobileService = new MobileServiceClient( PixelPrinterPlugin.GetServiceUrl( _environment ) );
         //_mobileService.AlternateLoginHost = new Uri( PixelPrinterPlugin.GetServiceUrl( PixelPrinterPlugin.TargetEnvironment.Live ) );

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
               await Login();
            }
         }
         catch
         {
            await Login();
         }
      }

      private async Task Login()
      {
         try
         {
            var strToSplit = await PixelPrinterPlugin.GetAuthToken( _environment );
            var splitStr = strToSplit.Split( ',' );
            _authToken = splitStr[0];
            _userId = splitStr[1];
            if ( !string.IsNullOrWhiteSpace( _authToken ) )
            {
               //await _mobileService.LoginAsync( MobileServiceAuthenticationProvider.Google, GetJToken( _authToken ) );
               //_mobileService.CurrentUser = new MobileServiceUser( _userId );
               var users = _mobileService.GetTable<User>().WithParameters( new Dictionary<string, string>() { { "doit", "blah" } } );
               var dlg = new MessageDialog( _authToken );
               await dlg.ShowAsync();
            }
            else
            {
               var dlg = new MessageDialog( "DID NOT LOGIN" );
               await dlg.ShowAsync();
            }
         }
         catch ( Exception )
         {
            var i = 0;
            i++;
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
         //// Check for a signed-in user.
         //if ( client.CurrentUser == null ||
         //    String.IsNullOrEmpty( client.CurrentUser.MobileServiceAuthenticationToken ) )
         //{
         //   // Raise an exception if there is no token.
         //   throw new InvalidOperationException(
         //       "The client isn't signed-in or the token value isn't set." );
         //}

         //// Get just the JWT part of the token.
         //var jwt = client.CurrentUser
         //    .MobileServiceAuthenticationToken
         //    .Split( new Char[] { '.' } )[1];

         //var jwt = token.Split( new Char[] { '.' } )[1];

         //// Undo the URL encoding.
         //jwt = jwt.Replace( '-', '+' );
         //jwt = jwt.Replace( '_', '/' );
         //switch ( jwt.Length % 4 )
         //{
         //   case 0:
         //   {
         //      break;
         //   }
         //   case 2:
         //   {
         //      jwt += "==";
         //      break;
         //   }
         //   case 3:
         //   {
         //      jwt += "=";

         //      break;
         //   }
         //   default:
         //   {
         //      //throw new System.Exception(
         //      //   "The base64url string is not valid." );
         //      break;
         //   }
         //}

         //// Decode the bytes from base64 and write to a JSON string.
         //var bytes = Convert.FromBase64String( jwt );
         //string jsonString = UTF8Encoding.UTF8.GetString( bytes, 0, bytes.Length );

         // Parse as JSON object and get the exp field value, 
         // which is the expiration date as a JavaScript primative date.
         var jsonObj = GetJToken( token );
         //JObject jsonObj = JObject.Parse( jsonString );
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