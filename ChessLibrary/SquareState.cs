using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class SquareState
    {
        public static SquareState[][][] SquareStateMap;

        private SquareState() { }

        static SquareState()
        {
            SquareStateMap = new SquareState[64][][];
            Array colors = Enum.GetValues(typeof(Colors));

            for (int i = 0; i <= 63; i++)
            {
                SquareStateMap[i] = new SquareState[2][];
                foreach (Colors c in colors)
                {
                    SquareStateMap[i][(int)c] = new SquareState[6];
                    Array pieces = Enum.GetValues(typeof(PieceTypes));
                    foreach (PieceTypes pieceType in pieces)
                    {
                        SquareStateMap[i][(int)c][(int)pieceType - 1] = new SquareState(
                            new Square(i)
                        )
                        {
                            Piece = Piece.Pieces[(int)c][(int)pieceType - 1]
                        };
                    }
                }
            }
        }

        public Square Square
        {
            get { return GetSquare(_valueInternal); }
            set { _valueInternal = (ushort)((~(63 << 6) & _valueInternal) | GetSquare(value)); }
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
                return Piece.Pieces[color][piece];
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

        public SquareState(Square square)
        {
            Square = square;
        }

        private static ushort GetSquare(Square square)
        {
            ushort squareNumber = (ushort)(((square.Rank - 1) * 8) + (8 - (int)square.File));
            return (ushort)(squareNumber << 6);
        }


        private static Square[] _squares = Enumerable.Range(0, 64).Select(i => new Square(i)).ToArray();

        private static Square GetSquare(ushort value)
        {
            //if (!_squaresInitialized)
            //{
            //    for (int i = 0; i <= 63; i++)
            //    {
            //        _squares[i] = new Square(i);
            //    }
            //    _squaresInitialized = true;
            //}
            ushort squareNumber = (ushort)(value >> 6);
            return _squares[squareNumber];
        }

        //private static Square[] _squares = new Square[64];
        //private static bool _squaresInitialized = false;
    }
}
