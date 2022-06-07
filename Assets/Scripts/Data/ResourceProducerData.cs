using UnityEngine;
using System;

[Serializable]
public class ResourceProducerData
{
    [SerializeField] private string _name;
    [SerializeField] private string _consumeResource;
    [SerializeField] private int _consumeAmout;
    [SerializeField] private int _consumeTime;
    [SerializeField] private string _produceResource;
    [SerializeField] private int _produceAmount;
    [SerializeField] private int _produceTime;
    [SerializeField] private ResourceProducerView _spawnObject;

    public string Name => _name;
    public string ConsumeResource => _consumeResource;
    public int ConsumeAmount => _consumeAmout;
    public int ConsumeTime => _consumeTime;
    public string ProduceResource => _produceResource;
    public int ProduceAmount => _produceAmount;
    public int ProduceTime => _produceTime;
    public ResourceProducerView SpawnObject => _spawnObject;
}
