mergeInto(LibraryManager.library, 
{
   Hello: function ( appUrl ) 
   {
      handleSignInClick();

      return;

      var url = Pointer_stringify( appUrl );
      //alert(url);
      var client = new WindowsAzure.MobileServiceClient( url );
      //alert(client);
      client.login( "facebook" ).done( function (results) {
      alert("You are now logged in as: " + results.userId);
   }, function (err) 
      {
         alert("Error: " + err);
      }
   );

     // window.alert("Hello, world!");
   },
});