using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Board : ICloneable
    {
        private SquareState[][] _squares;

        public Board(bool setup = true)
        {
            _squares = new SquareState[8][];

            if (setup)
            {
                for (Files i = Files.A; i <= Files.H; i++)
                {
                    _squares[(int)i - 1] = new SquareState[8];
                    // Counting from 1 to 8 rather than 0 to 7 here to match the board labels
                    for (int j = 1; j <= 8; j++)
                    {
                        _squares[(int)i - 1][j - 1] = new SquareState(new Square()
                        {
                            File = i,
                            Rank = j
                        });
                    }
                }
            }
        }

        public SquareState GetSquare(Files file, int rank)
        {
            return _squares[(int)file - 1][ rank - 1];
        }

        public object Clone()
        {
            var newBoard = new Board(true);
            for (Files file = Files.A; file <= Files.H; file++)
            {
                // Counting from 1 to 8 rather than 0 to 7 here to match the board labels
                for (int rank = 1; rank <= 8; rank++)
                {
                    newBoard._squares[(int)file - 1][rank - 1].Piece = GetSquare(file, rank).Piece;
                }
            }
            return newBoard;
        }

        public void MovePiece(Move move)
        {
            var initialPiece = GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece;
            GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = null;

            if (initialPiece != null && initialPiece.Type == PieceTypes.Pawn)
            {
                var startingRank = initialPiece.Color == Colors.Black ? 4 : 5;
                if (move.StartingSquare.Rank == startingRank && move.DestinationSquare.File != move.StartingSquare.File)
                {
                    // Pawn capture... but is it en passant?
                    var destination = GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank);
                    if (destination.Piece == null)
                    {
                        // Yup. It's an en passant.
                        destination.Piece = initialPiece;
                        var capturedPawnSquare = GetSquare(move.DestinationSquare.File, move.StartingSquare.Rank);
                        capturedPawnSquare.Piece = null;
                        return;
                    }
                }
            }

            if (move.PromotedPiece == null)
            {
                GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = initialPiece;
            }
            else
            {
                GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = move.PromotedPiece;
            }

            if (initialPiece != null && initialPiece.Type == PieceTypes.King)
            {
                var rank = initialPiece.Color == Colors.Black ? 8 : 1;
                if (move.StartingSquare.Rank == rank && move.StartingSquare.File == Files.E && (move.DestinationSquare.File == Files.G || move.DestinationSquare.File == Files.C))
                {
                    // Castling.
                    if (move.DestinationSquare.File == Files.G)
                    {
                        var rookCurrentSquare = GetSquare(Files.H, rank);
                        var targetRookSquare = GetSquare(Files.F, rank);
                        targetRookSquare.Piece = rookCurrentSquare.Piece;
                        rookCurrentSquare.Piece = null;
                    }

                    if (move.DestinationSquare.File == Files.C)
                    {
                        var rookCurrentSquare = GetSquare(Files.A, rank);
                        var targetRookSquare = GetSquare(Files.D, rank);
                        targetRookSquare.Piece = rookCurrentSquare.Piece;
                        rookCurrentSquare.Piece = null;
                    }
                }
            }
        }

    }
}
