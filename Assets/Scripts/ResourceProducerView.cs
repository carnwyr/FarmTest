using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ResourceProducerView : MonoBehaviour, IDisposable
{
    [SerializeField] private Slider _progress;
    [SerializeField] private Slider _consumableLeft;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private ResourceProducerModel _model;

    public void Initialize(ResourceProducerModel model) {
        _model = model;
        _model.AddTo(_subscriptions);

        _model.ProduceProgress
            .Subscribe(x => _progress.value = x)
            .AddTo(_subscriptions);

        if (_model.NeedsConsumable)
        {
            _model.ConsumableLeft
                .Subscribe(x => _consumableLeft.value = x)
                .AddTo(_subscriptions);
        }
    }

    private void OnDestroy() {
        Dispose();
    }

    public void Dispose() {
        _subscriptions.Dispose();
    }
}
