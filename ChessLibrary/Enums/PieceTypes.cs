using ChessLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public enum PieceTypes
    {
        [PieceScore(1)]
        Pawn = 1,

        [PieceScore(5)]
        Rook = 2,

        [PieceScore(3)]
        Knight = 3,

        [PieceScore(3)]
        Bishop = 4,

        [PieceScore(9)]
        Queen = 5,
        King = 6
    }
}
