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

        private int? _score = null;
        
        public int Score
        {
            get
            {
                if (_score == null || _score == 0)
                {
                    switch (Type)
                    {
                        case PieceTypes.Pawn:
                            _score = 2;
                            break;
                        case PieceTypes.Knight:
                        case PieceTypes.Bishop:
                            _score = 8;
                            break;
                        case PieceTypes.Rook:
                            _score = 13;
                            break;
                        case PieceTypes.Queen:
                            _score = 21;
                            break;
                        case PieceTypes.King:
                            _score = 100;
                            break;
                        default:
                            _score = 0;
                            break;
                    }
                }
                return _score.Value;
            }
        }
    }
}
