using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
      private readonly string _settingsFile = Path.Combine( ApplicationData.Current.LocalFolder.Path, "user.txt" );

      public MainPage()
      {
         this.InitializeComponent();

         _mobileService = new MobileServiceClient( PixelPrinterPlugin.GetServiceUrl( _environment ) );
         _mobileService.AlternateLoginHost = new Uri( PixelPrinterPlugin.GetServiceUrl( PixelPrinterPlugin.TargetEnvironment.Live ) );
      }

      private async void LoginNewClick( object sender, RoutedEventArgs e )
      {
         await Login();
      }

      private async void LoginAgainClick( object sender, RoutedEventArgs e )
      {
         try
         {
            if ( IsTokenExpired( GetAuthToken() ) )
            {
               var dlg = new MessageDialog( "TOKEN EXPIRED - LOGGING IN AGAIN" );
               await dlg.ShowAsync();

               DeleteSettingsFile();
            }
         }
         catch { }

         await Login();
      }

      private async Task Login()
      {
         var authToken = GetAuthToken();
         var userId = GetUserId();

         try
         {
            if ( !string.IsNullOrWhiteSpace( authToken ) && !string.IsNullOrWhiteSpace( userId ) )
            {
               _mobileService.CurrentUser = new MobileServiceUser( userId ) { MobileServiceAuthenticationToken = authToken };

               var dlg = new MessageDialog( "ALREADY SIGNED IN" );
               await dlg.ShowAsync();
            }
            else
            {
               var strToSplit = await PixelPrinterPlugin.GetAuthToken( _environment );
               var splitStr = strToSplit.Split( ',' );
               authToken = splitStr[0];
               userId = splitStr[1];
               var uniqueId = GetJToken( authToken )["stable_sid"].ToString();

               await SaveSettings( userId, authToken );

               var dlg = new MessageDialog( "SIGNED IN" );
               await dlg.ShowAsync();
            }
         }
         catch { }

         var dlg2 = new MessageDialog( authToken );
         await dlg2.ShowAsync();
         dlg2 = new MessageDialog( userId );
         await dlg2.ShowAsync();
      }

      private async void GetUserClick( object sender, RoutedEventArgs e )
      {
         try
         {
            //if ( !string.IsNullOrWhiteSpace( _authToken ) )
            //{
            var user = await _mobileService.GetTable<User>().ToCollectionAsync();
            var dlg = new MessageDialog( user.First().UserName );
            await dlg.ShowAsync();
            //}
            //else
            //{
            //   var dlg = new MessageDialog( "NOT LOGGED IN" );
            //   await dlg.ShowAsync();
            //}
         }
         catch ( MobileServiceInvalidOperationException ex )
         {
            var dlg = new MessageDialog( $"Exception: {ex.Message} - {ex.Response.StatusCode}" );
            await dlg.ShowAsync();

            if ( ex.Response.StatusCode == HttpStatusCode.NotFound )
            {
               dlg = new MessageDialog( "USER NOT FOUND - CREATING USER" );
               await dlg.ShowAsync();

               try
               {
                  await _mobileService.GetTable<User>().InsertAsync( new User() { UserName = "abababababababababab" } );

                  dlg = new MessageDialog( "CREATED USER - GETTING USER" );
                  await dlg.ShowAsync();

                  try
                  {
                     var user = await _mobileService.GetTable<User>().ToCollectionAsync();
                     dlg = new MessageDialog( user.First().UserName );
                     await dlg.ShowAsync();
                  }
                  catch ( MobileServiceInvalidOperationException ex2 )
                  {
                     dlg = new MessageDialog( $"Exception: {ex2.Message} - {ex2.Response.StatusCode}" );
                     await dlg.ShowAsync();
                  }
               }
               catch ( MobileServiceInvalidOperationException ex3 )
               {
                  dlg = new MessageDialog( $"Exception: {ex3.Message} - {ex3.Response.StatusCode}" );
                  await dlg.ShowAsync();
               }
            }
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

      private string GetUserId()
      {
         return GetLineOfSettingsFile( 0 );
      }

      private string GetAuthToken()
      {
         return GetLineOfSettingsFile( 1 );
      }

      private string GetLineOfSettingsFile( int lineIndex )
      {
         if ( File.Exists( _settingsFile ) )
         {
            var fileText = File.ReadLines( _settingsFile ).ToList();
            if ( fileText.Count() >= 2 )
            {
               if ( !string.IsNullOrWhiteSpace( fileText[lineIndex] ) )
               {
                  return fileText[lineIndex];
               }
            }
         }

         return null;
      }

      private void DeleteSettingsFile()
      {
         if ( File.Exists( _settingsFile ) )
         {
            File.Delete( _settingsFile );
         }
      }

      private async Task SaveSettings( string userId, string authToken )
      {
         await Task.Run( () => File.WriteAllLines( _settingsFile, new string[] { userId, authToken } ) );
      }
   }
}