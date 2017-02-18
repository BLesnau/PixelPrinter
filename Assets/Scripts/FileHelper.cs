using System.IO;
using UnityEngine;

public static class FileHelper
{
   public static bool FileExists( string filePath )
   {
      var longPath = Path.Combine( Application.persistentDataPath, filePath );
      return File.Exists( longPath );
   }
}