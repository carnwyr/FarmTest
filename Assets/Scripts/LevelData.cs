using System;
using UnityEngine;

[Serializable]
public class LevelData {
    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeY;
    [SerializeField] private int _goal;

    public int SizeX => _sizeX;
    public int SizeY => _sizeY;
    public int Goal => _goal;
}