using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace PixelPrinterService.DataObjects
{
   public class Figure : EntityData
   {
      [Index( "Index_FileId", 1, IsUnique = true )]
      [MaxLength( 100 )]
      public string FileId { get; set; }

      public string ThumbId { get; set; }

      [MaxLength( 100 )]
      public string UserId { get; set; }
   }
}