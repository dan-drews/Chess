using System.Collections.Generic;

namespace ChessLibrary
{
    public interface IMoveGenerator
    {
        Move[] GetAllLegalMoves(
            FullBitBoard b,
            Colors color,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle,
            bool includeQuietMoves = true
        );
        Move[]? GetAllLegalMoves(
            FullBitBoard b,
            SquareState squareState,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle,
            bool ignoreCheck = false,
            bool includeQuietMoves = true
        );
        public bool HasAnyLegalMoves(
            FullBitBoard b,
            Colors color,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle
        );
        bool IsKingInCheck(FullBitBoard b, Colors color);
    }
}
