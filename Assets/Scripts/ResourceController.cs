using System.Collections.Generic;
using UniRx;
using System.Linq;

public class ResourceController {
    private readonly ResourceConfig _config;
    private readonly Dictionary<string, ReactiveProperty<int>> _resources = new Dictionary<string, ReactiveProperty<int>>();

    public Dictionary<string, ReactiveProperty<int>> Resources => _resources;
    public ReactiveCommand<(string, int)> ResourceAmountChanged { get; } = new ReactiveCommand<(string, int)>();

    public ResourceController(ResourceConfig resourceConfig) {
        _config = resourceConfig;
        foreach (var resource in resourceConfig.Resources) {
            _resources.Add(resource.Name, new ReactiveProperty<int>());
        }
    }

    public void LoadData(Dictionary<string, int> resources) {
        foreach (var res in resources.Keys) {
            _resources[res].Value = resources[res];
        }
    }

    public void AddResource(string resource, int amount) {
        if (_resources.ContainsKey(resource)) {
            _resources[resource].Value += amount;
            ResourceAmountChanged.Execute((resource, _resources[resource].Value));
        }
    }

    public bool TrySpendResource(string resource, int amount) {
        if (_resources.ContainsKey(resource) && _resources[resource].Value >= amount) {
            _resources[resource].Value -= amount;
            ResourceAmountChanged.Execute((resource, _resources[resource].Value));
            return true;
        }
        return false;
    }

    public void Sell(string resource) {
        if (!_resources.ContainsKey(resource)) {
            return;
        }

        var amount = _resources[resource].Value;
        var price = _config.Resources.First(x => x.Name == resource).Price;
        var priceResource = _config.Resources.First(x => x.Name == resource).PriceResource;
        TrySpendResource(resource, amount);
        AddResource(priceResource, price * amount);
    }

    public void Sell(ResourceData data) {
        var amount = _resources[data.Name].Value;
        TrySpendResource(data.Name, amount);
        AddResource(data.PriceResource, data.Price * amount);
    }

    public void Reset() {
        foreach (var res in _resources) {
            res.Value.Value = 0;
            ResourceAmountChanged.Execute((res.Key, res.Value.Value));
        }
    }
}