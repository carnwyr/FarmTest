using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button _spawnButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private ResourceView _resourceCounter;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField] private ResourceProducerConfig _resourceProducerConfig;
    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] private Canvas _fieldCanvas;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    
    private ResourceController _resourceController;
    private LevelController _levelController;
    private ResourceProducerFactory _producerFactory;

    void Start()
    {
        _resourceController = new ResourceController(_resourceConfig);
        _producerFactory = new ResourceProducerFactory(_resourceController);
        _levelController = new LevelController(_levelConfig, _producerFactory, _fieldCanvas);
        
        CreateResourceCounters();
        CreateSpawnButtons();
        CreateSellButtons();

        _levelController.StartNextLevel();
    }

    private void CreateResourceCounters() {
        _resourceCounter.gameObject.SetActive(false);
        foreach (var resource in _resourceController.Resources) {
            var model = new ResourceModel(resource.Key, resource.Value);
            var view = Instantiate(_resourceCounter, _resourceCounter.transform.parent);
            view.Initialize(model);
            view.gameObject.SetActive(true);
        }
    }

    private void CreateSpawnButtons() {
        _spawnButton.gameObject.SetActive(false);
        foreach (var conf in _resourceProducerConfig.ResourceProducers) {
            var newButton = Instantiate(_spawnButton, _spawnButton.transform.parent);
            newButton.gameObject.SetActive(true);
            // TODO init
            newButton.GetComponentInChildren<Text>().text = conf.Name;
            newButton.OnClickAsObservable()
                .Subscribe(_ => _producerFactory.SetProducerData(conf))
                .AddTo(_subscriptions);
        }        
    }

    private void CreateSellButtons() {
        _sellButton.gameObject.SetActive(false);
        foreach (var conf in _resourceConfig.Resources) {
            if (string.IsNullOrEmpty(conf.PriceResource)) {
                continue;
            }

            var newButton = Instantiate(_sellButton, _sellButton.transform.parent);
            newButton.gameObject.SetActive(true);
            // TODO init
            newButton.GetComponentInChildren<Text>().text = conf.Name;
            newButton.OnClickAsObservable()
                .Subscribe(_ => _resourceController.Sell(conf))
                .AddTo(_subscriptions);
        }        
    }
}

public class LevelController {
    private readonly LevelConfig _config;
    private readonly ResourceProducerFactory _producerFactory;

    private GridLayoutGroup _field;
    private int _currentLevel;

    public LevelController(LevelConfig config, ResourceProducerFactory producerFactory, Canvas canvas) {
        _config = config;
        _producerFactory = producerFactory;

        _field = UnityEngine.Object.Instantiate(_config.FieldPrefab, canvas.transform);
    }

    public void StartNextLevel() {
        InitField();
    }

    private void InitField() {
        var sizeX = _config.Levels[_currentLevel].SizeX;
        var sizeY = _config.Levels[_currentLevel].SizeY;
        // TODO alter cell size according to the screen size
        _field.cellSize = _config.TilePrefab.GetComponent<RectTransform>().rect.size;
        _field.constraintCount = sizeX;

        for (var i = 0; i < sizeX; i++) {
            for (var j = 0; j < sizeY; j++) {
                var model = new FieldTileModel(_producerFactory);
                // TODO tile pooling
                var view = UnityEngine.Object.Instantiate(_config.TilePrefab, _field.transform);
                view.Initialize(model);
            }
        }
    }
}



