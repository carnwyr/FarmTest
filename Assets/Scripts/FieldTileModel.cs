using UniRx;

public class FieldTileModel 
{
    private ReactiveProperty<ResourceProducerModel> _producer = new ReactiveProperty<ResourceProducerModel>();

    public IReadOnlyReactiveProperty<ResourceProducerModel> Producer => _producer;
    public ResourceProducerFactory Factory { get; }

    public FieldTileModel(ResourceProducerFactory factory, ResourceProducerModel producer = null) {
        Factory = factory;
        _producer.Value = producer;
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