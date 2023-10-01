

//rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR

namespace ChessEngine
{
    public class Board
    {
        public Dictionary<string, Piece> boardMap = new Dictionary<string, Piece>();
        public string[] squares = new string[]
        {
            "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
            "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
            "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
            "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
            "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
            "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
            "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
            "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
        };

        public void addFenToBoard(string fen)
        {
            Dictionary<char, PieceType> piecesDict = new Dictionary<char, PieceType>
            {
                {'p', PieceType.pawn },
                {'n', PieceType.knight },
                {'b', PieceType.bishop },
                {'r', PieceType.rook },
                {'q', PieceType.queen },
                {'k', PieceType.king }
            };

            char[] fenChars = fen.ToCharArray();
            int squareIndex = 0;

            for (int i = 0; i < fenChars.Length; i++)
            {
                char c = fenChars[i];

                if (Char.IsDigit(c))
                {
                    for(int j = 0; j < Char.GetNumericValue(c); j++)
                    {
                        boardMap.Add(squares[squareIndex + j], new Piece(PieceType.blank, PieceColour.blank));
                    }
                    squareIndex += (int)Char.GetNumericValue(c);
                }
                else if (Char.IsUpper(c))
                {
                    boardMap.Add(squares[squareIndex], new Piece(piecesDict[Char.ToLower(c)], PieceColour.white));
                    squareIndex++;
                }
                else if (Char.IsLower(c))
                {
                    boardMap.Add(squares[squareIndex], new Piece(piecesDict[c], PieceColour.black));
                    squareIndex++;
                }
            }
        }
        public void setStartingPosition()
        {
            this.addFenToBoard("rnbqkbnr/pppppppp/8/8/7/8/PPPPPPPP/RNBQKBNR");
        }
    }
}