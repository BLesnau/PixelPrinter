using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using PixelPrinterService.DataObjects;
using PixelPrinterService.Models;

namespace PixelPrinterService
{
   //[Authorize]
   public class UserController : TableController<User>
   {
      protected override void Initialize( HttpControllerContext controllerContext )
      {
         base.Initialize( controllerContext );
         PixelPrinterContext context = new PixelPrinterContext();
         DomainManager = new EntityDomainManager<User>( context, Request );
      }

      // GET tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
      public SingleResult<User> GetUser( string id )
      {
         return Lookup( id );
      }

      // POST tables/User
      public async Task<IHttpActionResult> PostUser( User item )
      {
         User current = await InsertAsync( item );
         return CreatedAtRoute( "Tables", new { id = current.Id }, current );
      }

      // GET tables/User
      public async Task<IQueryable<User>> GetAllUsers(string doit)
      {
         await PostUser(new User() { UserName = "MyName"});

         var user = await User.GetAppServiceIdentityAsync<GoogleCredentials>( Request );
         //user.UserId
         return Query();
      }

      // PATCH tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
      //public Task<User> PatchUser( string id, Delta<User> patch )
      //{
      //   return UpdateAsync( id, patch );
      //}

      // DELETE tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
      //public Task DeleteUser( string id )
      //{
      //   return DeleteAsync( id );
      //}
   }
}