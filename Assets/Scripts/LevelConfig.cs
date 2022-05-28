using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(LevelConfig))]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private GridLayoutGroup _fieldPrefab;
    [SerializeField] private FieldTileView _tilePrefab;
    [SerializeField] private List<LevelData> _levels;

    public GridLayoutGroup FieldPrefab => _fieldPrefab;
    public FieldTileView TilePrefab => _tilePrefab;
    public List<LevelData> Levels => _levels;
}