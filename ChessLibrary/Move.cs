using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Move : ICloneable
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

            bool promotedPiecesMatch = comp.PromotedPiece == null || PromotedPiece == null
                                        ? PromotedPiece == null && comp.PromotedPiece == null
                                        : comp.PromotedPiece.Color == PromotedPiece.Color && PromotedPiece.Type == comp.PromotedPiece.Type;

            return comp.Player == Player 
                    && comp.StartingSquare.File == StartingSquare.File 
                    && StartingSquare.Rank == comp.StartingSquare.Rank 
                    && comp.DestinationSquare.File == DestinationSquare.File 
                    && DestinationSquare.Rank == comp.DestinationSquare.Rank
                    && promotedPiecesMatch;
        }

        public override int GetHashCode()
        {
            return 17 * (int)StartingSquare.File + 18 * StartingSquare.Rank + 34 * (int)DestinationSquare.File + 79 * DestinationSquare.Rank
                      + (PromotedPiece == null ? 0 : 103 * (int)PromotedPiece.Type + 151 * (int)PromotedPiece.Color);
        }

        public object Clone()
        {
            return new Move((Piece)Piece.Clone(), Player, (Square)StartingSquare.Clone(), (Square)DestinationSquare.Clone())
            {
                CapturedPiece = CapturedPiece == null ? null : (Piece)CapturedPiece.Clone(),
                PromotedPiece = PromotedPiece == null ? null : (Piece)PromotedPiece.Clone()
            };
        }
    }
}
