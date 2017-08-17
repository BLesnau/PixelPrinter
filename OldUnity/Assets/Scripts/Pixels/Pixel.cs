using UnityEngine;

public class Pixel : MonoBehaviour
{
   public Color Color
   {
      get
      {
         return GetComponent<Renderer>().material.color;
      }
      set
      {
         GetComponent<Renderer>().material.color = value;
      }
   }

   void Start()
   {
   }

   void Update()
   {
   }
}