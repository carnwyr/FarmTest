using System;
using UniRx;
using UnityEngine;

public class FieldTileModel : IDisposable 
{
    private ReactiveProperty<ResourceProducerModel> _producer = new ReactiveProperty<ResourceProducerModel>();

    public IReadOnlyReactiveProperty<ResourceProducerModel> Producer => _producer;
    public ResourceProducerFactory Factory { get; }

    public FieldTileModel(ResourceProducerFactory factory) {
        Factory = factory;
    }

    public void OnTap() {
        if (_producer.Value == null) {
            SpawnProducer();
        }
        else {
            _producer.Value.TryCollect();
        }
    }
  
    private void SpawnProducer() {
        _producer.Value = Factory.GetResourceProducerModel();
    }

    public void Dispose() {
        _producer.Dispose();
    }
}