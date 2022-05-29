using UniRx;

public class ResourceModel
{
    public ReactiveProperty<int> Count { get; }
    public ReactiveProperty<int> Goal { get; }
    public string Name { get; }

    public ResourceModel(string name, ReactiveProperty<int> count, ReactiveProperty<int> goal) {
        Name = name;
        Count = count;
        Goal = goal;
    }
}
