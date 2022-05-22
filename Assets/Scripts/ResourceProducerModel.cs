using System;
using UniRx;
using UnityEngine;

public class ResourceProducerModel : IDisposable
{
    private readonly ResourceController _resourceController;

    private string _name;
    private string _resource;
    private int _produceAmount;
    private int _produceTime;
    private IDisposable _productionCycle;

    public ReactiveProperty<float> ProduceProgress { get; } = new ReactiveProperty<float>();

    public ResourceProducerModel(ResourceProducerData data, ResourceController resourceController) {
        _resourceController = resourceController;

        _name = data.Name;
        _resource = data.ProducedResource;
        _produceAmount = data.ProduceAmount;
        _produceTime = data.ProduceTime;

        Produce();
    }

    private void Produce() {
        var productionEndTime = DateTime.UtcNow.AddSeconds(_produceTime);
        _productionCycle = Observable.EveryUpdate()
            .Subscribe(_ => {
                var timeLeft = (float)(productionEndTime - DateTime.UtcNow).TotalSeconds;
                var fractionLeft = Mathf.Clamp(timeLeft, 0, _produceTime) / _produceTime;
                ProduceProgress.Value = 1 - fractionLeft;
                if (Mathf.Approximately(ProduceProgress.Value, 1)) {
                    _productionCycle?.Dispose();
                }
            });
    }

    public void TryCollect() {
        if (Mathf.Approximately(ProduceProgress.Value, 1)) {
            _resourceController.AddResource(_resource, _produceAmount);
            Produce();
        }
    }

    public void Dispose() {
        _productionCycle?.Dispose();
        ProduceProgress.Dispose();
    }
}
