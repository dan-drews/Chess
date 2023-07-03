using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class SquareState
    {
        public Square Square
        {
            get
            {
                return GetSquare(_valueInternal);
            }
            set
            {
                _valueInternal = (ushort)((~(63 << 6) & _valueInternal) | GetSquare(value));
            }
        }
        public Piece? Piece
        {
            get
            {
                if((_valueInternal & 0b10000) == 0)
                {
                    return null;
                }
                var color = (_valueInternal & 0b1000) == 0 ? Colors.White : Colors.Black;
                var piece = (PieceTypes)(_valueInternal & 0b111);
                return new Piece()
                {
                    Color = color,
                    Type = piece
                };
            }
            set
            {
                if (value == null)
                {
                    _valueInternal = (ushort)(_valueInternal & (63 << 6));
                    return;
                }
                _valueInternal = (ushort)(_valueInternal | 0b10000);
                if (value.Color == Colors.Black)
                {
                    _valueInternal = (ushort)(_valueInternal | 0b1000);
                }
                _valueInternal = (ushort)(_valueInternal | (ushort)value.Type);
            }
        }

        private ushort _valueInternal;

        public SquareState(ushort value)
        {
            _valueInternal = value;
        }

        public SquareState(Square square)
        {
            Square = square;
        }

        private static ushort GetSquare(Square square)
        {
            ushort squareNumber = (ushort)(((square.Rank - 1) * 8) + ((int)square.File - 1));
            return (ushort)(squareNumber << 6);
        }

        private static Square GetSquare(ushort value)
        {
            ushort squareNumber = (ushort)(value >> 6);

            return new Square()
            {
                File = (Files)((squareNumber % 8) + 1),
                Rank = (squareNumber / 8) + 1
            };
        }
    }
}
