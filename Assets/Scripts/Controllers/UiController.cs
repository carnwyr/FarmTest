using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class UiController
{
    private readonly UiConfig _config;
    private readonly Canvas _canvas;
    private readonly string _targetResource;

    private readonly ResourceController _resourceController;
    private readonly LevelController _levelController;
    private readonly ResourceProducerConfig _producerConfig;
    private readonly ResourceProducerFactory _producerFactory;
    private readonly ResourceConfig _resourceConfig;

    private readonly Transform _resourceRoot;
    private readonly ScrollRect _sideBar;

    public UiController(UiConfig config, Canvas canvas, string targetResource, ResourceController resourceController, LevelController levelController, 
        ResourceProducerConfig resourceProducerConfig, ResourceProducerFactory resourceProducerFactory, ResourceConfig resourceConfig) 
    {
        _config = config;
        _canvas = canvas;
        _targetResource = targetResource;

        _resourceController = resourceController;
        _levelController = levelController;
        _producerConfig = resourceProducerConfig;
        _producerFactory = resourceProducerFactory;
        _resourceConfig = resourceConfig;

        _resourceRoot = Object.Instantiate(_config.ResourceRoot, _canvas.transform);
        _sideBar = Object.Instantiate(_config.SideBar, _canvas.transform);
    }

    public void CreateViews() {
        CreateResourceCounters();
        CreateSpawnButtons();
        CreateSellButtons();
    }

    private void CreateResourceCounters() {
        var prefab = _config.ResourceCounter;
        foreach (var resource in _resourceController.Resources) {
            var goal = resource.Key == _targetResource ? _levelController.ResourceGoal : new ReactiveProperty<int>();
            var model = new ResourceModel(resource.Key, resource.Value, goal);
            var view = Object.Instantiate(prefab, _resourceRoot);
            view.Initialize(model);
        }
    }

    private void CreateSpawnButtons() {
        var header = Object.Instantiate(_config.SideBarHeader, _sideBar.content);
        header.text = _config.SpawnHeader;

        var prefab = _config.SpawnButton;
        foreach (var conf in _producerConfig.ResourceProducers) {
            var model = new SpawnButtonModel(conf.Name, _producerFactory);
            var view = Object.Instantiate(prefab, _sideBar.content);
            view.Initialize(model);
        }
    }

    private void CreateSellButtons() {
        var header = Object.Instantiate(_config.SideBarHeader, _sideBar.content);
        header.text = _config.SellHeader;

        var prefab = _config.SellButton;
        foreach (var conf in _resourceConfig.Resources) {
            if (string.IsNullOrEmpty(conf.PriceResource)) {
                continue;
            }

            var model = new SellButtonModel(conf.Name, _resourceController);
            var view = Object.Instantiate(prefab, _sideBar.content);
            view.Initialize(model);
        }        
    }
}