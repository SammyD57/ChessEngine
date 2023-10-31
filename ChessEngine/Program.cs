using ChessEngine;
using System.Drawing;

Board board = new Board();
board.setStartingPosition();


while (true)
{
    board.printBoard();
    board.updateAttackDefendMap();
    foreach(var kvp in board.attackDefendMap)
    {
        Console.WriteLine(kvp.Key + ": " + kvp.Value);
    }
    board.makeMove(new Move(Console.ReadLine(), board));
}


