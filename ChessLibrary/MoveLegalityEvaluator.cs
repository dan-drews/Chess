using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public static class MoveLegalityEvaluator
    {
        public static List<Move>? GetAllLegalMoves(Board b, SquareState squareState)
        {
            if (squareState.Piece == null)
            {
                return null;
            }

            var piece = squareState.Piece;
            var color = piece.Color;
            var square = squareState.Square;

            var result = new List<Move>();

            // TODO: All pieces are unable to make a move if it would put the king in check.


            int pawnDirection = color == Colors.White ? 1 : -1;
            if (piece.Type == PieceTypes.Pawn)
            {
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
                        result.Add(new Move(piece, color, square, squareForward.Square));

                        if ((color == Colors.White && square.Rank == 2) || (color == Colors.Black && square.Rank == 7))
                        {
                            var twoSquaresForward = b.GetSquare(squareState.Square.File, squareState.Square.Rank + (pawnDirection * 2));
                            if (twoSquaresForward.Piece == null)
                            {
                                result.Add(new Move(piece, color, square, twoSquaresForward.Square));
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
                        if(diagonalSquare.Piece != null && diagonalSquare.Piece.Color != color)
                        {
                            result.Add(new Move(piece, color, square, diagonalSquare.Square)
                            {
                                CapturedPiece = diagonalSquare.Piece
                            });
                        }
                    }
                }

                // TODO - En Passant
            }

            if(piece.Type == PieceTypes.Knight)
            {
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

                foreach(var t in targets)
                {
                    if(t.file >= Files.A && t.file <= Files.H && t.rank >= 1 && t.rank <= 8)
                    {
                        var targetSquare = b.GetSquare(t.file, t.rank);
                        if(targetSquare.Piece == null || targetSquare.Piece.Color != color)
                        {
                            result.Add(new Move(piece, color, square, targetSquare.Square)
                            {
                                CapturedPiece = targetSquare.Piece
                            });
                        }
                    }
                }

            }

            return result;
        }
    }
}
