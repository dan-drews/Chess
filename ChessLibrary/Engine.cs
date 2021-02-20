using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class Engine
    {
        const decimal MAX_DEPTH = 3M;
        public static (NodeInfo? node, int depth) GetBestMove(Game game, Colors playerColor)
        {
            var opponentColor = playerColor == Colors.White ? Colors.Black : Colors.White;
            var isCheckmate = game.IsCheckmate;
            if (!isCheckmate)
            {
                NodeInfo? result = null;
                var sw = new Stopwatch();
                int depthToSearch = 3;
                bool checkmate = false;
                while (sw.ElapsedMilliseconds < 1000 && !checkmate)
                {
                    depthToSearch++;
                    sw.Reset();
                    sw.Start();
                    result = GetMoveScores(game, playerColor, opponentColor, depthToSearch, null, int.MinValue, int.MaxValue);
                    checkmate = result.Score > 10000000;
                    sw.Stop();
                }
                return (result, depthToSearch);
            }


            return (null, 3);
        }

        private static NodeInfo GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, Move? move, int alpha, int beta)
        {

            var isCheckmate = game.IsCheckmate;
            var isStalemate = game.IsStalemate;

            if (currentDepth == 0 || isCheckmate ||isStalemate)

            {
                if(move == null)
                {
                    throw new Exception("Error! Move is null");
                }
                if (game.IsStalemate)
                {
                    return new NodeInfo(move, 0, 0, 0);
                }
                if (isCheckmate)
                {
                    return move.Piece.Color == playerColor ? new NodeInfo(move, 1000000000 + currentDepth, 0, 0) : new NodeInfo(move, -1000000000 + currentDepth, 0, 0);
                }

                return new NodeInfo(move, game.GetScore(playerColor) - game.GetScore(opponentColor), 0, 0);
            }

            var legalMoves = game.GetAllLegalMoves().OrderBy(x=> Guid.NewGuid()).ToList();
            if (game.PlayerToMove == playerColor)
            {
                var value = new NodeInfo(move, int.MinValue, alpha, beta);
                foreach(var m in legalMoves)
                {

                    game.AddMove(m, false);
                    var node = GetMoveScores(game, playerColor, opponentColor, currentDepth - 1, m, alpha, beta);
                    game.UndoLastMove();
                    value = value.Score >= node.Score ? value : new NodeInfo(m, node.Score, node.Alpha, node.Beta);
                    alpha = Math.Max(alpha, value.Score);                    
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                return value;
            }
            else
            {
                var value = new NodeInfo(move, int.MaxValue, alpha, beta);
                foreach (var m in legalMoves)
                {
                    game.AddMove(m, false);
                    var node = GetMoveScores(game, playerColor, opponentColor, currentDepth - 1, m, alpha, beta);
                    game.UndoLastMove();
                    value = value.Score <= node.Score ? value : new NodeInfo(m, node.Score, node.Alpha, node.Beta);
                    beta = Math.Min(beta, value.Score);
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                return value;
            }
        }

        public class NodeInfo
        {
            public Move? Move { get; set; }
            public int Score { get; set; } 
            public int Alpha { get; set; }
            public int Beta { get; set; }

            public NodeInfo(Move? move, int score, int alpha, int beta)
            {
                Move = move;
                Score = score;
                Alpha = alpha;
                Beta = beta;
            }
        }

    }
}
