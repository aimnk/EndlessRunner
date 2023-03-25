namespace Game.Features.Parallax
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(ParallaxData), menuName = "Game/Data/" + nameof(ParallaxData))]
    public class ParallaxData : ScriptableObject
    {
        [field: SerializeField] 
        public GameObject PrefabParallax { get; set; }
    }
}
