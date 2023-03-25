namespace Game.Features.Configs
{
  using UnityEngine;

  [CreateAssetMenu(fileName = nameof(ObstaclesData), menuName = "Game/Data/" + nameof(ObstaclesData))]
  public class ObstaclesData : ScriptableObject
  {
    [field: SerializeField] 
    public Sprite SpriteObstacle { get; private set; }

    [field: SerializeField] 
    public Vector3 Scale { get; private set; }

    [field: SerializeField]
    public Vector3 Rotation { get; private set; }
  }
}
