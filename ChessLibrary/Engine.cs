using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class Engine
    {
        const decimal MAX_DEPTH = 2M;
        public static Move? GetBestMove(Game game, Colors playerColor, int currentDepth = 1)
        {
            var opponentColor = playerColor == Colors.White ? Colors.Black : Colors.White;
            var isCheckmate = game.IsCheckmate;
            if (!isCheckmate)
            {
                return GetMoveScores(game, playerColor, opponentColor, currentDepth, null).m;
            }


            return null;
        }

        private static (Move m, int score, int totalMoves) GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, Move? move)
        {

            var isCheckmate = game.IsCheckmate;
            var isStalemate = game.IsStalemate;
            if (currentDepth > MAX_DEPTH * 2 || isCheckmate ||isStalemate)
            {
                if(move == null)
                {
                    throw new Exception("Error! Move is null");
                }
                if (game.IsStalemate)
                {
                    return (move, 0, 1);
                }
                if (isCheckmate)
                {
                    return move.Piece.Color == playerColor ? (move, 1000000000, 1) : (move, -1000000000, 1);
                }

                return (move, game.GetScore(playerColor) - game.GetScore(opponentColor), 1);
            }

            var legalMoves = game.GetAllLegalMoves();
            var scores = new List<(Move m, int score, int totalMoves)>();
            foreach(var m in legalMoves)
            {
                game.AddMove(m, false);
                scores.Add(GetMoveScores(game, playerColor, opponentColor, currentDepth + 1, m));
                game.UndoLastMove();
            }

            if (game.PlayerToMove == playerColor)
            {
                var bestMove = scores.MaxBy(x => x.score).MinBy(x=> x.totalMoves).OrderBy(x=> new Guid()).First();
                if (move == null)
                {
                    return bestMove;
                }
                return (move, bestMove.score, bestMove.totalMoves + 1);
            }
            else
            {
                var bestMove = scores.MinBy(x => x.score).OrderBy(x => new Guid()).First();
                if (move == null)
                {
                    return bestMove;
                }
                return (move, bestMove.score, bestMove.totalMoves + 1);
            }
        }

        private class GameScore
        {
            public int MyScore { get; set; }
            public int OpponentScore { get; set; }
            public bool OpponentWinsWithCheckmate { get; set; }
            public bool IWinWithCheckmate { get; set; }
            public List<GameScore>? ChildrenGames { get; set; }
            public int TotalChildren { get; set; } = 1;

            public int ChildrenCountWithHigherScore { get; set; } = 0;
            public int ChildrenCountWithLowerScore { get; set; } = 0;
            public int ChildrenCountWithNeutralScore { get; set; } = 0;

            public int ChildrenMaxScoreDiff { get; set; }
            public int ChildrenMinScoreDiff { get; set; }
            public double ChildrenAvgScoreDiff { get; set; }
            public Move? Move { get; set; }

            public int ScoreDiff
            {
                get
                {
                    return MyScore - OpponentScore;
                }
            }

        }

    }
}
