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
         //var newView = CoreApplication.CreateNewView();
         //int newViewId = 0;
         //await newView.Dispatcher.RunAsync( CoreDispatcherPriority.Normal, () =>
         //{
         //   var frame = new Frame();
         //   frame.Navigate( typeof( TView ), null );
         //   Window.Current.Content = frame;

         //   newViewId = ApplicationView.GetForCurrentView().Id;
         //} );
         //var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync( newViewId );
         ////if ( switchToView && viewShown )
         ////{
         ////   // Switch to new view
         ////   await ApplicationViewSwitcher.SwitchAsync( newViewId );
         ////}

         //try
         //{
         // var dialog = new NativePlugin.TestContentDialog2();
         //var dialog = new ContentDialog()
         //{
         //   Background = new SolidColorBrush( Colors.Black ),
         //   //MinWidth = 0,
         //   //MinHeight = 0,
         //   Width = 1000,
         //   Height = 1000,
         //   Content = new LoginWindow()
         //};

         //var dialog = new Windows.UI.Popups.MessageDialog(
         //       "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
         //       "Morbi iaculis augue vel elementum volutpat.",
         //       "Lorem Ipsum" );

         var dialog = new ContentDialog();

         var stackPanel1 = new StackPanel() { Orientation = Orientation.Vertical };

         var webView = new WebView( WebViewExecutionMode.SeparateThread ) { Width = 400, Height = 300 };
         webView.Navigate( new Uri( "http://google.com" ) );

         var stackPanel2 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
         var button1 = new Button() { Content = "Button 1" };
         var button2 = new Button() { Content = "Button 2" };
         stackPanel2.Children.Add( button1 );
         stackPanel2.Children.Add( button2 );

         stackPanel1.Children.Add( webView );
         stackPanel1.Children.Add( stackPanel2 );

         dialog.Content = stackPanel1;

         //var panel = new StackPanel();

         //panel.Children.Add( new TextBlock
         //{
         //   Text = "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
         //                "Morbi iaculis augue vel elementum volutpat.",
         //   TextWrapping = TextWrapping.Wrap,
         //} );

         //var cb = new CheckBox
         //{
         //   Content = "Agree"
         //};

         //cb.SetBinding( CheckBox.IsCheckedProperty, new Binding
         //{
         //   Source = dialog,
         //   Path = new PropertyPath( "IsPrimaryButtonEnabled" ),
         //   Mode = BindingMode.TwoWay,
         //} );
         //panel.Children.Add( cb );
         //dialog.Content = panel;

         //var dialog = new LoginControl();

         await dialog.ShowAsync();
         //}
         //catch ( Exception ex )
         //{
         //   var dialog = new Windows.UI.Popups.MessageDialog( ex.Message );
         //   await dialog.ShowAsync();
         //}
         //var b = new Rect( new Point( 1, 1 ), new Point( 1, 1 ) );

         //UnityEngine.WSA.Application.InvokeOnUIThread( async () =>
         // {
         //    await Window.Current.Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
         //    {
         //       var dialog = new MessageDialog( "Your message here" );
         //       await dialog.ShowAsync();
         //    } );
         // }, true );

         //Window.Current.Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
         //{
         //   int i = 0;
         //   i++;
         //} );



         //// Setup Content
         //var panel = new StackPanel();

         //panel.Children.Add( new TextBlock
         //{
         //   Text = "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
         //                "Morbi iaculis augue vel elementum volutpat.",
         //   TextWrapping = TextWrapping.Wrap,
         //} );

         //var cb = new CheckBox
         //{
         //   Content = "Agree"
         //};

         //cb.SetBinding( CheckBox.IsCheckedProperty, new Binding
         //{
         //   Source = dialog,
         //   Path = new PropertyPath( "IsPrimaryButtonEnabled" ),
         //   Mode = BindingMode.TwoWay,
         //} );

         //panel.Children.Add( cb );
         //dialog.Content = panel;

         //// Add Buttons
         //dialog.PrimaryButtonText = "OK";
         //dialog.IsPrimaryButtonEnabled = false;
         //dialog.PrimaryButtonClick += delegate
         //{
         //   btn.Content = "Result: OK";
         //};

         //dialog.SecondaryButtonText = "Cancel";
         //dialog.SecondaryButtonClick += delegate
         //{
         //   btn.Content = "Result: Cancel";
         //};

         //// Show Dialog
         //var result = await dialog.ShowAsync();
         //if ( result == ContentDialogResult.None )
         //{
         //   btn.Content = "Result: NONE";
         //}
         return "Universal Windows Auth Token";
      }
   }
}
