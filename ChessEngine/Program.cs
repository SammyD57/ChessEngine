using ChessEngine;


Board board = new Board();
board.addFenToBoard("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR");



while (true)
{
    board.printBoard();
    var bMoves = board.generateLegalBishopMoves();
    foreach (var move in bMoves)
    {
        Console.WriteLine(move.startSquare + move.targetSquare);
    }
    board.makeMove(new Move(Console.ReadLine(), board));
}


