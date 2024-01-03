using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ChessLibrary
{
    public class ZobristTable
    {
        private static ulong[,] _table = new ulong[20, 64];
        const int COLOR_TO_MOVE_INDEX = 12;
        const int EN_PASSANT_INDEX = 13;
        const int CASTLING_INDEX = 14;

        static ZobristTable()
        {
            // Generating a zobrist table is very fast, so we could easily do it at runtime.
            // However, that breaks the opening book moveset since they're hash based
            var filePath = Path.Combine("zobrist", "zobrist.json");
            if (!Directory.Exists("zobrist"))
            {
                Directory.CreateDirectory("zobrist");
            }
            if (File.Exists(filePath))
            {
                _table = JsonConvert.DeserializeObject<ulong[,]>(File.ReadAllText(filePath))!;
                return;
            }
            Random r = new Random();
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    _table[i, j] = GetRandomLong(r);
                }
            }
            File.WriteAllText(filePath, JsonConvert.SerializeObject(_table));
        }

        private static ulong GetRandomLong(Random rnd)
        {
            var buffer = new byte[sizeof(ulong)];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static ulong CalculateZobristHash(Game game)
        {
            ulong hash = 0;
            int squareIndex = 0;
            for (var f = Files.A; f <= Files.H; f++)
            {
                for (int r = 1; r <= 8; r++)
                {
                    var piece = game.Board.GetSquare(f, r).Piece;
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
                        if (piece.Color == Colors.Black)
                        {
                            pieceIndex += 6;
                        }
                        hash ^= _table[pieceIndex, squareIndex];
                    }
                    squareIndex++;
                }
            }
            if (game.PlayerToMove == Colors.Black)
            {
                hash ^= _table[COLOR_TO_MOVE_INDEX, 0];
            }
            if (game.EnPassantFile != null)
            {
                hash ^= _table[EN_PASSANT_INDEX, (int)game.EnPassantFile - 1];
            }
            int castle = 0;
            castle |= game.WhiteCanLongCastle ? 0b0001 : 0;
            castle |= game.WhiteCanShortCastle ? 0b0010 : 0;
            castle |= game.BlackCanLongCastle ? 0b0100 : 0;
            castle |= game.BlackCanShortCastle ? 0b1000 : 0;
            hash ^= _table[CASTLING_INDEX, castle];
            return hash;
        }
    }
}
