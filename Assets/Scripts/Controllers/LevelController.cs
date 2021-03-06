using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;

public class LevelController : IDisposable {
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    private readonly LevelConfig _config;
    private readonly ResourceProducerFactory _producerFactory;
    private readonly ResourceController _resourceController;
    private readonly string _targetResource;

    private GridLayoutGroup _field;
    private List<(int, ResourceProducerModel)> _loadedTileContents;
    private ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>();

    public IReadOnlyReactiveProperty<int> CurrentLevel => _currentLevel;
    public ReactiveProperty<int> ResourceGoal { get; } = new ReactiveProperty<int>();
    public ReactiveCommand<Unit> FinishLevel { get; } = new ReactiveCommand<Unit>();
    public ReactiveCommand<(int, ResourceProducerModel)> ChangeTileContent { get; } = new ReactiveCommand<(int, ResourceProducerModel)>();

    public LevelController(LevelConfig config, ResourceProducerFactory producerFactory, ResourceController resourceController, Canvas canvas, string targetResource)
    {
        _config = config;
        _producerFactory = producerFactory;
        _resourceController = resourceController;
        _targetResource = targetResource;

        _field = UnityEngine.Object.Instantiate(_config.FieldPrefab, canvas.transform);
    }

    public void LoadData(int currentLevel, List<(int, ResourceProducerModel)> loadedTiles) {
        _currentLevel.Value = currentLevel;
        _loadedTileContents = loadedTiles;
    }

    public bool CanContinue() {
        return _currentLevel.Value < _config.Levels.Count;
    }

    public void StartLevel() {
        if (_currentLevel.Value >= _config.Levels.Count) {
            return;
        }

        InitField();
        ResourceGoal.Value = _config.Levels[_currentLevel.Value].Goal;
        _resourceController.Resources[_targetResource]
            .Where(x => x >= ResourceGoal.Value)
            .First()
            .Subscribe(_ => WinLevel())
            .AddTo(_subscriptions);
    }

    // TODO tile pooling
    // TODO tile lifetime
    private void InitField() {
        ClearField();
        var sizeX = _config.Levels[_currentLevel.Value].SizeX;
        var sizeY = _config.Levels[_currentLevel.Value].SizeY;
        _field.constraintCount = sizeX;

        for (var i = 0; i < sizeX; i++) {
            for (var j = 0; j < sizeY; j++) {
                var index = i * sizeX + j;
                ResourceProducerModel loadedTileContent = null;
                if(_loadedTileContents != null && _loadedTileContents.Any(x => x.Item1 == index)) {
                    var loadedTile = _loadedTileContents.First(x => x.Item1 == index);
                    loadedTileContent = loadedTile.Item2;
                    _loadedTileContents.Remove(loadedTile);
                }
                var model = new FieldTileModel(_producerFactory, loadedTileContent);
                var view = UnityEngine.Object.Instantiate(_config.TilePrefab, _field.transform);
                view.Initialize(model);

                model.Producer
                    .Where(x => x != null)
                    .First()
                    .Subscribe(x => ChangeTileContent.Execute((index, x)))
                    .AddTo(_subscriptions);
            }
        }
        
        _loadedTileContents?.Clear();
    }

    private void ClearField() {
        foreach (Transform tile in _field.transform) {
            GameObject.Destroy(tile.gameObject);
        }
    }

    private void WinLevel() {
        _currentLevel.Value++;
        _resourceController.Reset();
        FinishLevel.Execute(Unit.Default);
    }

    public void Dispose() {
        _subscriptions.Dispose();
    }
}