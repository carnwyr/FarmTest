using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ResourceConfig))]
public class ResourceConfig : ScriptableObject
{
    [SerializeField] private List<ResourceData> _resources;

    public List<ResourceData> Resources => _resources;
}
