using ChessLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Piece
    {
        public PieceTypes Type { get; set; }
        public Colors Color { get; set; }
        public int Score
        {
            get
            {
                switch (Type)
                {
                    case PieceTypes.Pawn:
                        return 1;
                    case PieceTypes.Knight:
                    case PieceTypes.Bishop:
                        return 3;
                    case PieceTypes.Rook:
                        return 5;
                    case PieceTypes.Queen:
                        return 9;
                    case PieceTypes.King:
                        return 100;
                }
                return 0;
            }
        }
    }
}
