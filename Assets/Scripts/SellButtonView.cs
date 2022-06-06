using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SellButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Text _text;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private SellButtonModel _model;

    public void Initialize(SellButtonModel model) {
        _model = model;

        _text.text = _model.ResourceName;

        _button.OnClickAsObservable()
            .Subscribe(x => _model.SellResource())
            .AddTo(_subscriptions);
    }

    private void OnDestroy() {
        _subscriptions.Dispose();
    }
}
