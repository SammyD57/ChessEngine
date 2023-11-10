using System.Runtime.InteropServices;
using ChessEngine;

Board board = new Board();

string[] command = Console.ReadLine().Split(" ");

switch (command[0].ToLower())
{
    case "startgame":
    board.SetStartingPosition();
    while (true)
    {
        board.PrintBoard();
        var legalMoves = board.GenerateAllLegalMoves();
        Move playerMove = new Move(Console.ReadLine(), board);
        while (!legalMoves.Contains(playerMove))
        {
            Console.WriteLine("Illegal Move");
            playerMove = new Move(Console.ReadLine(), board); 
        }
        board.MakeMove(playerMove);
    }
    break;
    
    default:
    Console.WriteLine("Invalid Command");
    break;
}

