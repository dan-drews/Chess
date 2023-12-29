using System.Collections.Generic;

namespace ChessLibrary
{
    public interface IMoveGenerator
    {
        Move[] GetAllLegalMoves(BitBoard b, Colors color, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool includeQuietMoves = true);
        Move[]? GetAllLegalMoves(BitBoard b, SquareState squareState, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool ignoreCheck = false, bool includeQuietMoves = true);
        bool IsKingInCheck(BitBoard b, Colors color);
    }
}