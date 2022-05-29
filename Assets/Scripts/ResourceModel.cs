using UniRx;
using System;

public class ResourceModel : IDisposable
{
    public ReactiveProperty<int> Count { get; }
    public int Goal { get; }
    public string Name { get; }

    public ResourceModel(string name, ReactiveProperty<int> count, int goal = 0) {
        Name = name;
        Count = count;
        Goal = goal;
    }

    public void Dispose() {
        Count.Dispose();
    }
}
