using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary.Evaluation
{
    public class Evaluator : IEvaluator
    {
        public ScorerConfiguration Config { get; set; }

        public Evaluator(ScorerConfiguration config)
        {
            Config = config;
        }

        public (int blackScore, int whiteScore) GetScore(
            BitBoard board,
            bool isWhiteKingInCheck,
            bool isBlackKingInCheck,
            bool isStalemate,
            int totalMoveCount
        )
        {
            return (
                GetScoreInternal(
                    board,
                    isWhiteKingInCheck,
                    isBlackKingInCheck,
                    Colors.Black,
                    isStalemate
                ),
                GetScoreInternal(
                    board,
                    isWhiteKingInCheck,
                    isBlackKingInCheck,
                    Colors.White,
                    isStalemate
                )
            );
        }

        public int GetPieceValue(PieceTypes piece)
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

        private int GetScoreInternal(
            BitBoard board,
            bool isWhiteKingInCheck,
            bool isBlackKingInCheck,
            Colors color,
            bool isStalemate
        )
        {
            if (isStalemate)
            {
                return Config.StalemateScore;
            }
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
                        else if (
                            (rank == 6 || rank == 3) && f >= Files.C && f <= Files.F
                            || (f == Files.C || f == Files.F) && (rank == 5 || rank == 4)
                        )
                        {
                            score += Config.CenterBorderValue;
                        }
                    }
                }
            }
            bool isOpponentInCheck =
                color == Colors.White ? isBlackKingInCheck : isWhiteKingInCheck;
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
