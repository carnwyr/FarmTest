using UniRx;

public class FieldTileModel 
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
            _producer.Value.OnTap();
        }
    }
  
    private void SpawnProducer() {
        _producer.Value = Factory.GetResourceProducerModel();
    }
}