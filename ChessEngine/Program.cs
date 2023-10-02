using ChessEngine;


Board board = new Board();
board.setStartingPosition();

/*
foreach (var kvp in board.boardMap)
{   
    Console.WriteLine("Square = {0}, Type = {1}, Colour = {2}", kvp.Key, kvp.Value.Type, kvp.Value.Colour);
}
*/

board.printBoard();