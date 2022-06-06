using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;

public class ResourceProducerFactory 
{
    private readonly ResourceController _resourceController;
    private readonly ResourceProducerConfig _config;

    private ResourceProducerData _producerData;

    public ReactiveProperty<string> CurrentProducer { get; } = new ReactiveProperty<string>();

    public ResourceProducerFactory(ResourceController resourceController, ResourceProducerConfig config) {
        _resourceController = resourceController;
        _config = config;
    }

    public void SetProducerData(string producerName) {
        CurrentProducer.Value = producerName;
        _producerData = _config.ResourceProducers.FirstOrDefault(x => x.Name == producerName);
    }

    public ResourceProducerModel GetResourceProducerModel() {
        if (_producerData == null) {
            return null;
        }
        var model = new ResourceProducerModel(_producerData, _resourceController);
        model.StartProduction().Forget();
        return model;
    }

    public ResourceProducerModel GetResourceProducerModel(string producerName, DateTime productionEndTime, int productionCyclesCount) {
        var config = _config.ResourceProducers.FirstOrDefault(x => x.Name == producerName) ?? _producerData;
        if (config == null) {
            return null;
        }
        var model = new ResourceProducerModel(config, _resourceController, productionEndTime, productionCyclesCount);
        if (productionEndTime.Ticks == 0 || (model.NeedsConsumable && productionCyclesCount == 0)) {
            model.StartProduction().Forget();
        } else
        {
            model.Produce();
        }
        return model;
    }

    public ResourceProducerView GetResourceProducerView(ResourceProducerModel model) {
        var config = _config.ResourceProducers.FirstOrDefault(x => x.Name == model.Name) ?? _producerData;
        if (config == null) {
            return null;
        }
        var view = UnityEngine.Object.Instantiate(config.SpawnObject);
        view.Initialize(model);
        return view;
    }

    public ResourceProducerView CreateResourceProducer() {
        var model = GetResourceProducerModel();
        var view = UnityEngine.Object.Instantiate(_producerData.SpawnObject);
        view.Initialize(model);
        return view;
    }
}