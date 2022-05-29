using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button _spawnButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private ResourceView _resourceCounter;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField] private ResourceProducerConfig _resourceProducerConfig;
    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] private Canvas _fieldCanvas;
    [SerializeField] private Canvas _uiCanvas;
    [SerializeField] private VictoryView _victoryView;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    
    private ResourceController _resourceController;
    private LevelController _levelController;
    private ResourceProducerFactory _producerFactory;

    void Start()
    {
        _resourceController = new ResourceController(_resourceConfig);
        _producerFactory = new ResourceProducerFactory(_resourceController);
        _levelController = new LevelController(_levelConfig, _producerFactory, _resourceController, _fieldCanvas);

        _levelController.AddTo(_subscriptions);

        CreateResourceCounters();
        CreateSpawnButtons();
        CreateSellButtons();

        _levelController.FinishLevel
            .Subscribe(_ => FinishLevel())
            .AddTo(_subscriptions);

        StartLevel();
    }

    private void StartLevel() {
        _resourceController.Reset();
        _levelController.StartLevel();
    }

    private void CreateResourceCounters() {
        _resourceCounter.gameObject.SetActive(false);
        foreach (var resource in _resourceController.Resources) {
            // TODO remove hardcode
            var goal = resource.Key == "Coin" ? _levelController.ResourceGoal : new ReactiveProperty<int>();
            var model = new ResourceModel(resource.Key, resource.Value, goal);
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

    private void FinishLevel() {
        var canContinue = _levelController.CanContinue();
        var model = new VictoryModel(canContinue);
        var view = Instantiate(_victoryView, _uiCanvas.transform);
        view.Initialize(model);
        view.ContinueGame
            .First()
            .Do(_ => GameObject.Destroy(view.gameObject))
            .Subscribe(_ => StartLevel())
            .AddTo(_subscriptions);
    }
}