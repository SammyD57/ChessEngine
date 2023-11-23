namespace ChessEngine.Utilities
{
    public static class DebugUtility
    {
        public static void PrintBoard(Board board)
        {
            string output = "";
            for (int i = 1; i <= 64; i++)
            {
                Piece currentPiece = board.boardMap[board.allSquares[i - 1]];
                if (currentPiece.Type == PieceType.blank)
                {
                 output += "| - ";
                }
                else if (currentPiece.Colour == PieceColour.white)
                {
                 output += "| " + board.piecesDict.FirstOrDefault(p => p.Value == currentPiece.Type).Key.ToString().ToUpper() + " ";
                }
                else
                {
                 output += "| " + board.piecesDict.FirstOrDefault(p => p.Value == currentPiece.Type).Key.ToString() + " ";
                }
                if (i % 8 == 0)
                {
                 output += "|\n---------------------------------\n";
                }
            }
            Console.Write(output + "\n");
        }
    }
}