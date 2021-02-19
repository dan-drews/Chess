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
                return IsCheckmate || IsStalemate;
            }
        }

        private List<Move>? _legalMoves;

        public bool IsStalemate
        {
            get
            {
                if (_legalMoves == null)
                {
                    _legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(Board, PlayerToMove, Moves);
                }
                return !IsKingInCheck(PlayerToMove) && !GetAllLegalMoves().Any();
            }
        }

        public bool IsCheckmate
        {
            get
            {
                if (_legalMoves == null)
                {
                    _legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(Board, PlayerToMove, Moves);
                }
                return IsKingInCheck(PlayerToMove) && !GetAllLegalMoves().Any();
            }
        }

        public List<Move> GetAllLegalMoves()
        {
            if (_legalMoves == null)
            {
                _legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(Board, PlayerToMove, Moves);
            }
            return _legalMoves;
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
                        score += piece.Score * 5;

                        if ((rank == 5 || rank == 4) && (f == Files.D || f == Files.E))
                        {
                            score += 2;
                        }
                        else if (((rank == 6 || rank == 3) && f >= Files.C && f <= Files.F) ||
                                 ((f == Files.C || f == Files.F) && (rank == 5 || rank == 4)))
                        {
                            score += 1;
                        }

                        if (piece.Score > highestPiece * 3 && piece.Type != PieceTypes.King)
                        {
                            highestPiece = piece.Score * 3;
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
                score -= 15;
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
                Board.MovePiece(move);

                _blackScore = null;
                _whiteScore = null;
                _legalMoves = null;
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
            bool hasPerformedMove = false;
            if (initialPiece != null && initialPiece.Type == PieceTypes.Pawn)
            {
                var startingRank = initialPiece.Color == Colors.Black ? 4 : 5;
                var moveDirection = initialPiece.Color == Colors.Black ? -1 : 1;
                if (move.StartingSquare.Rank == startingRank && move.DestinationSquare.File != move.StartingSquare.File)
                {
                    // Pawn capture... but is it en passant?
                    if(move.DestinationSquare.Rank == startingRank + moveDirection && Math.Abs(move.DestinationSquare.File - move.StartingSquare.File) == 1)
                    {
                        // Moved diagonally, but still, was it an en passant?
                        var moveBeforeLast = Moves.ElementAt(Moves.Count - 2);
                        if (moveBeforeLast.Piece.Type == PieceTypes.Pawn 
                            && moveBeforeLast.StartingSquare.Rank == startingRank + (moveDirection * 2) 
                            && moveBeforeLast.DestinationSquare.Rank == startingRank
                            && moveBeforeLast.DestinationSquare.File == move.DestinationSquare.File)
                        {
                            // Yup, it was en passant.
                            Board.GetSquare(moveBeforeLast.DestinationSquare.File, moveBeforeLast.DestinationSquare.Rank).Piece = moveBeforeLast.Piece;
                            Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = initialPiece;
                            Board.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = null;
                            hasPerformedMove = true;
                        }
                    }
                }
            }
            if (!hasPerformedMove)
            {
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
            }

            _blackScore = null;
            _whiteScore = null;
            _legalMoves = null;
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
