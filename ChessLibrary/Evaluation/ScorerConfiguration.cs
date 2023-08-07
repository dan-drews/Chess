namespace ChessLibrary.Evaluation
{
    public class ScorerConfiguration
    {
        public int MaxTimeMilliseconds { get; set; } = 1000;
        public int StartingDepth { get; set; } = 1;
        public int? MaxDepth { get; set; } = 1;

        public int OpponentInCheckScore { get; set; } = 50;
        public int SelfInCheckScore { get; set; } = -15;
        public int CenterSquareValue { get; set; } = 2;
        public int CenterBorderValue { get; set; } = 1;
        public int StalemateScore { get; set; } = 0;

        public int PawnValue { get; set; } = 10;
        public int KnightValue { get; set; } = 40;
        public int BishopValue { get; set; } = 40;
        public int RookValue { get; set; } = 65;
        public int QueenValue { get; set; } = 105;
        public int KingValue { get; set; } = 500;
    }
}
