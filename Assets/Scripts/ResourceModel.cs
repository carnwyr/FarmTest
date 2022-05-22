using UniRx;
using System;

public class ResourceModel : IDisposable
{
    public ReactiveProperty<int> Count { get; }
    public string Name { get; }

    public ResourceModel(string name, ReactiveProperty<int> count) {
        Name = name;
        Count = count;
    }

    public void Dispose() {
        Count.Dispose();
    }
}
