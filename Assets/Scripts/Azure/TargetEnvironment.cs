public class TargetEnvironment
{
   public enum TargetAppEnvironment { Local, Live };
   private static TargetAppEnvironment _environment;

   static TargetEnvironment()
   {
      _environment = TargetAppEnvironment.Live;
   }

   public static void SetEnvironment( TargetAppEnvironment env )
   {
      _environment = env;
   }

   public static string GetServiceUrl()
   {
      return GetServiceUrl( _environment );
   }

   public static string GetServiceUrl( TargetAppEnvironment env )
   {
      switch ( env )
      {
         case TargetAppEnvironment.Local:
         {
            return "http://desktop-or80phq/PixelPrinterService";
         }
         case TargetAppEnvironment.Live:
         default:
         {
            return "https://pixelprinter.azurewebsites.net";
         }
      }
   }
}
