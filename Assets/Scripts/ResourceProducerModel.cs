using System;
using UniRx;
using UnityEngine;

public class ResourceProducerModel
{
    private string _resource;
    private int _produceAmount;
    private int _produceTime;
    private IDisposable _productionCycle;

    public ReactiveProperty<float> ProduceProgress { get; } = new ReactiveProperty<float>();

    public ResourceProducerModel(string resource, int amount, int time) {
        _resource = resource;
        _produceAmount = amount;
        _produceTime = time;

        Produce();
    }

    private void Produce() {
        var productionEndTime = DateTime.UtcNow.AddSeconds(_produceTime);
        _productionCycle = Observable.EveryUpdate()
            .Subscribe(_ => {
                var timeLeft = (float)(productionEndTime - DateTime.UtcNow).TotalSeconds;
                var fractionLeft = Mathf.Clamp(timeLeft, 0, _produceTime) / _produceTime;
                ProduceProgress.Value = 1 - fractionLeft;
                if (ProduceProgress.Value == 1) {
                    _productionCycle?.Dispose();
                }
            });
    }

    private void OnDestroy() {
        _productionCycle?.Dispose();
        ProduceProgress.Dispose();
    }
}
