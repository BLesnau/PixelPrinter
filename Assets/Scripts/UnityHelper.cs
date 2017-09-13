public static class UnityHelper
{
   public static bool IsUnity()
   {
#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
      return true;
#else
      return false;
#endif
   }

   public static bool IsEditor()
   {
#if UNITY_EDITOR
      return true;
#else
      return false;
#endif
   }

   public static bool IsUWP()
   {
#if UNITY_WSA && !UNITY_EDITOR
      return true;
#else
      return false;
#endif
   }

   public static bool IsAndroid()
   {
#if UNITY_ANDROID && !UNITY_EDITOR
      return true;
#else
      return false;
#endif
   }

   public static bool IsIOS()
   {
#if UNITY_IOS && !UNITY_EDITOR
      return true;
#else
      return false;
#endif
   }

   public static bool IsWebGL()
   {
#if UNITY_WEBGL && !UNITY_EDITOR
      return true;
#else
      return false;
#endif
   }
}