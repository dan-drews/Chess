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
                return IsKingInCheck(PlayerToMove) && !MoveLegalityEvaluator.GetAllLegalMoves(Board, PlayerToMove, Moves).Any();
            }
        }
        public int GetScore(Colors color)
        {
            if (color == Colors.White)
            {
                if (_whiteScore == null)
                {
                    _whiteScore = GetScoreInternal(color);
                }
                return _whiteScore.Value;
            }
            else
            {
                if (_blackScore == null)
                {
                    _blackScore = GetScoreInternal(color);
                }
                return _blackScore.Value;
            }
        }

        private int GetScoreInternal(Colors color)
        {
            int score = 0;
            int highestPiece = 0;
            Colors opposingColor = color == Colors.Black ? Colors.White : Colors.Black;
            for (Files f = Files.A; f <= Files.H; f++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var piece = Board.GetSquare(f, rank).Piece;
                    if (piece != null && piece.Color == color)
                    {
                        score += piece.Score;

                        if ((rank == 5 || rank == 4) && (f == Files.D || f == Files.E))
                        {
                            score += 2;
                        }
                        else if (((rank == 6 || rank == 3) && f >= Files.C && f <= Files.F) ||
                                 ((f == Files.C || f == Files.F) && (rank == 5 || rank == 4)))
                        {
                            score += 1;
                        }

                        if (piece.Score > highestPiece && piece.Type != PieceTypes.King)
                        {
                            highestPiece = piece.Score;
                        }
                    }
                }
            }
            if (IsKingInCheck(opposingColor))
            {
                score += highestPiece - 1;
            }
            if (IsKingInCheck(color))
            {
                score -= 8;
            }
            return score;
        }

        private bool? _isBlackKingInCheck = null;
        private bool? _isWhiteKingInCheck = null;

        private int? _blackScore = null;
        private int? _whiteScore = null;


        private bool IsKingInCheck(Colors color)
        {
            if (color == Colors.Black)
            {
                if (_isBlackKingInCheck == null)
                {
                    _isBlackKingInCheck = MoveLegalityEvaluator.IsKingInCheck(Board, color, Moves);
                }
                return _isBlackKingInCheck.Value;
            }
            else
            {
                if (_isWhiteKingInCheck == null)
                {
                    _isWhiteKingInCheck = MoveLegalityEvaluator.IsKingInCheck(Board, color, Moves);
                }
                return _isWhiteKingInCheck.Value;
            }
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

                _blackScore = null;
                _whiteScore = null;

                _isWhiteKingInCheck = null;
                _isBlackKingInCheck = null;
                return Board;
            }
            else
            {
                throw new Exception("Invalid Move");
            }
        }

        public Board UndoLastMove()
        {
            var move = Moves.Last();
            var startingSquare = Board.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank);
            if (startingSquare.Piece == null)
            {
                throw new Exception("No piece to move");
            }
            var initialPiece = move.Piece;
            startingSquare.Piece = move.CapturedPiece;
            Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = initialPiece;

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
                        rookCurrentSquare.Piece = targetRookSquare.Piece;
                        targetRookSquare.Piece = null;
                    }

                    if (move.DestinationSquare.File == Files.C)
                    {
                        var rookCurrentSquare = Board.GetSquare(Files.A, rank);
                        var targetRookSquare = Board.GetSquare(Files.D, rank);
                        rookCurrentSquare.Piece = targetRookSquare.Piece;
                        targetRookSquare.Piece = null;
                    }
                }
            }

            _blackScore = null;
            _whiteScore = null;

            _isWhiteKingInCheck = null;
            _isBlackKingInCheck = null;
            Moves.RemoveAt(Moves.Count - 1); // can't just remove "Move" because the move equality kicks in.
            return Board;

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
