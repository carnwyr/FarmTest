using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ResourceView : MonoBehaviour
{
    [SerializeField] private Text _label;
    [SerializeField] private Text _count;

    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    private ResourceModel _model;

    public void Initialize(ResourceModel model) {
        _model = model;
        _model.AddTo(_subscriptions);

        _label.text = _model.Name;
        _model.Count
            .Subscribe(x => _count.text = x.ToString())
            .AddTo(_subscriptions);
    }

    private void OnDestroy() {
        _subscriptions.Dispose();
    }
}
