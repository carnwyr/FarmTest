public class SellButtonModel 
{
    private readonly ResourceController _resourceController;

    public string ResourceName { get; }

    public SellButtonModel(string name, ResourceController resourceController) {
        _resourceController = resourceController;
        ResourceName = name;
    }

    public void SellResource() {
        _resourceController.Sell(ResourceName);
    }
}