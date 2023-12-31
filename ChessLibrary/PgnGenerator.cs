using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public static class PgnGenerator
    {
        public static string GeneratePgnFromMoveList(List<Move> moves)
        {
            var game = new Game(Enums.BoardType.BitBoard);
            game.ResetGame();
            var sb = new StringBuilder();
            int counter = 1;
            bool firstMoveOfHalf = true;
            foreach (var move in moves)
            {
                if (firstMoveOfHalf)
                {
                    sb.Append($"{counter}. ");
                }
                sb.Append(GetMoveAlgebraicNotation(game, move));
                sb.Append(" ");
                if (!firstMoveOfHalf)
                {
                    counter++;
                }
                firstMoveOfHalf = !firstMoveOfHalf;
                game.AddMove(move, false);
            }
            return sb.ToString();
        }

        private static string GetMoveAlgebraicNotation(Game game, Move move)
        {
            return move.Piece switch
            {
                PieceTypes.Pawn => GetPawnMoveAlgebraicNotation(move),
                PieceTypes.Knight => GetGeneralMoveAlgebraicNotation(game, move),
                PieceTypes.Bishop => GetGeneralMoveAlgebraicNotation(game, move),
                PieceTypes.Rook => GetGeneralMoveAlgebraicNotation(game, move),
                PieceTypes.Queen => GetGeneralMoveAlgebraicNotation(game, move),
                PieceTypes.King => GetKingMoveAlgebraicNotation(move),
                _ => string.Empty
            };
        }

        private static string GetKingMoveAlgebraicNotation(Move move)
        {
            if (move.Flags == Flag.LongCastle)
            {
                return "O-O-O";
            }

            if (move.Flags == Flag.ShortCastle)
            {
                return "O-O";
            }

            return GetStandardNotation(move);
        }

        private static string GetGeneralMoveAlgebraicNotation(Game game, Move move)
        {
            var duplicateMoves = GetDuplicateMoves(game, move);
            if (!duplicateMoves.Any())
            {
                return GetStandardNotation(move);
            }
            return GetDuplicateMoveAlgebraicNotation(game, move);
        }

        private static IEnumerable<Move> GetDuplicateMoves(Game game, Move move)
        {
            var legalMoves = game.GetAllLegalMoves();
            var targetSquare = new Square(move.TargetSquare);
            return legalMoves
                .Where(x => x.Piece == move.Piece)
                .Where(x => x.TargetSquare == move.TargetSquare)
                .Where(x => x.StartingSquare != move.StartingSquare);
        }

        private static string GetDuplicateMoveAlgebraicNotation(Game game, Move move)
        {
            var duplicateMoves = GetDuplicateMoves(game, move);

            var startingSquare = new Square(move.StartingSquare);
            var targetSquare = new Square(move.TargetSquare);
            var startingFile = startingSquare.File;
            var startingRank = startingSquare.Rank;
            var targetFile = targetSquare.File;
            var targetRank = targetSquare.Rank;

            var sameRankMoves = duplicateMoves.Where(
                x => new Square(x.StartingSquare).Rank == startingRank
            );
            var sameFileMoves = duplicateMoves.Where(
                x => new Square(x.StartingSquare).File == startingFile
            );

            if (sameRankMoves.Any())
            {
                if (!sameFileMoves.Any())
                {
                    return GetPieceIndentifier(move)
                        + startingFile.ToString().ToLower()
                        + GetTargetSquareFromMove(move);
                }
                return GetPieceIndentifier(move)
                    + startingFile.ToString().ToLower()
                    + startingRank.ToString()
                    + GetTargetSquareFromMove(move);
            }
            return GetPieceIndentifier(move)
                + startingRank.ToString().ToLower()
                + GetTargetSquareFromMove(move);
        }

        private static string GetTargetSquareFromMove(Move move)
        {
            var targetSquare = new Square(move.TargetSquare);
            if (move.CapturedPiece == null)
            {
                return targetSquare.File.ToString().ToLower() + targetSquare.Rank.ToString();
            }
            return "x" + targetSquare.File.ToString().ToLower() + targetSquare.Rank.ToString();
        }

        private static string GetStandardNotation(Move move)
        {
            return GetPieceIndentifier(move) + GetTargetSquareFromMove(move);
        }

        // TODO figure out how to handle check detection in this. Do I need to track it on the move after it's made?

        private static string GetPawnMoveAlgebraicNotation(Move move)
        {
            var targetSquare = new Square(move.TargetSquare);
            var startingSquare = new Square(move.StartingSquare);

            if (move.CapturedPiece == null)
            {
                if (move.PromotedType == null)
                {
                    return targetSquare.File.ToString().ToLower() + targetSquare.Rank.ToString();
                }
                return targetSquare.File.ToString().ToLower()
                    + targetSquare.Rank.ToString()
                    + "="
                    + GetPromotedPieceName(move);
            }

            if (move.PromotedType == null)
            {
                return startingSquare.File.ToString().ToLower()
                    + "x"
                    + targetSquare.File.ToString().ToLower()
                    + targetSquare.Rank.ToString();
            }

            return startingSquare.File.ToString().ToLower()
                + "x"
                + targetSquare.File.ToString().ToLower()
                + targetSquare.Rank.ToString()
                + "="
                + GetPromotedPieceName(move);
        }

        private static string GetPromotedPieceName(Move move)
        {
            return move.PromotedType switch
            {
                PieceTypes.Bishop => "B",
                PieceTypes.Knight => "N",
                PieceTypes.Queen => "Q",
                PieceTypes.Rook => "R",
                _ => string.Empty
            };
        }

        private static string GetPieceIndentifier(Move move)
        {
            return move.Piece switch
            {
                PieceTypes.Bishop => "B",
                PieceTypes.Knight => "N",
                PieceTypes.Queen => "Q",
                PieceTypes.Rook => "R",
                PieceTypes.King => "K",
                _ => string.Empty
            };
        }
    }
}
