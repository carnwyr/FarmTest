using UnityEngine;
using System;

[Serializable]
public class ResourceProducerData
{
    [SerializeField] private string _name;
    [SerializeField] private int _produceAmount;
    [SerializeField] private int _produceTime;
    [SerializeField] private string _producedResource;
    [SerializeField] private ResourceProducerView _spawnObject;

    public string Name => _name;
    public int ProduceAmount => _produceAmount;
    public int ProduceTime => _produceTime;
    public string ProducedResource => _producedResource;
    public ResourceProducerView SpawnObject => _spawnObject;
}
