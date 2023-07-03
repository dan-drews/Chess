using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class NaiveMoveLegality //: IMoveLegality
    {

        public List<Move> GetAllLegalMoves(IBoard b, Colors color, List<Move> pastMoves)
        {
            var result = new List<Move>();
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var squareChecking = b.GetSquare(file, rank);
                    if (squareChecking.Piece != null && squareChecking.Piece.Color == color)
                    {
                        result.AddRange(GetAllLegalMoves(b, squareChecking, pastMoves, false)!);
                    }
                }
            }
            return result;
        }

        public List<Move>? GetAllLegalMoves(IBoard b, SquareState squareState, List<Move> pastMoves, bool ignoreCheck = false)
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
                    potentialMoves = GetValidPawnMoves(b, squareState, piece, color, square, pawnDirection, pastMoves.Any() ? pastMoves.Last() : null);
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
                    potentialMoves = GetValidKingMoves(b, piece, color, square, pastMoves);
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

                var tempBoard = (IBoard)b.Clone();
                tempBoard.MovePiece(move);

                // find the king after the move.
                SquareState kingSquareState = GetKingSquare(tempBoard, color);

                // Loop over all squares. If oponent is in square, see if opponent can legally attack king.
                bool isInCheck = KingIsInCheck(color, tempBoard, kingSquareState, pastMoves);
                if (!isInCheck)
                {
                    result.Add(move);
                }
            }

            return result;
        }

        public bool IsKingInCheck(IBoard b, Colors color, List<Move> pastMoves)
        {
            SquareState kingSquareState = GetKingSquare(b, color);
            return KingIsInCheck(color, b, kingSquareState, pastMoves);
        }

        private List<Move> GetValidStraightLineMovves(IBoard b, Piece piece, Colors color, Square square)
        {
            var result = new List<Move>();

            // Check all spaces to the right.
            if (square.File < Files.H)
            {
                for (Files file = square.File + 1; file <= Files.H; file++)
                {
                    var targetSquare = b.GetSquare(file, square.Rank);
                    if (targetSquare.Piece != null)
                    {
                        if (targetSquare.Piece.Color != color)
                        {
                            result.Add(new Move(piece, color, square, targetSquare.Square)
                            {
                                CapturedPiece = targetSquare.Piece
                            });
                        }
                        break;
                    }

                    result.Add(new Move(piece, color, square, targetSquare.Square));
                }
            }

            // Check all spaces to the left.
            if (square.File > Files.A)
            {
                for (Files file = square.File - 1; file >= Files.A; file--)
                {
                    var targetSquare = b.GetSquare(file, square.Rank);
                    if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                    {
                        break;
                    }
                }
            }

            // Check all spaces to the North.
            if (square.Rank < 8)
            {
                for (int rank = square.Rank + 1; rank <= 8; rank++)
                {
                    var targetSquare = b.GetSquare(square.File, rank);
                    if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                    {
                        break;
                    }
                }
            }

            // Check all spaces to the South.
            if (square.Rank > 1)
            {
                for (int rank = square.Rank - 1; rank >= 1; rank--)
                {
                    var targetSquare = b.GetSquare(square.File, rank);
                    if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                    {
                        break;
                    }
                }
            }
            return result;
        }

        private List<Move> GetValidDiagonalMoves(IBoard b, Piece piece, Colors color, Square square)
        {
            var result = new List<Move>();
            int rank = square.Rank;
            Files file = square.File;

            while (rank + 1 <= 8 && file + 1 <= Files.H)
            {
                rank = rank + 1;
                file = file + 1;
                var targetSquare = b.GetSquare(file, rank);
                if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                {
                    break;
                }
            }

            rank = square.Rank;
            file = square.File;
            while (rank - 1 >= 1 && file + 1 <= Files.H)
            {
                rank = rank - 1;
                file = file + 1;
                var targetSquare = b.GetSquare(file, rank);
                if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                {
                    break;
                }
            }

            rank = square.Rank;
            file = square.File;
            while (rank + 1 <= 8 && file - 1 >= Files.A)
            {
                rank = rank + 1;
                file = file - 1;
                var targetSquare = b.GetSquare(file, rank);
                if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                {
                    break;
                }
            }

            rank = square.Rank;
            file = square.File;
            while (rank - 1 >= 1 && file - 1 >= Files.A)
            {
                rank = rank - 1;
                file = file - 1;
                var targetSquare = b.GetSquare(file, rank);
                if (AddMoveOrBreak(piece, color, square, result, targetSquare))
                {
                    break;
                }
            }

            return result;
        }

        private bool AddMoveOrBreak(Piece piece, Colors color, Square square, List<Move> result, SquareState targetSquare)
        {
            if (targetSquare.Piece != null)
            {
                if (targetSquare.Piece.Color != color)
                {
                    result.Add(new Move(piece, color, square, targetSquare.Square)
                    {
                        CapturedPiece = targetSquare.Piece
                    });
                }
                return true;
            }
            result.Add(new Move(piece, color, square, targetSquare.Square));
            return false;
        }

        private List<Move> GetValidRookMoves(IBoard b, Piece piece, Colors color, Square square)
        {
            return GetValidStraightLineMovves(b, piece, color, square);
        }


        private List<Move> GetValidBishopMoves(IBoard b, Piece piece, Colors color, Square square)
        {
            return GetValidDiagonalMoves(b, piece, color, square);
        }

        private List<Move> GetValidKingMoves(IBoard b, Piece piece, Colors color, Square square, List<Move> pastMoves)
        {
            List<Move> potentialMoves = new List<Move>();
            var targets = new (Files file, int rank)[]
            {
                (square.File - 1, square.Rank),
                (square.File - 1, square.Rank + 1),
                (square.File - 1, square.Rank - 1),

                (square.File + 1, square.Rank),
                (square.File + 1, square.Rank + 1),
                (square.File + 1, square.Rank - 1),

                (square.File, square.Rank - 1),
                (square.File, square.Rank + 1),
            };

            if (square.File == Files.E)
            {
                int rank = color == Colors.White ? 1 : 8;
                if (square.Rank == rank)
                {
                    // It's the proper tile to castle

                    // Check and see if left rook is in place
                    var leftRookSquare = b.GetSquare(Files.A, rank);
                    if (leftRookSquare.Piece != null && leftRookSquare.Piece.Type == PieceTypes.Rook)
                    {
                        var canCastle = true;
                        // See if king or that rook have moved.
                        foreach (var m in pastMoves)
                        {
                            if ((m.Piece.Color == piece.Color && m.Piece.Type == PieceTypes.King) || m.Piece == leftRookSquare.Piece)
                            {
                                canCastle = false;
                                break;
                            }
                        }

                        if (canCastle)
                        {
                            for (Files f = square.File - 1; f > Files.A; f--)
                            {
                                if (b.GetSquare(f, rank).Piece != null)
                                {
                                    canCastle = false;
                                    break;
                                }
                            }
                        }

                        if (canCastle)
                        {
                            Colors attacker = color == Colors.White ? Colors.Black : Colors.White;
                            if (IsSquareUnderAttack(attacker, b, b.GetSquare(Files.E, rank), pastMoves) ||
                                IsSquareUnderAttack(attacker, b, b.GetSquare(Files.D, rank), pastMoves) ||
                                IsSquareUnderAttack(attacker, b, b.GetSquare(Files.C, rank), pastMoves))
                            {
                                canCastle = false;
                            }
                        }

                        if (canCastle)
                        {
                            potentialMoves.Add(new Move(piece, color, square, new Square() { File = Files.C, Rank = rank }));
                        }
                    }

                    var rightRookSquare = b.GetSquare(Files.H, rank);
                    if (rightRookSquare.Piece != null && rightRookSquare.Piece.Type == PieceTypes.Rook)
                    {
                        var canCastle = true;
                        // See if king or that rook have moved.
                        foreach (var m in pastMoves)
                        {
                            if ((m.Piece.Color == piece.Color && m.Piece.Type == PieceTypes.King) || m.Piece == rightRookSquare.Piece)
                            {
                                canCastle = false;
                                break;
                            }
                        }

                        if (canCastle)
                        {
                            for (Files f = square.File + 1; f < Files.H; f++)
                            {
                                if (b.GetSquare(f, rank).Piece != null)
                                {
                                    canCastle = false;
                                    break;
                                }
                            }
                        }

                        if (canCastle)
                        {
                            Colors attacker = color == Colors.White ? Colors.Black : Colors.White;
                            if (IsSquareUnderAttack(attacker, b, b.GetSquare(Files.E, rank), pastMoves) ||
                                IsSquareUnderAttack(attacker, b, b.GetSquare(Files.F, rank), pastMoves) ||
                                IsSquareUnderAttack(attacker, b, b.GetSquare(Files.G, rank), pastMoves))
                            {
                                canCastle = false;
                            }
                        }

                        if (canCastle)
                        {
                            potentialMoves.Add(new Move(piece, color, square, new Square() { File = Files.G, Rank = rank }));
                        }
                    }

                }
            }

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

        private List<Move> GetValidQueenMoves(IBoard b, Piece piece, Colors color, Square square)
        {
            return GetValidStraightLineMovves(b, piece, color, square).Concat(GetValidDiagonalMoves(b, piece, color, square)).ToList();
        }

        private List<Move> GetValidKnightMoves(IBoard b, Piece piece, Colors color, Square square)
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

        private List<Move> GetValidPawnMoves(IBoard b, SquareState squareState, Piece piece, Colors color, Square square, int pawnDirection, Move? mostRecentMove)
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

                // En Passant
                var startingRank = color == Colors.Black ? 4 : 5;
                var moveDirection = color == Colors.Black ? -1 : 1;
                if (mostRecentMove != null
                    && square.Rank == startingRank
                    && mostRecentMove.Piece.Type == PieceTypes.Pawn
                    && mostRecentMove.DestinationSquare.Rank == startingRank
                    && mostRecentMove.StartingSquare.Rank == startingRank + (moveDirection * 2) //Started in the appropriate starting rank.
                    && Math.Abs(mostRecentMove.DestinationSquare.File - square.File) == 1)
                {
                    potentialMoves.Add(new Move(piece, color, square, b.GetSquare(mostRecentMove.DestinationSquare.File, startingRank + moveDirection).Square)
                    {
                        CapturedPiece = mostRecentMove.Piece
                    });
                }

                var result = new List<Move>();
                foreach (var move in potentialMoves)
                {
                    int endRank = color == Colors.White ? 8 : 1;
                    if (move.DestinationSquare.Rank == endRank)
                    {
                        // Promotion. Don'd add this move, but add 1 for each promoted piece type
                        result.Add(new Move(piece, color, move.StartingSquare, move.DestinationSquare)
                        {
                            CapturedPiece = move.CapturedPiece,
                            PromotedPiece = new Piece() { Color = color, Type = PieceTypes.Queen }
                        });
                        result.Add(new Move(piece, color, move.StartingSquare, move.DestinationSquare)
                        {
                            CapturedPiece = move.CapturedPiece,
                            PromotedPiece = new Piece() { Color = color, Type = PieceTypes.Knight }
                        });
                        result.Add(new Move(piece, color, move.StartingSquare, move.DestinationSquare)
                        {
                            CapturedPiece = move.CapturedPiece,
                            PromotedPiece = new Piece() { Color = color, Type = PieceTypes.Bishop }
                        });
                        result.Add(new Move(piece, color, move.StartingSquare, move.DestinationSquare)
                        {
                            CapturedPiece = move.CapturedPiece,
                            PromotedPiece = new Piece() { Color = color, Type = PieceTypes.Rook }
                        });
                    }
                    else
                    {
                        result.Add(move);
                    }
                }
                return result;
            }


            return potentialMoves;
        }

        private bool IsSquareUnderAttack(Colors attackingColor, IBoard board, SquareState square, List<Move> pastMoves)
        {
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var squareChecking = board.GetSquare(file, rank);
                    if (squareChecking.Piece != null && squareChecking.Piece.Color == attackingColor)
                    {
                        if (IsPieceAttackingSquare(board, squareChecking, square, pastMoves))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool KingIsInCheck(Colors color, IBoard tempBoard, SquareState kingSquareState, List<Move> pastMoves)
        {
            var attackingColor = color == Colors.White ? Colors.Black : Colors.White;
            return IsSquareUnderAttack(attackingColor, tempBoard, kingSquareState, pastMoves);
        }

        private bool IsPieceAttackingSquare(IBoard board, SquareState attacker, SquareState target, List<Move> pastMoves)
        {
            var piece = attacker.Piece;
            List<Move>? moves;
            switch (piece!.Type)
            {
                case PieceTypes.Rook:
                    if (attacker.Square.Rank == target.Square.Rank || attacker.Square.File == target.Square.File)
                    {
                        moves = GetAllLegalMoves(board, attacker, pastMoves, true);
                        return moves != null && moves.Any(move => move.DestinationSquare.Rank == target.Square.Rank && move.DestinationSquare.File == target.Square.File);
                    }
                    else
                    {
                        return false;
                    }
                case PieceTypes.Bishop:
                    // They are diagonal in this case
                    if (Math.Abs(attacker.Square.Rank - target.Square.Rank) == Math.Abs(attacker.Square.File - target.Square.File))
                    {
                        moves = GetAllLegalMoves(board, attacker, pastMoves, true);
                        return moves != null && moves.Any(move => move.DestinationSquare.Rank == target.Square.Rank && move.DestinationSquare.File == target.Square.File);
                    }
                    else
                    {
                        return false;
                    }
                case PieceTypes.Queen:
                    if (Math.Abs(attacker.Square.Rank - target.Square.Rank) == Math.Abs(attacker.Square.File - target.Square.File) || attacker.Square.Rank == target.Square.Rank || attacker.Square.File == target.Square.File)
                    {
                        moves = GetAllLegalMoves(board, attacker, pastMoves, true);
                        return moves != null && moves.Any(move => move.DestinationSquare.Rank == target.Square.Rank && move.DestinationSquare.File == target.Square.File);
                    }
                    else
                    {
                        return false;
                    }
                case PieceTypes.Pawn:
                    int direction = piece.Color == Colors.Black ? -1 : 1;
                    return Math.Abs(attacker.Square.File - target.Square.File) == 1 && attacker.Square.Rank + direction == target.Square.Rank;
                case PieceTypes.King:
                    return Math.Abs(attacker.Square.File - target.Square.File) <= 1 && Math.Abs(attacker.Square.Rank - target.Square.Rank) <= 1;
                case PieceTypes.Knight:
                    return (Math.Abs(attacker.Square.File - target.Square.File) == 1 && Math.Abs(attacker.Square.Rank - target.Square.Rank) == 2) ||
                           (Math.Abs(attacker.Square.File - target.Square.File) == 2 && Math.Abs(attacker.Square.Rank - target.Square.Rank) == 1);

            }

            moves = GetAllLegalMoves(board, attacker, pastMoves, true);
            return moves != null && moves.Any(move => move.DestinationSquare.Rank == target.Square.Rank && move.DestinationSquare.File == target.Square.File);
        }

        private SquareState GetKingSquare(IBoard b, Colors color)
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
