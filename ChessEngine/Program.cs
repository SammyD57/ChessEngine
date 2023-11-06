using ChessEngine;
using System.Drawing;

Board board = new Board();
board.SetStartingPosition();


while (true)
{
    board.PrintBoard();
    board.UpdateAttackDefendMap();
    //foreach(var kvp in board.attackDefendMap)
    //{
    //    Console.WriteLine(kvp.Key + ": " + kvp.Value);
    //}
    board.MakeMove(new Move("e2e4", board));
}


