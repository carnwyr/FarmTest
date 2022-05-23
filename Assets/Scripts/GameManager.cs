using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _fieldX;
    [SerializeField] private int _fieldY;
    // TODO spawn
    [SerializeField] private GridLayoutGroup _field;
    [SerializeField] private FieldTileView _tilePrefab;
    [SerializeField] private Button _spawnButton;
    [SerializeField] private ResourceView _resourceCounter;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField] private ResourceProducerConfig _resourceProducerConfig;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    
    private ResourceController _resourceController;
    private ResourceProducerFactory _producerFactory;

    void Start()
    {
        _resourceController = new ResourceController(_resourceConfig);
        _producerFactory = new ResourceProducerFactory(_resourceController);

        InitField();
        CreateResourceCounters();
        CreateSpawnButtons();
    }

    private void InitField() {
        _field.cellSize = _tilePrefab.GetComponent<RectTransform>().rect.size;
        _field.constraintCount = _fieldX;

        for (var i = 0; i < _fieldX; i++) {
            for (var j = 0; j < _fieldY; j++) {
                var model = new FieldTileModel(_producerFactory);
                var view = Instantiate(_tilePrefab, _field.transform);
                view.Initialize(model);
                view.AddTo(_subscriptions);
            }
        }
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
}

public class ResourceController {
    private readonly Dictionary<string, ReactiveProperty<int>> _resources = new Dictionary<string, ReactiveProperty<int>>();

    // TODO remove
    public Dictionary<string, ReactiveProperty<int>> Resources => _resources;

    public ResourceController(ResourceConfig resourceConfig) {
        foreach (var resource in resourceConfig.Resources) {
            _resources.Add(resource.Name, new ReactiveProperty<int>());
        }
    }

    public void AddResource(string resource, int amount) {
        if (_resources.ContainsKey(resource)) {
            _resources[resource].Value += amount;
        }
    }

    public bool TrySpendResource(string resource, int amount) {
        if (_resources.ContainsKey(resource) && _resources[resource].Value >= amount) {
            _resources[resource].Value -= amount;
            return true;
        }
        return false;
    }
}