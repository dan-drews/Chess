using System.Collections.Generic;

namespace ChessLibrary
{
    public interface IMoveLegality
    {
        List<Move> GetAllLegalMoves(IBoard b, Colors color, List<Move> pastMoves);
        List<Move>? GetAllLegalMoves(IBoard b, SquareState squareState, List<Move> pastMoves, bool ignoreCheck = false);
        bool IsKingInCheck(IBoard b, Colors color, List<Move> pastMoves);
    }
}