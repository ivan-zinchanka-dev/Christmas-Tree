namespace Services.Score
{
    internal struct OnScoreChangedComponent
    {
        public int Value { get; private set; }
        
        public OnScoreChangedComponent(int value)
        {
            Value = value;
        }
    }
}