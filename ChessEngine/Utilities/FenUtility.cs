
namespace ChessEngine.Utilities
{
    public static class FenUtility
    {
        public static void AddFen(Board board, string fen)
        {
            string[] fenSections = fen.Split(" ");
            char[] fenPieces = fenSections[0].ToCharArray();
            int squareIndex = 0;

            for (int i = 0; i < fenPieces.Length; i++)
            {
                char c = fenPieces[i];

                if (Char.IsDigit(c))
                {
                    for (int j = 0; j < Char.GetNumericValue(c); j++)
                    {
                        board.boardMap.Add(board.allSquares[squareIndex + j], new Piece(PieceType.blank, PieceColour.blank));
                    }
                    squareIndex += (int)Char.GetNumericValue(c);
                }
                else if (Char.IsUpper(c))
                {
                    board.boardMap.Add(board.allSquares[squareIndex], new Piece(board.piecesDict[Char.ToLower(c)], PieceColour.white));
                    squareIndex++;
                }
                else if (Char.IsLower(c))
                {
                    board.boardMap.Add(board.allSquares[squareIndex], new Piece(board.piecesDict[c], PieceColour.black));
                    squareIndex++;
                }
            }

            string activeColour = fenSections[1];
            if (activeColour == "w")
            {
                board.isWhiteToMove = true;
            }
            else
            {
                board.isWhiteToMove = false;
            }

            char[] castlingRights = fenSections[2].ToCharArray();
            foreach (char c in castlingRights)
            {
                switch (c)
                {
                    case 'K':
                        board.whiteHasKingsideCastleRight = true;
                        break;
                    case 'Q':
                        board.whiteHasQueensideCastleRight = true;
                        break;
                    case 'k':
                        board.blackHasKingsideCastleRight = true;
                        break;
                    case 'q':
                        board.blackHasQueensideCastleRight = true;
                        break;
                }
            }

            if (fenSections[3] != "-")
            {
                board.enPassantSquare = fenSections[3];
            }
            board.fiftyMoveRuleCount = int.Parse(fenSections[4]);
            board.fullMovesCount = int.Parse(fenSections[5]);
        }
    }
}
