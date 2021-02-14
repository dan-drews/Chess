using ChessLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Piece : ICloneable
    {
        public PieceTypes Type { get; set; }
        public Colors Color { get; set; }

        private int? _score = null;
        public int Score
        {
            get
            {
                if(_score == null)
                {
                    _score = Type.GetAttributeOfType<PieceScoreAttribute>()?.Score ?? 0;
                }
                return _score.Value;
            }
            private set
            {
                _score = value;
            }
        }

        public object Clone()
        {
            return new Piece() { Type = this.Type, Color = this.Color, Score = this.Score };
        }
    }
}
