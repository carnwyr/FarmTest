using UnityEngine;
using System;

[Serializable]
public class ResourceData
{
    [SerializeField] private string _name;

    public string Name => _name;
}
