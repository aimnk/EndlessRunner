namespace Game.Features.UI
{
   using UnityEngine;
   
   public class MainUI : MonoBehaviour
   {
      [field: SerializeField] 
      public TimerWindow TimerWindow { get; private set; }

      [field: SerializeField]
      public GameObject LoseWindow { get; private set; }
      
      [field: SerializeField] 
      public GameObject StartWindow { get; private set; }
   }
}
