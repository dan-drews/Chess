using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    class ZobristTable
    {
        private static ulong[,] _table = new ulong[12, 64];

        static ZobristTable()
        {
            Random r = new Random();
            for(int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    _table[i, j] = GetRandomLong(r);
                }
            }
        }

        private static ulong GetRandomLong(Random rnd)
        {
            var buffer = new byte[sizeof(ulong)];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static ulong CalculateZobristHash(IBoard board)
        {
            ulong hash = 0;
            int squareIndex = 0;
            for (var f = Files.A; f <= Files.H; f++)
            {
                for (int r = 1; r <= 8; r++)
                {
                    var piece = board.GetSquare(f, r).Piece;
                    if (piece != null)
                    {
                        int pieceIndex = 0;
                        switch (piece.Type)
                        {
                            case PieceTypes.Pawn:
                                pieceIndex = 0;
                                break;
                            case PieceTypes.Bishop:
                                pieceIndex = 1;
                                break;
                            case PieceTypes.Knight:
                                pieceIndex = 2;
                                break;
                            case PieceTypes.Rook:
                                pieceIndex = 3;
                                break;
                            case PieceTypes.Queen:
                                pieceIndex = 4;
                                break;
                            case PieceTypes.King:
                                pieceIndex = 5;
                                break;
                        }
                        if(piece.Color == Colors.Black)
                        {
                            pieceIndex += 6;
                        }
                        hash ^= _table[pieceIndex, squareIndex];
                    }
                    squareIndex++;
                }
            }
            return hash;
        }
    }
}
