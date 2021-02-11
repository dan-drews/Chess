using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public static class Extensions
    {
        public static string GetNotation(this PieceTypes p)
        {
            switch (p)
            {
                case PieceTypes.Bishop:
                    return "B";
                case PieceTypes.King:
                    return "K";
                case PieceTypes.Knight:
                    return "N";
                case PieceTypes.Pawn:
                    return "P";
                case PieceTypes.Queen:
                    return "Q";
                case PieceTypes.Rook:
                    return "R";
            }
            return "";
        }
    }
}
