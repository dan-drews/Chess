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
                            _score = 1;
                            break;
                        case PieceTypes.Knight:
                        case PieceTypes.Bishop:
                            _score = 3;
                            break;
                        case PieceTypes.Rook:
                            _score = 5;
                            break;
                        case PieceTypes.Queen:
                            _score = 9;
                            break;
                        case PieceTypes.King:
                            _score = 100;
                            break;
                    }
                    _score = 0;
                }
                return _score.Value;
            }
        }
    }
}
