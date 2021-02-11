using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLibrary
{
    public class Game
    {
        public List<Move> Moves { get; private set; } = new List<Move>();

        public Board Board { get; } = new Board();

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

        public Board AddMove(Move move)
        {
            var legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(Board, Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank));
            if(legalMoves == null)
            {
                throw new Exception("Invalid move");
            }
            if(legalMoves.Any(x=> x.Equals(move)))
            {
                Moves.Add(legalMoves[legalMoves.IndexOf(move)]);
                var initialPiece = Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece;
                Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = null;
                Board.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = initialPiece;
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
