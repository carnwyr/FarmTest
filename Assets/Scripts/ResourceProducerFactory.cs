public class ResourceProducerFactory 
{
    private ResourceController _resourceController;

    private ResourceProducerData _producerData;

    public ResourceProducerFactory(ResourceController resourceController) {
        _resourceController = resourceController;
    }

    public void SetProducerData(ResourceProducerData producerConfig) {
        _producerData = producerConfig;
    }

    public ResourceProducerModel GetResourceProducerModel() {
        if (_producerData == null) {
            return null;
        }
        var model = new ResourceProducerModel(_producerData, _resourceController);
        return model;
    }

    public ResourceProducerView GetResourceProducerView(ResourceProducerModel model) {
        if (_producerData == null) {
            return null;
        }
        var view = UnityEngine.Object.Instantiate(_producerData.SpawnObject);
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