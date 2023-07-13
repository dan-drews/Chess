using System;

namespace ChessLibrary
{
    public interface IBoard : ICloneable
    {
        SquareState GetSquare(Files file, int rank);
        SquareState GetSquare(int position);
        void MovePiece(NewMove move);
        void SetupBoard();
        void SetPiece(Files f, int rank, PieceTypes type, Colors color);
        void SetPiece(int position, PieceTypes type, Colors color);
        void ClearPiece(Files f, int rank);
        void ClearPiece(int position);
    }
}