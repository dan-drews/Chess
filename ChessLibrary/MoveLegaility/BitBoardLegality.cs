using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary.MoveLegaility
{
    public class BitBoardLegality : IMoveLegality
    {
        public List<Move> GetAllLegalMoves(IBoard b, Colors color, List<Move> pastMoves)
        {
            var board = (BitBoard)b;
            return board.AllValidMoves(color, pastMoves);
        }

        public List<Move>? GetAllLegalMoves(IBoard b, SquareState squareState, List<Move> pastMoves, bool ignoreCheck = false)
        {
            var allValidMoves = ((BitBoard)b).AllValidMoves(squareState.Piece!.Color, pastMoves);
            return allValidMoves.Where(x => x.StartingSquare.Rank == squareState.Square.Rank && x.StartingSquare.File == squareState.Square.File).ToList();
        }

        public bool IsKingInCheck(IBoard b, Colors color, List<Move> pastMoves)
        {
            return ((BitBoard)b).IsKingInCheck(color);
        }
    }
}
