using UnityEngine;

public class GeneralSetup : MonoBehaviour
{
   public TargetEnvironment.TargetAppEnvironment Environment = TargetEnvironment.TargetAppEnvironment.Local;

   void Start()
   {
      TargetEnvironment.SetEnvironment( Environment );
   }
}