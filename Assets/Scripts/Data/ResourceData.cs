using UnityEngine;
using System;

[Serializable]
public class ResourceData
{
    [SerializeField] private string _name;
    [SerializeField] private string _priceResource;
    [SerializeField] private int _price;

    public string Name => _name;
    public string PriceResource => _priceResource;
    public int Price => _price;
}
