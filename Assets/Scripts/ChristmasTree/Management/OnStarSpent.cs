namespace ChristmasTree.Management
{
    public struct OnStarSpent
    {
        public bool Success { get; private set; }

        public OnStarSpent(bool success)
        {
            Success = success;
        }
    }
}