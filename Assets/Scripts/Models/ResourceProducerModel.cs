using System;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ResourceProducerModel : IDisposable
{
    private readonly ResourceController _resourceController;

    private string _name;
    private string _consumeResource;
    private int _consumeAmount;
    private int _consumeTime;
    private string _produceResource;
    private int _produceAmount;
    private int _produceTime;
    private IDisposable _productionCycle;
    private UniTask _consumeCycle;
    private ReactiveProperty<DateTime> _productionEndTime = new ReactiveProperty<DateTime>();
    private ReactiveProperty<int> _productionCyclesCount = new ReactiveProperty<int>();

    public ReactiveProperty<float> ProduceProgress { get; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> ConsumableLeft { get; } = new ReactiveProperty<float>();

    public string Name => _name;
    public IReadOnlyReactiveProperty<DateTime> ProductionEndTime => _productionEndTime;
    public IReadOnlyReactiveProperty<int> ProductionCyclesCount => _productionCyclesCount;

    public bool NeedsConsumable { get; }

    public ResourceProducerModel(ResourceProducerData data, ResourceController resourceController) {
        _resourceController = resourceController;

        _name = data.Name;
        _consumeResource = data.ConsumeResource;
        _consumeAmount = data.ConsumeAmount;
        _consumeTime = data.ConsumeTime;
        _produceResource = data.ProduceResource;
        _produceAmount = data.ProduceAmount;
        _produceTime = data.ProduceTime;

        NeedsConsumable = !string.IsNullOrEmpty(_consumeResource);
    }

    public ResourceProducerModel(ResourceProducerData data, ResourceController resourceController, 
        DateTime productionEndTime, int productionCyclesCount) : this(data, resourceController)
    {
        _productionEndTime.Value = productionEndTime;
        _productionCyclesCount.Value = productionCyclesCount;

        var timeLeft = (float)(_productionEndTime.Value - DateTime.UtcNow).TotalSeconds;
        var fractionLeft = Mathf.Clamp(timeLeft, 0, _produceTime) / _produceTime;
        ProduceProgress.Value = 1 - fractionLeft;

        if (NeedsConsumable)
        {
            var totalCycles = _consumeTime / _produceTime;
            var currentCycleLeft = 1f / totalCycles * fractionLeft;
            ConsumableLeft.Value = (float)(_productionCyclesCount.Value - 1) / totalCycles + currentCycleLeft;
        }
    }

    public void OnTap() {
        if (Mathf.Approximately(ProduceProgress.Value, 1)) {
            Collect();
        }
        else if (_productionCyclesCount.Value == 0) {
            Consume();
        }
    }

    public async UniTask StartProduction() {
        ProduceProgress.Value = 0;
        _consumeCycle = _productionCyclesCount
            .Where(x => x > 0 || !NeedsConsumable)
            .First()
            .ToUniTask();
        await _consumeCycle;
        _productionEndTime.Value = DateTime.UtcNow.AddSeconds(_produceTime);
        Produce();
    } 

    public void Produce() {        
        _productionCycle = Observable.EveryUpdate()
            .Subscribe(_ => {
                var timeLeft = (float)(_productionEndTime.Value - DateTime.UtcNow).TotalSeconds;
                var fractionLeft = Mathf.Clamp(timeLeft, 0, _produceTime) / _produceTime;
                ProduceProgress.Value = 1 - fractionLeft;

                if (NeedsConsumable)
                {
                    var totalCycles = _consumeTime / _produceTime;
                    var currentCycleLeft = 1f / totalCycles * fractionLeft;
                    ConsumableLeft.Value = (float)(_productionCyclesCount.Value - 1) / totalCycles + currentCycleLeft;
                }

                if (Mathf.Approximately(ProduceProgress.Value, 1)) {
                    _productionCycle?.Dispose();
                }
            });
    }

    private void Consume() 
    {
        if (!NeedsConsumable) {
            return;
        }

        if (_resourceController.TrySpendResource(_consumeResource, _produceAmount)) {
            _productionCyclesCount.Value = _consumeTime / _produceTime;
        }
    }

    public void Collect() {
        _resourceController.AddResource(_produceResource, _produceAmount);
        if (NeedsConsumable)
        {
            _productionCyclesCount.Value--;
        }
        StartProduction().Forget();
    }

    public void Dispose() {
        _productionCycle?.Dispose();
        ProduceProgress.Dispose();
    }
}
