namespace Game.Features.Configs
{
    using AssetProvider;

    public class GameConfig
    {
        public DifficultData DifficultData { get; private set; }

        public InputConfigData InputConfigData { get; private set; }
        
        private IAssetProvider _assetProvider;

        public GameConfig(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            Init();
        }

        private void Init()
        {
            InputConfigData = _assetProvider.LoadAsset<InputConfigData>(AssetsDataPath.INPUT_DATA_PATH);
            DifficultData = _assetProvider.LoadAsset<DifficultData>(AssetsDataPath.DIFFICULT_DATA_PATH);
        }
    }
}
