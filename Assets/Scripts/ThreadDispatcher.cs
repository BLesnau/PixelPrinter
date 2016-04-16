using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for 
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class ThreadDispatcher : MonoBehaviour
{
   private readonly static Queue<Action> _executionQueue = new Queue<Action>();
   private static ThreadDispatcher _instance = null;

   public static ThreadDispatcher Instance()
   {
      if ( !Exists() )
      {
         throw new Exception( "UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene." );
      }
      return _instance;
   }

   public static bool Exists()
   {
      return _instance != null;
   } 

   public void Update()
   {
      lock ( _executionQueue )
      {
         while ( _executionQueue.Count > 0 )
         {
            _executionQueue.Dequeue().Invoke();
         }
      }
   }

   public void Enqueue( IEnumerator action )
   {
      lock ( _executionQueue )
      {
         _executionQueue.Enqueue( () =>
         {
            StartCoroutine( action );
         } );
      }
   }

   void Awake()
   {
      if ( _instance == null )
      {
         _instance = this;
         DontDestroyOnLoad( this.gameObject );
      }
   }

   void OnDestroy()
   {
      _instance = null;
   }
}