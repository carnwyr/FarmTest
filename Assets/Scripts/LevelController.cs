using UnityEngine;
using UnityEngine.UI;

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

    public int GetCurrentGoal() {
        return _config.Levels[_currentLevel].Goal;
    }

    public void StartLevel() {
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