using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Piece : ICloneable
    {
        public PieceTypes Type { get; set; }
        public Colors Color { get; set; }

        public object Clone()
        {
            return new Piece() { Type = this.Type, Color = this.Color };
        }
    }
}
