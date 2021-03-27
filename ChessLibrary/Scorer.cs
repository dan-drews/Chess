using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{

    public class ScorerConfiguration
    {
        public int MaxTimeMilliseconds { get; set; } = 1000;
        public int StartingDepth { get; set; } = 3;

        public int OpponentInCheckScore { get; set; } = 50;
        public int SelfInCheckScore { get; set; } = -15;
        public int CenterSquareValue { get; set; } = 2;
        public int CenterBorderValue { get; set; } = 1;

        public int PawnValue { get; set; } = 10;
        public int KnightValue { get; set; } = 40;
        public int BishopValue { get; set; } = 40;
        public int RookValue { get; set; } = 65;
        public int QueenValue { get; set; } = 105;
        public int KingValue { get; set; } = 500;
    }

    public class Scorer
    {
        public ScorerConfiguration Config { get; set; }
        public Scorer(ScorerConfiguration config)
        {
            Config = config;
        }

        public (int blackScore, int whiteScore) GetScore(IBoard board, bool isWhiteKingInCheck, bool isBlackKingInCheck)
        {
            return (GetScoreInternal(board, isWhiteKingInCheck, isBlackKingInCheck, Colors.Black), GetScoreInternal(board, isWhiteKingInCheck, isBlackKingInCheck, Colors.White));
        }

        private int GetPieceValue(PieceTypes piece)
        {
            switch (piece)
            {
                case PieceTypes.Pawn:
                    return Config.PawnValue;

                case PieceTypes.Knight:
                    return Config.KnightValue;

                case PieceTypes.Bishop:
                    return Config.BishopValue;

                case PieceTypes.Rook:
                    return Config.RookValue;

                case PieceTypes.Queen:
                    return Config.QueenValue;

                case PieceTypes.King:
                    return Config.KingValue;

                default:
                    return 0;
            }
        }

        private int GetScoreInternal(IBoard board, bool isWhiteKingInCheck, bool isBlackKingInCheck, Colors color)
        {
            int score = 0;
            for (Files f = Files.A; f <= Files.H; f++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var piece = board.GetSquare(f, rank).Piece;
                    if (piece != null && piece.Color == color)
                    {
                        score += GetPieceValue(piece.Type);

                        if ((rank == 5 || rank == 4) && (f == Files.D || f == Files.E))
                        {
                            score += Config.CenterSquareValue;
                        }
                        else if (((rank == 6 || rank == 3) && f >= Files.C && f <= Files.F) ||
                                 ((f == Files.C || f == Files.F) && (rank == 5 || rank == 4)))
                        {
                            score += Config.CenterBorderValue;
                        }
                    }
                }
            }
            bool isOpponentInCheck = color == Colors.White ? isBlackKingInCheck : isWhiteKingInCheck;
            bool isSelfInCheck = color == Colors.White ? isWhiteKingInCheck : isBlackKingInCheck;
            if (isOpponentInCheck)
            {
                score += Config.OpponentInCheckScore;
            }
            if (isSelfInCheck)
            {
                score += Config.SelfInCheckScore;
            }
            return score;
        }
    }
}
