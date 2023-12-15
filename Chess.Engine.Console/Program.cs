using ChessLibrary;
using ChessLibrary.Evaluation;
using System.Security.Cryptography.X509Certificates;

public class Program
{

    private static Game _game;
    private static Engine? _whiteEngine;
    private static Engine? _blackEngine;


    public static void Main(string[] args)
    {
        if (!Directory.Exists("logs"))
        {
            Directory.CreateDirectory("logs");
        }

        _game = new Game(ChessLibrary.Enums.BoardType.BitBoard);

        var whiteConfig = new ScorerConfiguration()
        {
            SelfInCheckScore = -100,
            BishopValue = 320,
            KnightValue = 325,
            OpponentInCheckScore = 40,
            CenterSquareValue = 60,
            CenterBorderValue = 30,
            PawnValue = 120,
            KingValue = 99999,
            MaxTimeMilliseconds = 100_000, //300_000,// Int32.MaxValue, //10000,
            QueenValue = 900,
            RookValue = 600,
            StartingDepth = 4,
            MaxDepth = null
        };

        var blackConfig = new ScorerConfiguration()
        {
            SelfInCheckScore = -100,
            BishopValue = 320,
            KnightValue = 325,
            MaxDepth = null,
            OpponentInCheckScore = 40,
            CenterSquareValue = 60,
            CenterBorderValue = 30,
            PawnValue = 120,
            KingValue = 99999,
            MaxTimeMilliseconds = 100_000, //Int32.MaxValue, //10000,
            QueenValue = 900,
            RookValue = 600,
            StartingDepth = 4,
            //MaxDepth == null
        };

        _whiteEngine = new Engine(whiteConfig);
        _blackEngine = new Engine(blackConfig);

        while (true)
        {
            var command = Console.ReadLine();
            File.AppendAllLines("logs\\args.txt", new[] { command });
            if(command == "uci")
            {
                Respond("id name DanerdBot");
                Respond("id author Dan");
                Respond("option name Move Overhead type spin default 10 min 0 max 5000");
                Respond("option name Threads type spin default 1 min 1 max 512");
                Respond("option name Hash type spin default 16 min 1 max 33554432");
                Respond("uciok");
                Respond("readyok");
            }
            var commandParts = command!.Split(' ');
            if (commandParts[0] == "position")
            {
                if (commandParts[1] == "startpos")
                {
                    _game.ResetGame();
                    for(int i = 3; i < commandParts.Length; i++)
                    {
                        var move = commandParts[i];
                        var moves = _game.GetAllLegalMoves();
                        var startingFile = Enum.Parse<Files>(move[0].ToString(), true);
                        var startingRank = int.Parse(move[1].ToString());
                        var endingFile = Enum.Parse<Files>(move[2].ToString(), true);
                        var endingRank = int.Parse(move[3].ToString());

                        var startingSquare = new Square { File = startingFile, Rank = startingRank };
                        var destinationSquare = new Square { File = endingFile, Rank = endingRank };

                        var moveToMake = moves.First(x=> x.StartingSquare == startingSquare.SquareNumber && x.TargetSquare == destinationSquare.SquareNumber);
                        _game.AddMove(moveToMake, false);
                    }
                }
            }

            if (command!.StartsWith("go"))
            {
                Engine e;
                if(_game.PlayerToMove == Colors.White)
                {
                    e = _whiteEngine;
                }
                else
                {
                    e = _blackEngine;
                }
                var result = e.GetBestMove(_game, _game.PlayerToMove);
                var startingSquare = new Square(result.node.Move.Value.StartingSquare);
                var endingSquare = new Square(result.node.Move.Value.TargetSquare);
                Respond($"bestmove {startingSquare.File.ToString().ToLower()}{startingSquare.Rank}{endingSquare.File.ToString().ToLower()}{endingSquare.Rank}");
            }

        }

    }

    private static void Respond(string response)
    {
        File.AppendAllLines("logs\\args.txt", new[] { $"     {response}" });
        Console.WriteLine(response);
    }

}