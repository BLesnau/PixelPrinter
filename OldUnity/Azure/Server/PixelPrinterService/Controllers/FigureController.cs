using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using PixelPrinterService.DataObjects;
using PixelPrinterService.Models;

namespace PixelPrinterService.Controllers
{
   public class FigureController : TableController<Figure>
   {
      protected override void Initialize( HttpControllerContext controllerContext )
      {
         base.Initialize( controllerContext );
         PixelPrinterContext context = new PixelPrinterContext();
         DomainManager = new EntityDomainManager<Figure>( context, Request );
      }

      // GET tables/Figure
      public IQueryable<Figure> GetAllFigure()
      {
         return Query();
      }

      // GET tables/Figure/48D68C86-6EA6-4C25-AA33-223FC9A27959
      public SingleResult<Figure> GetFigure( string id )
      {
         return Lookup( id );
      }

      // PATCH tables/Figure/48D68C86-6EA6-4C25-AA33-223FC9A27959
      public Task<Figure> PatchFigure( string id, Delta<Figure> patch )
      {
         return UpdateAsync( id, patch );
      }

      // POST tables/Figure
      public async Task<IHttpActionResult> PostFigure( Figure item )
      {
         Figure current = await InsertAsync( item );
         return CreatedAtRoute( "Tables", new { id = current.Id }, current );
      }

      // DELETE tables/Figure/48D68C86-6EA6-4C25-AA33-223FC9A27959
      public Task DeleteFigure( string id )
      {
         return DeleteAsync( id );
      }
   }
}
