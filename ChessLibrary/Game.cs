using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLibrary
{
    public class Game : ICloneable
    {
        public object Clone()
        {
            return new Game()
            {
                Board = (Board)Board.Clone(),
                Moves = Moves.Select(x => (Move)x.Clone()).ToList(),
            };
        }

        public Colors PlayerToMove
        {
            get
            {
                return Moves.Count % 2 == 0 ? Colors.White : Colors.Black;
            }
        }

        public List<Move> Moves { get; private set; } = new List<Move>();

        public Board Board { get; private set; } = new Board();

        public bool IsGameOver
        {
            get
            {
                return IsCheckmate;
            }
        }

        public bool IsCheckmate
        {
            get
            {
                return MoveLegalityEvaluator.IsKingInCheck(Board, PlayerToMove, Moves) && !MoveLegalityEvaluator.GetAllLegalMoves(Board, PlayerToMove, Moves).Any();
            }
        }

        public int GetScore(Colors color)
        {
            int score = 0;
            int highestPiece = 0;
            Colors opposingColor = color == Colors.Black ? Colors.White : Colors.Black;
            for (Files f = Files.A; f <= Files.H; f++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var piece = Board.GetSquare(f, rank).Piece;
                    if(piece != null && piece.Color == color)
                    {
                        score += piece.Score;

                        if((rank == 5 && f == Files.D) || 
                           (rank == 4 && f == Files.D) ||
                           (rank == 5 && f == Files.E) ||
                           (rank == 4 && f == Files.E))
                        {
                            score += 2;
                        }

                        if(piece.Score > highestPiece && piece.Type != PieceTypes.King)
                        {
                            highestPiece = piece.Score;
                        }
                    }
                }
            }
            if(MoveLegalityEvaluator.IsKingInCheck(Board, opposingColor, Moves))
            {
                score += highestPiece - 1;
            }
            if (MoveLegalityEvaluator.IsKingInCheck(Board, color, Moves))
            {
                score -= 8;
            }
            return score;
        }

        public void ResetGame()
        {
            Moves = new List<Move>();

            // Clear Board
            for (Files f = Files.A; f <= Files.H; f++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    Board.GetSquare(f, rank).Piece = null;
                }
            }
            SetupBoard();
        }

        public Board AddMove(Move move, bool validate = true)
        {
            var startingSquare = Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank);
            if (startingSquare.Piece == null)
            {
                throw new Exception("No piece to move");
            }
            if (startingSquare.Piece.Color != PlayerToMove || startingSquare.Piece.Color != move.Player)
            {
                throw new Exception("Wrong Color Moving");
            }
            List<Move>? legalMoves = null;
            if (validate)
            {
                legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(Board, startingSquare, Moves);
                if (legalMoves == null)
                {
                    throw new Exception("Invalid move");
                }
            }
            if (!validate || legalMoves!.Any(x => x.Equals(move)))
            {
                if (validate)
                {
                    Moves.Add(legalMoves![legalMoves.IndexOf(move)]);
                }
                else
                {
                    Moves.Add(move);
                }
                var initialPiece = Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece;
                Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = null;
                if (move.PromotedPiece == null)
                {
                    Board.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = initialPiece;
                }
                else
                {
                    Board.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = move.PromotedPiece;
                }

                if (initialPiece != null && initialPiece.Type == PieceTypes.King)
                {
                    var rank = initialPiece.Color == Colors.Black ? 8 : 1;
                    if (move.StartingSquare.Rank == rank && move.StartingSquare.File == Files.E && (move.DestinationSquare.File == Files.G || move.DestinationSquare.File == Files.C))
                    {
                        // Castling.
                        if (move.DestinationSquare.File == Files.G)
                        {
                            var rookCurrentSquare = Board.GetSquare(Files.H, rank);
                            var targetRookSquare = Board.GetSquare(Files.F, rank);
                            targetRookSquare.Piece = rookCurrentSquare.Piece;
                            rookCurrentSquare.Piece = null;
                        }

                        if (move.DestinationSquare.File == Files.C)
                        {
                            var rookCurrentSquare = Board.GetSquare(Files.A, rank);
                            var targetRookSquare = Board.GetSquare(Files.D, rank);
                            targetRookSquare.Piece = rookCurrentSquare.Piece;
                            rookCurrentSquare.Piece = null;
                        }
                    }
                }

                return Board;
            }
            else
            {
                throw new Exception("Invalid Move");
            }
        }

        private void SetupBoard()
        {
            // White Pawns
            for (Files f = Files.A; f <= Files.H; f++)
            {
                Board.GetSquare(f, 2).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Pawn };
            }

            // Black Pawns
            for (Files f = Files.A; f <= Files.H; f++)
            {
                Board.GetSquare(f, 7).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Pawn };
            }

            Board.GetSquare(Files.A, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };
            Board.GetSquare(Files.H, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };

            Board.GetSquare(Files.B, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Knight };
            Board.GetSquare(Files.G, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Knight };

            Board.GetSquare(Files.C, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Bishop };
            Board.GetSquare(Files.F, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Bishop };

            Board.GetSquare(Files.E, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.King };
            Board.GetSquare(Files.D, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Queen };

            Board.GetSquare(Files.A, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Rook };
            Board.GetSquare(Files.H, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Rook };

            Board.GetSquare(Files.B, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Knight };
            Board.GetSquare(Files.G, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Knight };

            Board.GetSquare(Files.C, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Bishop };
            Board.GetSquare(Files.F, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Bishop };

            Board.GetSquare(Files.E, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.King };
            Board.GetSquare(Files.D, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Queen };
        }
    }
}
