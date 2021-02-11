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

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var comp = obj as Move;
            if (comp == null)
            {
                return false;
            }

            return comp.Player == Player && comp.StartingSquare.File == StartingSquare.File && StartingSquare.Rank == comp.StartingSquare.Rank && comp.DestinationSquare.File == DestinationSquare.File && DestinationSquare.Rank == comp.DestinationSquare.Rank;
        }

        public override int GetHashCode()
        {
            return 17 * (int)StartingSquare.File + 18 * StartingSquare.Rank + 34 * (int)DestinationSquare.File * 79 * DestinationSquare.Rank;
        }

    }
}
