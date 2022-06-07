using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = nameof(UiConfig))]
public class UiConfig : ScriptableObject
{
    [SerializeField] private SpawnButtonView _spawnButton;
    [SerializeField] private SellButtonView _sellButton;
    [SerializeField] private ResourceView _resourceCounter;
    [SerializeField] private Transform _resourceRoot;
    [SerializeField] private ScrollRect _sideBar;
    [SerializeField] private Text _sideBarHeader;
    [SerializeField] private string _spawnHeader;
    [SerializeField] private string _sellHeader;

    public SpawnButtonView SpawnButton => _spawnButton;
    public SellButtonView SellButton => _sellButton;
    public ResourceView ResourceCounter => _resourceCounter;
    public Transform ResourceRoot => _resourceRoot;
    public ScrollRect SideBar => _sideBar;
    public Text SideBarHeader => _sideBarHeader;
    public string SpawnHeader => _spawnHeader;
    public string SellHeader => _sellHeader;
}
