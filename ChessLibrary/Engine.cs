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
            if (currentDepth == 1)
            {
                var myCurrentScore = game.GetScore(playerColor);
                var opponentCurrentScore = game.GetScore(opponentColor);
            }
            var isCheckmate = game.IsCheckmate;
            var gameScore = new GameScore()
            {
                MyScore = game.GetScore(playerColor),
                OpponentScore = game.GetScore(opponentColor),
                OpponentWinsWithCheckmate = isCheckmate && game.PlayerToMove == playerColor,
                IWinWithCheckmate = isCheckmate && game.PlayerToMove == opponentColor
            };
            if (!isCheckmate)
            {
                gameScore.ChildrenGames = new List<GameScore>();
                gameScore.ChildrenGames.Add(GetMoveScores(game, playerColor, opponentColor, currentDepth, null));
                var bestGameAvg = gameScore.ChildrenGames[0].ChildrenGames!.Select(x => x.ChildrenMinScoreDiff).Max();
                var bestGames = gameScore.ChildrenGames[0].ChildrenGames!.Where(x => x.ChildrenMinScoreDiff == bestGameAvg);
                var gameAvg = bestGames.Max(x => x.ChildrenAvgScoreDiff);
                var g = bestGames.First(x => x.ChildrenAvgScoreDiff == gameAvg);
                return g.Move;
            }


            return null;
        }

        private static GameScore GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, Move? move)
        {
            var isCheckmate = game.IsCheckmate;
            var gameScore = new GameScore()
            {
                MyScore = game.GetScore(playerColor),
                OpponentScore = game.GetScore(opponentColor),
                OpponentWinsWithCheckmate = isCheckmate && game.PlayerToMove == playerColor,
                IWinWithCheckmate = isCheckmate && game.PlayerToMove == opponentColor,
                Move = move
            };
            if (gameScore.OpponentWinsWithCheckmate)
            {
                gameScore.OpponentScore += 1000;
            }

            if (gameScore.IWinWithCheckmate)
            {
                gameScore.MyScore += 100;
            }

            if (currentDepth <= MAX_DEPTH * 2 && !isCheckmate)
            {
                gameScore.ChildrenGames = new List<GameScore>();
                var legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(game.Board, game.PlayerToMove);
                if (legalMoves.Any())
                {
                    var legalMoveSubset = legalMoves.ToList(); //.OrderBy(x => Guid.NewGuid()).Take((legalMoves.Count + 3) / 4).ToList();
                    foreach (var m in legalMoveSubset)
                    {
                        var clonedGame = (Game)game.Clone();
                        clonedGame.AddMove(m, false);
                        gameScore.ChildrenGames.Add(GetMoveScores(clonedGame, playerColor, opponentColor, currentDepth + 1, m));
                    }
                    gameScore.ChildrenMinScoreDiff = gameScore.ChildrenGames.Select(x => x.ChildrenMinScoreDiff).Min();
                    gameScore.ChildrenMaxScoreDiff = gameScore.ChildrenGames.Select(x => x.ChildrenMaxScoreDiff).Max();
                    gameScore.TotalChildren = gameScore.ChildrenGames.Count();
                    gameScore.ChildrenAvgScoreDiff = gameScore.ChildrenGames.Select(x => x.ChildrenAvgScoreDiff * x.TotalChildren).Average();
                }
                else
                {
                    gameScore.ChildrenAvgScoreDiff = gameScore.ScoreDiff;
                    gameScore.ChildrenMaxScoreDiff = gameScore.ScoreDiff;
                    gameScore.ChildrenMinScoreDiff = gameScore.ScoreDiff;
                }
            }
            else
            {
                gameScore.ChildrenAvgScoreDiff = gameScore.ScoreDiff;
                gameScore.ChildrenMaxScoreDiff = gameScore.ScoreDiff;
                gameScore.ChildrenMinScoreDiff = gameScore.ScoreDiff;
            }

            return gameScore;
        }

        private class GameScore
        {
            public int MyScore { get; set; }
            public int OpponentScore { get; set; }
            public bool OpponentWinsWithCheckmate { get; set; }
            public bool IWinWithCheckmate { get; set; }
            public List<GameScore>? ChildrenGames { get; set; }
            public int TotalChildren { get; set; } = 1;

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
