namespace Game.Features.AssetProvider
{
  using UnityEngine;
  
  /// <summary>
  /// Провайдер для загрузки ассетов
  /// </summary>
  public interface IAssetProvider
  {
      public T LoadAsset<T>(string path) where T : Object;
  }
}