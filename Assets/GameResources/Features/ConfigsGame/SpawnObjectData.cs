namespace Game.Features.Configs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(SpawnObjectData), menuName = "Game/Data/" + nameof(SpawnObjectData))]
    public class SpawnObjectData : ScriptableObject
    {
        [field: SerializeField] 
        public GameObject Prefab { get; set; }
        
        [field: SerializeField] 
        public Vector3 StartPoint { get; set; }
    }
}