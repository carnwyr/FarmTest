using UniRx;
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class SaveController : IDisposable {
    [Serializable]
    private class SavedData {
        public int CurrentLevel;
        public List<ResourceData> Resources;
        public List<TileData> Field;
    }

    [Serializable]
    private class ResourceData {
        public string Name;
        public int Amount;

        public ResourceData(string name, int amount) {
            Name = name;
            Amount = amount;
        }
    }

    [Serializable]
    private class TileData {
        public int Index;
        public string ProducerName;
        public long ProductionEndTime;
        public int ProductionCyclesCount;
    }

    private readonly string _filePath = Path.Combine(Application.persistentDataPath, "save");
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private SavedData _savedData;

    public SaveController(ResourceController resourceController, LevelController levelController) {
        if (!TryLoadData()) {
            _savedData = GetDefaultData(resourceController);
            SaveData();
        }

        resourceController.ResourceAmountChanged
            .Subscribe(x => SetResource(x.Item1, x.Item2))
            .AddTo(_subscriptions);

        levelController.CurrentLevel
            .Subscribe(x => _savedData.CurrentLevel = x)
            .AddTo(_subscriptions);

        levelController.ChangeTileContent
            .Subscribe(x => SetTile(x.Item1, x.Item2))
            .AddTo(_subscriptions);
    }

    public void SaveData() {
        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Close();
        }
        var dataString = JsonUtility.ToJson(_savedData);
        File.WriteAllText(_filePath, dataString);
    }

    public bool TryLoadData() {
        if (!File.Exists(_filePath))
        {
            return false;
        }

        return true;
    }

    private SavedData GetDefaultData(ResourceController resourceController) {
        var defaultData = new SavedData();

        defaultData.Resources = new List<ResourceData>();
        foreach (var res in resourceController.Resources) {
            defaultData.Resources.Add(new ResourceData(res.Key, res.Value.Value));
        }

        defaultData.Field = new List<TileData>();

        return defaultData;
    }

    private void SetResource(string resourceName, int resourceAmount) {
        if (!_savedData.Resources.Any(x => x.Name == resourceName))
        {
            return;
        }

        _savedData.Resources.First(x => x.Name == resourceName).Amount = resourceAmount;
        SaveData();
    }

    private void SetTile(int index, ResourceProducerModel content) {
        var tileData = _savedData.Field.FirstOrDefault(x => x.Index == index);

        tileData = new TileData()
        {
            Index = index,
            ProducerName = content.Name,
            ProductionCyclesCount = content.ProductionCyclesCount.Value,
            ProductionEndTime = content.ProductionEndTime.Value.Ticks
        };

        _savedData.Field.Add(tileData);
        SaveData();

        content.ProductionCyclesCount
            .Do(x => tileData.ProductionCyclesCount = x)
            .Subscribe(x => SaveData())
            .AddTo(_subscriptions);

        content.ProductionEndTime
            .Do(x => tileData.ProductionEndTime = x.Ticks)
            .Subscribe(x => SaveData())
            .AddTo(_subscriptions);
    }

    public void Dispose() {
        _subscriptions.Dispose();
    }

}