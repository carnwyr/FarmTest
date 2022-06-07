using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ResourceProducerConfig))]
public class ResourceProducerConfig : ScriptableObject
{
    [SerializeField] private List<ResourceProducerData> _resourceProducers;

    public List<ResourceProducerData> ResourceProducers => _resourceProducers;
}
