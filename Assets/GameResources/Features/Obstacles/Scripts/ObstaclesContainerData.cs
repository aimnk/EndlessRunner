using System;
using System.Collections.Generic;
using Game.Features.Configs;
using UnityEngine;

/// <summary>
/// Контейнер доступных препятствий
/// </summary>
[CreateAssetMenu(fileName = nameof(ObstaclesContainerData), menuName = "Game/Data/" + nameof(ObstaclesContainerData))]
public class ObstaclesContainerData : ScriptableObject
{
   [field: SerializeField] 
   public List<HeightObstaclesData> HeightObstaclesDatas { get; private set; }
}

/// <summary>
/// Дата Препятствий в зависимости от высоты
/// </summary>
[Serializable]
public class HeightObstaclesData
{
   public float MinHeight;

   public float Maxheight;

   public List<ObstaclesData> ObstaclesDatas;
}