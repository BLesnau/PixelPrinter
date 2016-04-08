using UnityEngine;
using System.Collections;
using UnityEngine.WSA;

public class WSAComm : MonoBehaviour
{
   public delegate string StringCallback();

   public static StringCallback LoginCallback;

   // Use this for initialization
   void Start()
   {

   }

   // Update is called once per frame
   void Update()
   {

   }

   public void DoStuff( StringCallback callback )
   {
      LoginCallback = callback;
   }
}
