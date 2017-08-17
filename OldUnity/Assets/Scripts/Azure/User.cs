public class User : AzureResponseObject
{
   [AzureResponsePart]
   public string Id { get; set; }
   [AzureResponsePart]
   public string UserName { get; set; }
   [AzureResponsePart]
   public string UserUnique { get; set; }
}
