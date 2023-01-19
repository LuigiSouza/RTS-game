namespace T4.Resource
{
    public class Resource
    {
        private readonly ResourceType resourceType;
        private int currentAmount;

        public Resource(ResourceType resourceType, int initialAmount)
        {
            this.resourceType = resourceType;
            currentAmount = initialAmount;
        }

        public void AddAmount(int value)
        {
            currentAmount += value;
            if (currentAmount < 0) currentAmount = 0;
        }

        public ResourceType Type { get => resourceType; }
        public int Amount { get => currentAmount; }
    }
}