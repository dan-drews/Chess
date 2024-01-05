using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ChessLibrary.Boards;
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
            for (int i = 0; i < 12; i++)
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
            var board = game.Board;
            hash ^= GetPieceHash(board.WhitePawns, 0);
            hash ^= GetPieceHash(board.WhiteBishops, 1);
            hash ^= GetPieceHash(board.WhiteKnights, 2);
            hash ^= GetPieceHash(board.WhiteRooks, 3);
            hash ^= GetPieceHash(board.WhiteQueens, 4);
            hash ^= GetPieceHash(board.WhiteKing, 5);

            hash ^= GetPieceHash(board.BlackPawns, 0);
            hash ^= GetPieceHash(board.BlackBishops, 1);
            hash ^= GetPieceHash(board.BlackKnights, 2);
            hash ^= GetPieceHash(board.BlackRooks, 3);
            hash ^= GetPieceHash(board.BlackQueens, 4);
            hash ^= GetPieceHash(board.BlackKing, 5);

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

        private static ulong GetPieceHash(BitBoard pieces, int pieceIndex)
        {
            ulong hash = 0;
            var enumerator = pieces.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int position = enumerator.Current;
                hash ^= _table[pieceIndex, position];
            }
            return hash;
        }
    }
}
