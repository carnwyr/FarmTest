using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FieldTileView : MonoBehaviour, IDisposable 
{
    [SerializeField] private Button _button;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private FieldTileModel _model;

    public void Initialize(FieldTileModel model) {
        _model = model;
        _model.AddTo(_subscriptions);

        _model.Producer
            .Where(x => x != null)
            .Subscribe(x => CreateContentsView(x))
            .AddTo(_subscriptions);

        _button.OnClickAsObservable()
            .Subscribe(_ => _model.OnTap())
            .AddTo(_subscriptions);
    }

    private void CreateContentsView(ResourceProducerModel model) {
        var view = _model.Factory.GetResourceProducerView(model);
        view.transform.SetParent(transform, false);
        view.AddTo(_subscriptions);
    }

    private void OnDestroy() {
        Dispose();
    }

    public void Dispose() {
        _subscriptions.Dispose();
    }
}