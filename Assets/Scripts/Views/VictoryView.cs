using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class VictoryView : MonoBehaviour
{
    [SerializeField] private Button _continue;
    [SerializeField] private Text _altText;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private VictoryModel _model;

    public ReactiveCommand<Unit> ContinueGame { get; } = new ReactiveCommand<Unit>();

    public void Initialize(VictoryModel model) {
        _model = model;

        _continue.gameObject.SetActive(_model.CanContinue);
        _altText.gameObject.SetActive(!_model.CanContinue);

        _continue.OnClickAsObservable()
            .First()
            .Subscribe(_ => ContinueGame.Execute(Unit.Default))
            .AddTo(_subscriptions);
    }

    private void OnDestroy() {
        _subscriptions.Dispose();
    }
}