using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _fieldX;
    [SerializeField] private int _fieldY;
    // TODO spawn
    [SerializeField] private GridLayoutGroup _field;
    [SerializeField] private RectTransform _tilePrefab;
    // TODO dynamic
    [SerializeField] private Button _wheatSpawn;

    private RectTransform[,] _fieldContents;
    private ResourceProducer _selectedProducer;

    void Start()
    {
        InitField();
        SubscribeOnSpawnButtons();
    }

    private void InitField() {
        _field.cellSize = _tilePrefab.sizeDelta;
        _field.constraintCount = _fieldX;

        _fieldContents = new RectTransform[_fieldX, _fieldY];

        for (var i = 0; i < _fieldX; i++) {
            for (var j = 0; j < _fieldY; j++) {
                var go = Instantiate(_tilePrefab, _field.transform);
                _fieldContents[i, j] = go;
                go.GetComponent<Button>().onClick.AddListener(() => SpawnProducer(go));
            }
        }
    }

    private void SubscribeOnSpawnButtons() {
        _wheatSpawn.onClick.AddListener(() => _selectedProducer = ResourceProducer.Wheat);
    }

    private void SpawnProducer(RectTransform tile) {
        tile.GetChild(0).gameObject.SetActive(true);
    }
}

public enum ResourceProducer {
    Wheat,
    Chicken,
    Cow
}
