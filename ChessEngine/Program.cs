using ChessEngine;
using System.Drawing;

Board board = new Board();
board.setStartingPosition();


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


