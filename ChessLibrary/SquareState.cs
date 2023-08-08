using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class SquareState
    {
        private static Piece[][] _pieces;

        static SquareState()
        {
            _pieces = new Piece[][] {
                new Piece[]
                {
                    new Piece()
                    {
                        Color = Colors.White,
                        Type = PieceTypes.Pawn
                    },
                    new Piece()
                    {
                        Color = Colors.White,
                        Type = PieceTypes.Rook
                    },
                    new Piece()
                    {
                        Color = Colors.White,
                        Type = PieceTypes.Knight
                    },
                    new Piece()
                    {
                        Color = Colors.White,
                        Type = PieceTypes.Bishop
                    },
                    new Piece()
                    {
                        Color = Colors.White,
                        Type = PieceTypes.Queen
                    },
                    new Piece()
                    {
                        Color = Colors.White,
                        Type = PieceTypes.King
                    }
                },
                new Piece[]
                {
                    new Piece()
                    {
                        Color = Colors.Black,
                        Type = PieceTypes.Pawn
                    },
                    new Piece()
                    {
                        Color = Colors.Black,
                        Type = PieceTypes.Rook
                    },
                    new Piece()
                    {
                        Color = Colors.Black,
                        Type = PieceTypes.Knight
                    },
                    new Piece()
                    {
                        Color = Colors.Black,
                        Type = PieceTypes.Bishop
                    },
                    new Piece()
                    {
                        Color = Colors.Black,
                        Type = PieceTypes.Queen
                    },
                    new Piece()
                    {
                        Color = Colors.Black,
                        Type = PieceTypes.King
                    }
                }
            };
        }

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
                if ((_valueInternal & 0b10000) == 0)
                {
                    return null;
                }
                var color = (_valueInternal & 0b1000) == 0 ? 0 : 1;
                var piece = (_valueInternal & 0b111) - 1;
                return _pieces[color][piece]; ;
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

        public ushort _valueInternal { get; set; }

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
            ushort squareNumber = (ushort)(((square.Rank - 1) * 8) + (8 - (int)square.File));
            return (ushort)(squareNumber << 6);
        }

        private static Square GetSquare(ushort value)
        {
            if (!_squaresInitialized)
            {
                for (int i = 0; i <= 63; i++)
                {
                    _squares[i] = new Square(i);
                }
                _squaresInitialized = true;
            }
            ushort squareNumber = (ushort)(value >> 6);
            return _squares[squareNumber];
        }

        private static Square[] _squares = new Square[64];
        private static bool _squaresInitialized = false;
    }
}
