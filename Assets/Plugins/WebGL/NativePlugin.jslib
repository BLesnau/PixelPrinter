mergeInto(LibraryManager.library, 
{
   Hello: function ( appUrl ) 
   {
      var url = Pointer_stringify( appUrl );
      alert(url);
      var client = WindowsAzure.MobileServiceClient( url );
      alert(client);
      client.login( "google" ).done( function (results) {
      alert("You are now logged in as: " + results.userId);
   }, function (err) 
      {
         alert("Error: " + err);
      }
   );

      window.alert("Hello, world!");
   },
});