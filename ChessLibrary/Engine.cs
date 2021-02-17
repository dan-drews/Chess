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
                return GetMoveScores(game, playerColor, opponentColor, currentDepth, null, int.MinValue, int.MaxValue).Move;
            }


            return null;
        }

        private static NodeInfo GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, Move? move, int alpha, int beta)
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
                    return new NodeInfo(move, 0, 1, 0, 0);
                }
                if (isCheckmate)
                {
                    return move.Piece.Color == playerColor ? new NodeInfo(move, 1000000000, 1, 0, 0) : new NodeInfo(move, -1000000000, 1, 0, 0);
                }

                return new NodeInfo(move, game.GetScore(playerColor) - game.GetScore(opponentColor), 1, 0, 0);
            }

            var legalMoves = game.GetAllLegalMoves();
            if (game.PlayerToMove == playerColor)
            {
                var value = new NodeInfo(move, int.MinValue, 0, alpha, beta);
                foreach(var m in legalMoves)
                {

                    game.AddMove(m, false);
                    var node = GetMoveScores(game, playerColor, opponentColor, currentDepth + 1, m, alpha, beta);
                    game.UndoLastMove();
                    value = value.Score >= node.Score ? value : new NodeInfo(m, node.Score, node.TotalMoves + 1, node.Alpha, node.Beta);
                    alpha = Math.Max(alpha, value.Score);                    
                    if (alpha >= beta)
                    {
                        break;
                    }
                    if (node.Score == 1000000000)
                    {
                        break;
                    }
                }
                return value;
            }
            else
            {
                var value = new NodeInfo(move, int.MaxValue, 0, alpha, beta);
                foreach (var m in legalMoves)
                {
                    game.AddMove(m, false);
                    var node = GetMoveScores(game, playerColor, opponentColor, currentDepth + 1, m, alpha, beta);
                    game.UndoLastMove();
                    value = value.Score <= node.Score ? value : new NodeInfo(m, node.Score, node.TotalMoves + 1, node.Alpha, node.Beta);
                    beta = Math.Min(beta, value.Score);
                    if (alpha >= beta)
                    {
                        break;
                    }
                    if (node.Score == -1000000000)
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
            public int TotalMoves { get; set; }
            public int Alpha { get; set; }
            public int Beta { get; set; }

            public NodeInfo(Move? move, int score, int totalMoves, int alpha, int beta)
            {
                Move = move;
                Score = score;
                TotalMoves = totalMoves;
                Alpha = alpha;
                Beta = beta;
            }
        }

    }
}
