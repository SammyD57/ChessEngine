using ChessEngine;
using ChessEngine.Utilities;

Board board = new Board();
Run();
void Run() 
{
    var command = Console.ReadLine().Split(" ");
    switch (command[0].ToLower())
    {
        case "startgame":
            board.SetStartingPosition();
            bool isPlaying = true;
            while (isPlaying)
            {
                DebugUtility.PrintBoard(board);

                Move playerMove = new Move(Console.ReadLine(), board);
                while (!Move.isLegal(board, playerMove))
                {
                    Console.WriteLine("Illegal Move");
                    playerMove = new Move(Console.ReadLine(), board);
                }
                board.MakeMove(playerMove);
            }
        break;

        case "test":
            board.SetStartingPosition();
            board.AddPositionToStateHistory();
            DebugUtility.PrintBoard(board);
            Move m = new Move("e2e4", board);
            board.MakeMove(m);
            DebugUtility.PrintBoard(board);
            Console.WriteLine(board.isWhiteToMove);
            Console.WriteLine(board.plyCount);
            Console.WriteLine(board.colourToMove);
            board.UndoMove(1);
            DebugUtility.PrintBoard(board);
            Console.WriteLine(board.isWhiteToMove);
            Console.WriteLine(board.plyCount);
            Console.WriteLine(board.colourToMove);
            break;

        case "quit":
            Environment.Exit(0);
        break;

        default:
            Console.WriteLine("Invalid Command");
        break;
    }
    Run();
}
