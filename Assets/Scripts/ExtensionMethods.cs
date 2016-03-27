using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
   public static void RemoveAllBefore<T>( this LinkedList<T> list, LinkedListNode<T> node )
   {
      if ( node != null )
      {
         while ( node.Previous != null )
         {
            list.Remove( node.Previous );
         }
      }
      else
      {
         list.Clear();
      }
   }

   public static void RemoveAllAfter<T>( this LinkedList<T> list, LinkedListNode<T> node )
   {
      if ( node != null )
      {
         while ( node.Next != null )
         {
            list.Remove( node.Next );
         }
      }
      else
      {
         list.Clear();
      }
   }
}