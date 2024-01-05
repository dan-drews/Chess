using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public class MatchParser
    {
        public void LoadGameFromChessNotation(Game game, string chessNotation)
        {
            var gameMoves = chessNotation.Split(' ');
            foreach (var move in gameMoves[0..^2])
            {
                game.AddMove(GetMoveFromChessNotation(game, move), false);
            }
        }

        public Move GetMoveFromChessNotation(Game game, string move)
        {
            switch (move[0])
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                    // Pawn move
                    return GetPawnMove(game, move);
                case 'N':
                case 'B':
                case 'Q':
                case 'R':
                case 'K':
                    return GetStandardMove(game, move);
                case 'O':
                    return GetCastle(game, move);
            }
            throw new Exception();
        }

        private Move GetCastle(Game game, string move)
        {
            if (move == "O-O" || move == "O-O+" || move == "O-O#")
            {
                return game.GetAllLegalMoves().First(x => x.Flags == Flag.ShortCastle);
            }
            return game.GetAllLegalMoves().First(x => x.Flags == Flag.LongCastle);
        }

        private Move GetStandardMove(Game game, string move)
        {
            PieceTypes piece = GetPieceType(move[0]);
            bool isCapture = move.Contains('x');
            bool isCheck = move.EndsWith('+');
            bool isCheckmate = move.EndsWith("#");

            var targetSquareString = isCheck || isCheckmate ? move[^3..^1] : move[^2..];
            var targetSquare = GetSquareFromMove(targetSquareString);
            var legalMoves = game.GetAllLegalMoves();
            IEnumerable<Move> candidateMoves = legalMoves
                .Where(x => x.Piece == piece)
                .Where(x => x.TargetSquare == targetSquare)
                .Where(x => isCapture ? x.CapturedPiece != null : x.CapturedPiece == null);

            if (candidateMoves.Count() > 1)
            {
                bool hasFullSquareQualifier = isCapture
                    ? move.Split('x').Length == 3
                    : move.IndexOf(targetSquareString) == 3;
                if (hasFullSquareQualifier)
                {
                    var startingSquareString = move[1..2];
                    var startingSquare = GetSquareFromMove(startingSquareString);
                    candidateMoves = candidateMoves.Where(x => x.StartingSquare == startingSquare);
                }
                else
                {
                    var rankOrFile = move[1].ToString();
                    if (int.TryParse(rankOrFile, out int rank))
                    {
                        candidateMoves = candidateMoves.Where(
                            x => new Square(x.StartingSquare).Rank == rank
                        );
                    }
                    else
                    {
                        var file = GetFileFromString(move[1]);
                        candidateMoves = candidateMoves.Where(
                            x => new Square(x.StartingSquare).File == file
                        );
                    }
                }
            }
            return candidateMoves.First();
        }

        private static PieceTypes GetPieceType(char key)
        {
            return key switch
            {
                'N' => PieceTypes.Knight,
                'B' => PieceTypes.Bishop,
                'Q' => PieceTypes.Queen,
                'R' => PieceTypes.Rook,
                'K' => PieceTypes.King,
                _ => throw new NotImplementedException()
            };
        }

        private Move GetPawnMove(Game game, string move)
        {
            // TODO: Promotion
            PieceTypes? promotedPiece = null;
            if (move.Contains('='))
            {
                promotedPiece = GetPieceType(move.Split('=')[1][0]);
            }

            bool isCapture = move.Contains('x');
            bool isCheck = move.EndsWith('+');
            bool isCheckmate = move.EndsWith("#");
            bool isPromotion = promotedPiece != null;

            char? startingColumn = null;

            string targetSquareString;
            if (isPromotion)
            {
                targetSquareString = move.Split('=')[0][^2..];
            }
            else if (isCheck || isCheckmate)
            {
                targetSquareString = move[^3..^1];
            }
            else
            {
                targetSquareString = move[^2..];
            }

            var legalMoves = game.GetAllLegalMoves();
            IEnumerable<Move> candidateMoves = legalMoves.Where(x => x.Piece == PieceTypes.Pawn);
            var destinationSquare = GetSquareFromMove(targetSquareString);
            candidateMoves = candidateMoves
                .Where(x => x.TargetSquare == destinationSquare)
                .Where(x => x.PromotedType == promotedPiece)
                .Where(x => isCapture ? x.CapturedPiece != null : x.CapturedPiece == null);

            if (candidateMoves.Count() > 1)
            {
                startingColumn = move[0];
                candidateMoves = candidateMoves.Where(
                    x => new Square(x.StartingSquare).File == GetFileFromString(startingColumn)
                );
            }
            return candidateMoves.First();
            //throw new Exception("Unable to find proper move");
        }

        private Files GetFileFromString(char? file) =>
            file switch
            {
                'a' => Files.A,
                'b' => Files.B,
                'c' => Files.C,
                'd' => Files.D,
                'e' => Files.E,
                'f' => Files.F,
                'g' => Files.G,
                'h' => Files.H,
                _ => throw new NotImplementedException()
            };

        private int GetSquareFromMove(string move)
        {
            Files file = GetFileFromString(move[0]);

            int rank = int.Parse(move[1].ToString());
            var sq = new Square { File = file, Rank = rank, };
            return sq.SquareNumber;
        }
    }
}
