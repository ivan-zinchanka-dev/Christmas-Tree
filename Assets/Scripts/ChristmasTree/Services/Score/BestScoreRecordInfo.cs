namespace ChristmasTree.Services.Score
{
    public struct BestScoreRecordInfo
    {
        public int OldBestScore { get; private set; }
        public int NewBestScore { get; private set; }

        public BestScoreRecordInfo(int oldBestScore, int newBestScore)
        {
            OldBestScore = oldBestScore;
            NewBestScore = newBestScore;
        }
    }
}