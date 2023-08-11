using System.Collections.Generic;

namespace ChessLibrary
{
    public interface IMoveLegality
    {
        Move[] GetAllLegalMoves(IBoard b, Colors color, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool includeQuietMoves = true);
        Move[]? GetAllLegalMoves(IBoard b, SquareState squareState, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool ignoreCheck = false, bool includeQuietMoves = true);
        bool IsKingInCheck(IBoard b, Colors color, List<Move> pastMoves);
    }
}