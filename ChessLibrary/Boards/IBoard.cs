using System;

namespace ChessLibrary
{
    public interface IBoard : ICloneable
    {
        SquareState GetSquare(Files file, int rank);
        void MovePiece(Move move);
        void SetupBoard();
        void SetPiece(Files f, int rank, PieceTypes type, Colors color);
        void ClearPiece(Files f, int rank);
    }
}