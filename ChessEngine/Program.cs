
using ChessEngine;


Board board = new Board();
board.setStartingPosition();

var nMoves = board.generateLegalKnightMoves();
foreach (var move in nMoves)
{
    Console.WriteLine(move.startSquare + move.targetSquare);
}


//var test = board.isLegalChangeInPosition(-1, 2, "b1");
//Console.WriteLine(test);
