using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

// TODO available levels check
public class LevelController : IDisposable {
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    private readonly LevelConfig _config;
    private readonly ResourceProducerFactory _producerFactory;
    private readonly ResourceController _resourceController;

    private GridLayoutGroup _field;
    private int _currentLevel;

    public ReactiveProperty<int> ResourceGoal { get; } = new ReactiveProperty<int>();
    public ReactiveCommand<Unit> FinishLevel { get; } = new ReactiveCommand<Unit>();

    public LevelController(LevelConfig config, ResourceProducerFactory producerFactory, ResourceController resourceController, Canvas canvas) {
        _config = config;
        _producerFactory = producerFactory;
        _resourceController = resourceController;

        _field = UnityEngine.Object.Instantiate(_config.FieldPrefab, canvas.transform);
    }

    public bool CanContinue() {
        return _currentLevel < _config.Levels.Count;
    }

    public void StartLevel() {
        InitField();
        ResourceGoal.Value = _config.Levels[_currentLevel].Goal;
        // TODO remove hardcode
        // TODO move check to resource controller
        _resourceController.Resources["Coin"]
            .Where(x => x >= ResourceGoal.Value)
            .First()
            .Subscribe(_ => WinLevel())
            .AddTo(_subscriptions);
    }

    private void InitField() {
        ClearField();
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

    private void ClearField() {
        foreach (Transform tile in _field.transform) {
            GameObject.Destroy(tile.gameObject);
        }
    }

    private void WinLevel() {
        _currentLevel++;
        FinishLevel.Execute(Unit.Default);
    }

    public void Dispose() {
        _subscriptions.Dispose();
    }
}