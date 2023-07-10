using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public static class MoveOrdering
    {
        const int CAPTURED_PIECE_MULTIPLIER = 10;
        public static IEnumerable<Move> OrderMoves(this IEnumerable<Move> moves, Engine engine)
        {
            return moves
                    .OrderByDescending(x =>
                    {
                        var score = 0;
                        if (x.CapturedPiece != null)
                        {
                            score += CAPTURED_PIECE_MULTIPLIER * (x.CapturedPiece == null ? 0 : engine.Scorer.GetPieceValue(x.CapturedPiece.Type));
                            score -= engine.Scorer.GetPieceValue(x.Piece.Type);
                        }
                        if (x.Piece.Type == PieceTypes.Pawn && x.PromotedPiece != null)
                        {
                            score += engine.Scorer.GetPieceValue(x.PromotedPiece.Type);
                        }
                        return score;
                    })
                    .ThenBy(x=> Guid.NewGuid().ToString());
        }
    }
}
