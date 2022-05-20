using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;

public class GameManager : MonoBehaviour
{
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    [SerializeField] private int _fieldX;
    [SerializeField] private int _fieldY;
    // TODO spawn
    [SerializeField] private GridLayoutGroup _field;
    [SerializeField] private RectTransform _tilePrefab;
    [SerializeField] private Button _spawnButton;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField] private ResourceProducerConfig _resourceProducerConfig;

    private RectTransform[,] _fieldContents;
    private string _selectedProducer;

    void Start()
    {
        InitField();
        CreateSpawnButtons();
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

    private void CreateSpawnButtons() {
        _spawnButton.gameObject.SetActive(false);
        foreach (var conf in _resourceProducerConfig.ResourceProducers) {
            var newButton = Instantiate(_spawnButton, _spawnButton.transform.parent);
            newButton.gameObject.SetActive(true);
            // TODO init
            newButton.GetComponentInChildren<Text>().text = conf.Name;
            newButton.OnClickAsObservable()
                .Subscribe(_ => _selectedProducer = conf.Name)
                .AddTo(_subscriptions);
        }        
    }

    private void SpawnProducer(RectTransform tile) {
        var producerType = _resourceProducerConfig.ResourceProducers.FirstOrDefault(x => x.Name == _selectedProducer);
        if (producerType != null) {
            var model = new ResourceProducerModel(producerType.ProducedResource, producerType.ProduceAmount, producerType.ProduceTime);
            var view = Instantiate(producerType.SpawnObject, tile.transform);
            view.Initialize(model);
        }
    }
}