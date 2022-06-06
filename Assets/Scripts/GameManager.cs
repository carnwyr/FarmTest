using UnityEngine;
using UniRx;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField] private ResourceProducerConfig _resourceProducerConfig;
    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] private UiConfig _uiConfig;

    [SerializeField] private Canvas _fieldCanvas;
    [SerializeField] private Canvas _uiCanvas;
    [SerializeField] private VictoryView _victoryView;
    [SerializeField] private string _targetResource;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    
    private ResourceController _resourceController;
    private ResourceProducerFactory _producerFactory;
    private LevelController _levelController;
    private SaveController _saveController;
    private UiController _uiController;

    void Start()
    {
        _resourceController = new ResourceController(_resourceConfig);
        _producerFactory = new ResourceProducerFactory(_resourceController, _resourceProducerConfig);
        _levelController = new LevelController(_levelConfig, _producerFactory, _resourceController, _fieldCanvas, _targetResource);
        _saveController = new SaveController(_resourceController, _levelController, _producerFactory);
        _uiController = new UiController(_uiConfig, _uiCanvas, _targetResource, _resourceController, _levelController, _resourceProducerConfig, _producerFactory, _resourceConfig);

        _levelController.AddTo(_subscriptions);

        _uiController.CreateViews();
        _producerFactory.SetProducerData(_resourceProducerConfig.ResourceProducers.First().Name);
        _levelController.FinishLevel
            .Subscribe(_ => FinishLevel())
            .AddTo(_subscriptions);

        StartLevel();
    }

    private void StartLevel() {
        _levelController.StartLevel();
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