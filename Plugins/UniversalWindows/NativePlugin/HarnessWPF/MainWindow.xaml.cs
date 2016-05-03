using System;
using System.Windows;

namespace HarnessWPF
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void GetUserClick( object sender, RoutedEventArgs e )
      {
         try
         {
            var user = AzureHelper.GetUser();
         }
         catch ( AuthenticationExpiredException )
         {
            MessageBox.Show( "AUTHENTICATION EXPIRED" );
         }
         catch ( AzureErrorException )
         {
            MessageBox.Show( "AN AZURE ERROR OCURRED" );
         }
      }
   }
}
