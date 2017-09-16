using System.Runtime.InteropServices;

public class WebGLPluginInterop /*: MonoBehaviour*/
{
   [DllImport( "__Internal" )]
   public static extern void Hello( string appUrl );
}