using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace HarnessWPF
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private readonly string _settingsFile = Path.Combine( @"C: \Users\lesna\AppData\Local\Packages\b65e477c-6255-4ac2-9130-228d0e221b1a_a5p0ghy48hknt\LocalState", "user.txt" );

      public MainWindow()
      {
         InitializeComponent();
      }

      private void CheckTokenExpiration( object sender, RoutedEventArgs e )
      {
         try
         {
            if ( IsTokenExpired( GetAuthToken() ) )
            {
               MessageBox.Show( "TOKEN EXPIRED" );
            }
            else
            {
               MessageBox.Show( "TOKEN IS GOOD" );
            }

            return;
         }
         catch { }

         MessageBox.Show( "AN ERROR OCCURRED" );
      }

      private void GetUserClick( object sender, RoutedEventArgs e )
      {
         throw new System.NotImplementedException();
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

      private void DeleteSettingsFile()
      {
         if ( File.Exists( _settingsFile ) )
         {
            File.Delete( _settingsFile );
         }
      }

      private void SaveSettings( string userId, string authToken )
      {
         File.WriteAllLines( _settingsFile, new string[] { userId, authToken } );
      }
   }
}
