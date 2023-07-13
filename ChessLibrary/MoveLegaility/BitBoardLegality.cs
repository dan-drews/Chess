using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary.MoveLegaility
{
    public class BitBoardLegality : IMoveLegality
    {
        public List<NewMove> GetAllLegalMoves(IBoard b, Colors color, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool includeQuietMoves = true)
        {
            var board = (BitBoard)b;
            return board.AllValidMoves(color, enPassantFile, blackCanLongCastle, blackCanShortCastle, whiteCanLongCastle, whiteCanShortCastle, false, includeQuietMoves);
        }

        public List<NewMove>? GetAllLegalMoves(IBoard b, SquareState squareState, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool ignoreCheck = false, bool includeQuietMoves = true)
        {
            var allValidMoves = ((BitBoard)b).AllValidMoves(squareState.Piece!.Color, enPassantFile, blackCanLongCastle, blackCanShortCastle, whiteCanLongCastle, whiteCanShortCastle, ignoreCheck, includeQuietMoves);
            return allValidMoves.Where(x => x.StartingSquare == squareState.Square.SquareNumber).ToList();
        }

        public bool IsKingInCheck(IBoard b, Colors color, List<NewMove> pastMoves)
        {
            return ((BitBoard)b).IsKingInCheck(color);
        }
    }
}
