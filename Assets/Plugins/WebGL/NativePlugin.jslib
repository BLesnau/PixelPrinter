mergeInto(LibraryManager.library, 
{
   Hello: function ( appUrl ) 
   {
      var url = Pointer_stringify( appUrl );
      var client = WindowsAzure.MobileServiceClient( url );
      client.login( "facebook" ).done( function (results) {
      alert("You are now logged in as: " + results.userId);
   }, function (err) 
      {
         alert("Error: " + err);
      }
   );

      window.alert("Hello, world!");
   },
});