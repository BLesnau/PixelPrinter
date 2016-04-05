using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

         var dialog = new ContentDialog()
         {
            //Background = new SolidColorBrush( Colors.Black ),
            MinWidth = 0,
            MinHeight = 0,
            Width = 500,      
            Content = new LoginControl()
         };

         //var dialog = new LoginControl();

         await dialog.ShowAsync();

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
