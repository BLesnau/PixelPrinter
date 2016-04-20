using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Query;
using System.Web.Http.Results;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Newtonsoft.Json.Linq;
using PixelPrinterService.DataObjects;
using PixelPrinterService.Models;

namespace PixelPrinterService
{
   [Authorize]
   public class UserController : TableController<User>
   {
      protected override void Initialize( HttpControllerContext controllerContext )
      {
         base.Initialize( controllerContext );
         PixelPrinterContext context = new PixelPrinterContext();
         DomainManager = new EntityDomainManager<User>( context, Request );
      }

      // GET tables/User
      public async Task<IHttpActionResult> GetUser()
      {
         if ( ClaimsPrincipal.Current != null )
         {
            var stableSid = ClaimsPrincipal.Current.Claims.Where( c => c.Type == "stable_sid" ).Select( c => c.Value ).SingleOrDefault();

            if ( !string.IsNullOrWhiteSpace( stableSid ) )
            {
               var existingUsers = Query().Where( x => x.UserUnique == stableSid );
               if ( existingUsers.Count() == 1 )
               {
                  return Ok( (new SingleResult<User>( existingUsers )).Queryable );
               }

               await PostUser( new User() { UserUnique = stableSid, UserName = "TestName2" } );

               existingUsers = Query().Where( x => x.UserUnique == stableSid );
               if ( existingUsers.Count() == 1 )
               {
                  return Ok( (new SingleResult<User>( existingUsers )).Queryable );
               }
            }
         }

         return InternalServerError();
      }

      // POST tables/User
      public async Task<IHttpActionResult> PostUser( User item )
      {
         User current = await InsertAsync( item );
         return CreatedAtRoute( "Tables", new { id = current.Id }, current );
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
   }
}