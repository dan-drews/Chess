using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public static class MoveOrdering
    {
        public static Random Random = new Random(DateTime.UtcNow.Millisecond);
        const int CAPTURED_PIECE_MULTIPLIER = 10;
        public static IEnumerable<Move> OrderMoves(this IEnumerable<Move> moves, Engine engine, Move? previousBest)
        {
            return moves
                    .OrderBy(x => previousBest != null && x == previousBest.Value ? 0 : 1)
                    .ThenByDescending(x =>
                    {
                        var score = 0;
                        if (x.CapturedPiece != null)
                        {
                            score += CAPTURED_PIECE_MULTIPLIER * (x.CapturedPiece == null ? 0 : engine.Scorer.GetPieceValue(x.CapturedPiece.Value));
                            score -= engine.Scorer.GetPieceValue(x.Piece);
                        }
                        if (x.Piece == PieceTypes.Pawn && x.PromotedType != null)
                        {
                            score += engine.Scorer.GetPieceValue(x.PromotedType.Value);
                        }
                        return score;
                    });
        }
    }
}
