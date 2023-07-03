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
        public static int nodesEvaluated = 0;
        public static int nonQuietDepthNodesEvaluated = 0;
        public static long miliseconds = 0;
        public static int skips = 0;

        private int _maxTime;
        private int _startingDepth;
        public Scorer Scorer { get; set; }

        private Stopwatch _stopwatch = new Stopwatch();

        public Engine(ScorerConfiguration scoreConfig)
        {
            Scorer = new Scorer(scoreConfig);
            _maxTime = scoreConfig.MaxTimeMilliseconds;
            _startingDepth = scoreConfig.StartingDepth;
        }

        public (NodeInfo? node, int depth) GetBestMove(Game game, Colors playerColor)
        {
            _stopwatch.Restart();
            nodesEvaluated = 0;
            nonQuietDepthNodesEvaluated = 0;
            var opponentColor = playerColor == Colors.White ? Colors.Black : Colors.White;
            var isCheckmate = game.IsCheckmate;
            if (!isCheckmate)
            {
                NodeInfo? result = null;
                int depthToSearch = _startingDepth;
                bool checkmate = false;
                while (_stopwatch.ElapsedMilliseconds < _maxTime && !checkmate)
                {
                    nodesEvaluated = 0;
                    nonQuietDepthNodesEvaluated = 0;
                    skips = 0;
                    depthToSearch++;
                    var previousResult = result;
                    result = GetMoveScores(game, playerColor, opponentColor, depthToSearch, null, int.MinValue, int.MaxValue);
                    checkmate = result.Score > 10000000;
                    if(previousResult != null && previousResult.Score > result.Score)
                    {
                        result = previousResult;
                    }
                }
                return (result, depthToSearch);
            }


            return (null, 3);
        }

        private int GetLoudMoveScores(Game game, Colors playerColor, Colors opponentColor, Move? move, int alpha, int beta)
        {

            if (game.PlayerToMove == playerColor)
            {
            }

            var scores = Scorer.GetScore(game.Board, game.IsKingInCheck(Colors.White), game.IsKingInCheck(Colors.Black), game.IsStalemate);
            var playerScore = playerColor == Colors.Black ? scores.blackScore : scores.whiteScore;
            var opponentScore = playerColor == Colors.Black ? scores.whiteScore : scores.blackScore;
            Engine.nodesEvaluated++;
            Engine.nonQuietDepthNodesEvaluated++;
            if (playerScore - opponentScore >= beta)
            {
                return beta;
            }

            if (playerScore - opponentScore > alpha)
            {
                alpha = playerScore - opponentScore;
            }

            var legalNonQuietMoves = game.GetAllLegalMoves(false);
            legalNonQuietMoves = legalNonQuietMoves.OrderMoves(this).ToList();
            foreach (var nqm in legalNonQuietMoves)
            {
                game.AddMove(nqm, false);
                playerScore = GetLoudMoveScores(game, playerColor, opponentColor, move, alpha, beta);
                game.UndoLastMove();
                if (playerScore >= beta)
                {
                    return beta;
                }
                if (playerScore > alpha)
                {
                    alpha = playerScore;
                }



            }
            return playerScore;
        }

        private NodeInfo GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, Move? move, int alpha, int beta)
        {

            var isCheckmate = game.IsCheckmate;
            var isStalemate = game.IsStalemate;

            if(currentDepth == 0)
            {
                var loudMoveScore = GetLoudMoveScores(game, playerColor, opponentColor, move, alpha, beta); 
                //if(loudMoveScore)
            }

            if (currentDepth == 0 || isCheckmate ||isStalemate)
            {
                if (game.IsStalemate)
                {
                    return new NodeInfo(move, 0, 0, 0);
                }
                nodesEvaluated++;
                if(move == null)
                {
                    throw new Exception("Error! Move is null");
                }
                if (isCheckmate)
                {
                    return move.Piece.Color == playerColor ? new NodeInfo(move, 1000000000 + currentDepth, 0, 0) : new NodeInfo(move, -1000000000 + currentDepth, 0, 0);
                }

                var scores = Scorer.GetScore(game.Board, game.IsKingInCheck(Colors.White), game.IsKingInCheck(Colors.Black), game.IsStalemate);
                var playerScore = playerColor == Colors.Black ? scores.blackScore : scores.whiteScore;
                var opponentScore = playerColor == Colors.Black ? scores.whiteScore : scores.blackScore;
                return new NodeInfo(move, playerScore - opponentScore, 0, 0);
            }

            var legalMoves = game.GetAllLegalMoves();
            legalMoves = legalMoves.OrderMoves(this).ToList();
            if (game.PlayerToMove == playerColor)
            {
                var value = new NodeInfo(move, int.MinValue, alpha, beta);
                foreach(var m in legalMoves)
                {
                    if(_stopwatch.ElapsedMilliseconds >= _maxTime)
                    {
                        return value;
                    }
                    game.AddMove(m, false);
                    var node = GetMoveScores(game, playerColor, opponentColor, currentDepth - 1, m, alpha, beta);
                    game.UndoLastMove();
                    value = value.Score >= node.Score ? value : new NodeInfo(m, node.Score, node.Alpha, node.Beta);
                    alpha = Math.Max(alpha, value.Score);                    
                    if (alpha >= beta)
                    {
                        skips++;
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
                    if (_stopwatch.ElapsedMilliseconds >= _maxTime)
                    {
                        return value;
                    }
                    game.AddMove(m, false);
                    var node = GetMoveScores(game, playerColor, opponentColor, currentDepth - 1, m, alpha, beta);
                    game.UndoLastMove();
                    value = value.Score <= node.Score ? value : new NodeInfo(m, node.Score, node.Alpha, node.Beta);
                    beta = Math.Min(beta, value.Score);
                    if (alpha >= beta)
                    {
                        skips++;
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
