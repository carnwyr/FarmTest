using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ResourceProducerView : MonoBehaviour
{
    [SerializeField] private Slider _progress;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public ResourceProducerModel Model;

    public void Initialize(ResourceProducerModel model) {
        Model = model;

        Model.ProduceProgress
            .Subscribe(x => _progress.value = x)
            .AddTo(_subscriptions);
    }

    private void OnDestroy() {
        _subscriptions.Dispose();
    }
}
