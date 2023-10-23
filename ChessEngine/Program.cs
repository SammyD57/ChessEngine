using ChessEngine;
using System.Drawing;

Board board = new Board();
board.setStartingPosition();




while (true)
{
    board.printBoard();
    var qMoves = board.generateLegalQueenMoves();
    foreach (var move in qMoves)
    {
        Console.WriteLine(move.startSquare + move.targetSquare);
    }
    board.makeMove(new Move(Console.ReadLine(), board));
}


