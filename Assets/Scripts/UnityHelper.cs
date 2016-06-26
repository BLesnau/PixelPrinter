﻿#if UNITY_WSA || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
#define IS_UNITY
#endif

#if UNITY_EDITOR
#define IS_EDITOR
#endif

#if UNITY_WSA && !UNITY_EDITOR
#define IS_UWP
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
#define IS_ANDROID
#endif

#if UNITY_IOS && !UNITY_EDITOR
#define IS_IOS
#endif

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
}
