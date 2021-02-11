using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public static class MoveLegalityEvaluator
    {
        public static List<Move>? GetAllLegalMoves(Board b, SquareState squareState, bool ignoreCheck = false)
        {
            if (squareState.Piece == null)
            {
                return null;
            }

            var piece = squareState.Piece;
            var color = piece.Color;
            var square = squareState.Square;

            List<Move> potentialMoves;

            int pawnDirection = color == Colors.White ? 1 : -1;
            switch (piece.Type)
            {
                case PieceTypes.Pawn:
                    potentialMoves = GetValidPawnMoves(b, squareState, piece, color, square, pawnDirection);
                    break;
                case PieceTypes.Knight:
                    potentialMoves = GetValidKnightMoves(b, piece, color, square);
                    break;
                case PieceTypes.Rook:
                    potentialMoves = GetValidRookMoves(b, piece, color, square);
                    break;
                case PieceTypes.Bishop:
                    potentialMoves = GetValidBishopMoves(b, piece, color, square);
                    break;
                case PieceTypes.Queen:
                    potentialMoves = GetValidQueenMoves(b, piece, color, square);
                    break;
                case PieceTypes.King:
                    potentialMoves = GetValidKingMoves(b, piece, color, square);
                    break;
                default:
                    throw new Exception("Piece Move not Yet Handled");

            }

            if (ignoreCheck)
            {
                return potentialMoves;
            }

            var result = new List<Move>();

            foreach (var move in potentialMoves)
            {

                var tempBoard = (Board)b.Clone();
                var initialPiece = tempBoard.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece;
                tempBoard.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = null;
                tempBoard.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = initialPiece;
                // find the king after the move.
                SquareState kingSquareState = GetKingSquare(tempBoard, color);

                // Loop over all squares. If oponent is in square, see if opponent can legally attack king.
                bool isInCheck = KingIsInCheck(color, tempBoard, kingSquareState);
                if (!isInCheck)
                {
                    result.Add(move);
                }
            }

            return result;
        }
        private static List<Move> GetValidAdjacentMoves(Board b, Piece piece, Colors color, Square square)
        {
            return new List<Move>();
        }

        private static List<Move> GetValidDiagonalMoves(Board b, Piece piece, Colors color, Square square)
        {
            return new List<Move>();
        }

        private static List<Move> GetValidRookMoves(Board b, Piece piece, Colors color, Square square)
        {
            return GetValidAdjacentMoves(b, piece, color, square);
        }


        private static List<Move> GetValidBishopMoves(Board b, Piece piece, Colors color, Square square)
        {
            return GetValidDiagonalMoves(b, piece, color, square);
        }

        private static List<Move> GetValidKingMoves(Board b, Piece piece, Colors color, Square square)
        {
            // ToDo - Castling
            return new List<Move>();
        }

        private static List<Move> GetValidQueenMoves(Board b, Piece piece, Colors color, Square square)
        {
            return GetValidAdjacentMoves(b, piece, color, square).Concat(GetValidDiagonalMoves(b, piece, color, square)).ToList();
        }

        private static List<Move> GetValidKnightMoves(Board b, Piece piece, Colors color, Square square)
        {
            List<Move> potentialMoves = new List<Move>();
            var targets = new (Files file, int rank)[]
            {
                (square.File - 1, square.Rank - 2),
                (square.File - 1, square.Rank + 2),

                (square.File + 1, square.Rank - 2),
                (square.File + 1, square.Rank + 2),

                (square.File - 2, square.Rank - 1),
                (square.File - 2, square.Rank + 1),

                (square.File + 2, square.Rank - 1),
                (square.File + 2, square.Rank + 1),
            };

            foreach (var t in targets)
            {
                if (t.file >= Files.A && t.file <= Files.H && t.rank >= 1 && t.rank <= 8)
                {
                    var targetSquare = b.GetSquare(t.file, t.rank);
                    if (targetSquare.Piece == null || targetSquare.Piece.Color != color)
                    {
                        potentialMoves.Add(new Move(piece, color, square, targetSquare.Square)
                        {
                            CapturedPiece = targetSquare.Piece
                        });
                    }
                }
            }

            return potentialMoves;
        }

        private static List<Move> GetValidPawnMoves(Board b, SquareState squareState, Piece piece, Colors color, Square square, int pawnDirection)
        {
            List<Move> potentialMoves = new List<Move>();
            // Pawn can move 1 move forward if:
            // 1) It is unobstructed
            // 2) A square exists.... can't move off the board.

            // Pawn can move 2 moves forward if:
            // 1) It in either rank 2 or 7 depending on color
            // 2) No piece is either 1 more forward or 2 moves forward

            if (square.Rank + pawnDirection >= 1 && square.Rank + pawnDirection <= 8)
            {
                var squareForward = b.GetSquare(squareState.Square.File, squareState.Square.Rank + pawnDirection);
                if (squareForward.Piece == null)
                {
                    potentialMoves.Add(new Move(piece, color, square, squareForward.Square));

                    if ((color == Colors.White && square.Rank == 2) || (color == Colors.Black && square.Rank == 7))
                    {
                        var twoSquaresForward = b.GetSquare(squareState.Square.File, squareState.Square.Rank + (pawnDirection * 2));
                        if (twoSquaresForward.Piece == null)
                        {
                            potentialMoves.Add(new Move(piece, color, square, twoSquaresForward.Square));
                        }
                    }
                }

                // Pawn can move 1 square forward diagonally if:
                // 1) There is an enemy piece in that square
                // 2) A square exists.... can't move off the board.
                int[] diagonalDirections;
                switch (square.File)
                {
                    case Files.A:
                        diagonalDirections = new int[] { 1 };
                        break;
                    case Files.H:
                        diagonalDirections = new int[] { -1 };
                        break;
                    default:
                        diagonalDirections = new int[] { -1, 1 };
                        break;
                }
                foreach (int direction in diagonalDirections)
                {
                    var diagonalSquare = b.GetSquare(squareState.Square.File + direction, squareState.Square.Rank + pawnDirection);
                    if (diagonalSquare.Piece != null && diagonalSquare.Piece.Color != color)
                    {
                        potentialMoves.Add(new Move(piece, color, square, diagonalSquare.Square)
                        {
                            CapturedPiece = diagonalSquare.Piece
                        });
                    }
                }
            }

            // TODO - En Passant

            return potentialMoves;
        }

        private static bool KingIsInCheck(Colors color, Board tempBoard, SquareState kingSquareState)
        {
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var squareChecking = tempBoard.GetSquare(file, rank);
                    if (squareChecking.Piece != null && squareChecking.Piece.Color != color)
                    {
                        var moves = GetAllLegalMoves(tempBoard, squareChecking, true);
                        if (moves != null && moves.Any(move => move.DestinationSquare.Rank == kingSquareState.Square.Rank && move.DestinationSquare.File == kingSquareState.Square.File))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static SquareState GetKingSquare(Board b, Colors color)
        {
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var squareChecking = b.GetSquare(file, rank);
                    if (squareChecking.Piece != null && squareChecking.Piece.Color == color && squareChecking.Piece.Type == PieceTypes.King)
                    {
                        return squareChecking;
                    }
                }
            }
            throw new Exception("Cannot find king?");
        }
    }
}
