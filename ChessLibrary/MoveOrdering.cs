using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public static class MoveOrdering
    {
        public static IEnumerable<Move> OrderMoves(this IEnumerable<Move> moves, Engine engine)
        {
            return moves.OrderByDescending(x => x.CapturedPiece == null ? 0 : engine.Scorer.GetPieceValue(x.CapturedPiece.Type));
        }
    }
}
