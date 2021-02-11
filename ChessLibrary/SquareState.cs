using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class SquareState
    {
        public Square Square { get; set; }
        public Piece? Piece { get; set; }

        public SquareState(Square square)
        {
            Square = square;
        }
    }
}
