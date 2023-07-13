using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public static class MoveOrdering
    {
        const int CAPTURED_PIECE_MULTIPLIER = 10;
        public static IEnumerable<NewMove> OrderMoves(this IEnumerable<NewMove> moves, Engine engine)
        {
            return moves
                    .OrderByDescending(x =>
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
                    })
                    .ThenBy(x=> Guid.NewGuid().ToString());
        }
    }
}
