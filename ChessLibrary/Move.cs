using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Move
    {
        public Piece Piece { get; set; }
        public Colors Player { get; set; }
        public Square StartingSquare { get; set; }
        public Square DestinationSquare { get; set; }
        public Piece? CapturedPiece { get; set; }

        public Piece? PromotedPiece { get; set; }

        public Move(Piece piece, Colors player, Square startingSquare, Square destinationSquare)
        {
            Piece = piece;
            Player = player;
            StartingSquare = startingSquare;
            DestinationSquare = destinationSquare;
        }

    }
}
