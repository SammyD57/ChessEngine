﻿using ChessEngine;
using ChessEngine.Utilities;

Board board = new Board();
Run();
void Run() {
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
            Move m = new Move("e2e4", board);
            board.MakeMove(m);
            board.AddPositionToStateHistory();
            
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
