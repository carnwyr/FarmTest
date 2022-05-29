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

        _label.text = _model.Name;
        _model.Count
            .CombineLatest(_model.Goal, (count, goal) => (count, goal))
            .Subscribe(x => _count.text = GetCountText(x.count, x.goal))
            .AddTo(_subscriptions);
    }

    private string GetCountText(int count, int goal) {
        if (goal > 0) {
            return $"{count}/{goal}";
        }
        return count.ToString();
    }

    private void OnDestroy() {
        _subscriptions.Dispose();
    }
}
