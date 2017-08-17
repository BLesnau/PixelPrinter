using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Automation
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

      private void button_Click( object sender, RoutedEventArgs e )
      {
         Thread.Sleep( 2000 );
         SendKeys.SendWait( "lesnaubr@gmail.com" );
         SendKeys.SendWait( "{TAB}" );
         SendKeys.SendWait( "{TAB}" );
         SendKeys.SendWait( " " );
         Thread.Sleep( 2000 );
         SendKeys.SendWait( "Holein135135" );
         SendKeys.SendWait( "{TAB}" );
         SendKeys.SendWait( " " );
      }
   }
}
