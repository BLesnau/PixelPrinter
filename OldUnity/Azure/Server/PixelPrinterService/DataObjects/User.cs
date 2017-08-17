using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.ModelBinding;
using Microsoft.Azure.Mobile.Server;

namespace PixelPrinterService.DataObjects
{
   public class User : EntityData
   {
      [Index( "Index_UserName", 1, IsUnique = true )]
      [MaxLength(450)]
      public string UserName { get; set; }

      [Index( "Index_UserUnique", 1, IsUnique = true )]
      [MaxLength(450)]
      public string UserUnique { get; set; }
   }
}