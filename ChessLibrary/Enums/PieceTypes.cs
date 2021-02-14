using ChessLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public enum PieceTypes
    {
        [PieceScore(1)]
        Pawn,

        [PieceScore(5)]
        Rook,

        [PieceScore(3)]
        Knight,

        [PieceScore(3)]
        Bishop,

        [PieceScore(9)]
        Queen,
        King
    }
}
