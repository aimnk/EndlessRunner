namespace Game.Features.AssetProvider
{
  using UnityEngine;

  /// <summary>
  /// Провайдер для загрузки и спавна ассетов из ресурсов
  /// </summary>
  public class AssetResourcesProvider : IAssetProvider
  {
    public T LoadAsset<T>(string path) where T : Object 
      => Resources.Load<T>(path);
  }
}