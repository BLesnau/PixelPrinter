using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
   public Image Background;

   void Start()
   {
   }

   void Update()
   {
   }

   public void SetColor( Color color )
   {
      Background.color = color;
   }
}
