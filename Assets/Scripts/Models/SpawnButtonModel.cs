using UniRx;
using System;

public class SpawnButtonModel : IDisposable 
{
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    private readonly ResourceProducerFactory _factory;

    public string SpawnName { get; }
    public ReactiveProperty<bool> IsSelected { get; } = new ReactiveProperty<bool>();

    public SpawnButtonModel(string name, ResourceProducerFactory factory) {
        _factory = factory;

        SpawnName = name;
        _factory.CurrentProducer
            .Subscribe(x => IsSelected.Value = x == SpawnName)
            .AddTo(_subscriptions);
    }

    public void SetSelectedProducer() {
        _factory.SetProducerData(SpawnName);
    }

    public void Dispose() {
        _subscriptions.Dispose();
    }
}