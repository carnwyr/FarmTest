using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SpawnButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Text _text;
    [SerializeField] private Image _highlight;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private SpawnButtonModel _model;

    public void Initialize(SpawnButtonModel model) {
        _model = model;
        _model.AddTo(_subscriptions);

        _text.text = _model.SpawnName;

        _model.IsSelected
            .Subscribe(x => _highlight.enabled = x)
            .AddTo(_subscriptions);

        _button.OnClickAsObservable()
            .Subscribe(x => _model.SetSelectedProducer())
            .AddTo(_subscriptions);
    }

    private void OnDestroy() {
        _subscriptions.Dispose();
    }
}
