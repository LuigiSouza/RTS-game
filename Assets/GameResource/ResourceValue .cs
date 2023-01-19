using T4.Resource;

[System.Serializable]
public class ResourceValue
{
    public ResourceType code;
    public int amount = 0;

    public ResourceValue(ResourceType code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}