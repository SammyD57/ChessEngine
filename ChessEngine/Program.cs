using ChessEngine;
using System.Drawing;

Board board = new Board();
board.addFenToBoard("8/3p4/8/8/4P3/8/8/8");


while (true)
{
    board.printBoard();
    var pMoves = board.generateLegalPawnMoves();
    foreach (var move in pMoves)
    {
        Console.WriteLine(move.coordinateNotation);
    }
    board.makeMove(new Move(Console.ReadLine(), board));
}


