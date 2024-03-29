﻿namespace ChessLibrary.Evaluation
{
    public interface IEvaluator
    {
        ScorerConfiguration Config { get; set; }

        int GetPieceValue(PieceTypes piece);

        (int blackScore, int whiteScore) GetScore(
            FullBitBoard board,
            bool isWhiteKingInCheck,
            bool isBlackKingInCheck,
            bool isStalemate,
            int totalMoveCount
        );
    }
}
