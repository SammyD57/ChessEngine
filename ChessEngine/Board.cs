
using System.Drawing;

namespace ChessEngine
{
    public class Board
    {
        public int plyCount = 0;
        public Dictionary<string, Piece> boardMap = new Dictionary<string, Piece>();


        public string[] allSquares = new string[]
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
        Dictionary<char, PieceType> piecesDict = new Dictionary<char, PieceType>
        {
            {'p', PieceType.pawn },
            {'n', PieceType.knight },
            {'b', PieceType.bishop },
            {'r', PieceType.rook },
            {'q', PieceType.queen },
            {'k', PieceType.king }
        };

        private readonly int[] diagonalXDeltas = { 1, 1, -1, -1 };
        private readonly int[] diagonalYDeltas = { 1, -1, -1, 1 };

        private readonly int[] knightXDeltas = { 1, 2, 2, 1, -1, -2, -2, -1 };
        private readonly int[] knightYDeltas = { 2, 1, -1, -2, -2, -1, 1, 2 };


        public void makeMove(Move move)
        {
            boardMap[move.targetSquare] = boardMap[move.startSquare];
            boardMap[move.startSquare] = new Piece(PieceType.blank, PieceColour.blank);
            move.pieceToMove.numMovesMade++;
            plyCount++;
        }

        public void printBoard()
        {
            string board = "";
            for (int i = 1; i <= 64; i++)
            {
                Piece currentPiece = boardMap[allSquares[i - 1]];
                if (currentPiece.Type == PieceType.blank)
                {
                    board += "| - ";
                }
                else if (currentPiece.Colour == PieceColour.white)
                {
                    board += "| " + piecesDict.FirstOrDefault(p => p.Value == currentPiece.Type).Key.ToString().ToUpper() + " ";
                }
                else
                {
                    board += "| " + piecesDict.FirstOrDefault(p => p.Value == currentPiece.Type).Key.ToString() + " ";
                }
                if (i % 8 == 0)
                {
                    board += "|\n---------------------------------\n";
                }
            }
            Console.Write(board + "\n");
        }

        public void addFenToBoard(string fen)
        {

            char[] fenChars = fen.ToCharArray();
            int squareIndex = 0;

            for (int i = 0; i < fenChars.Length; i++)
            {
                char c = fenChars[i];

                if (Char.IsDigit(c))
                {
                    for (int j = 0; j < Char.GetNumericValue(c); j++)
                    {
                        boardMap.Add(allSquares[squareIndex + j], new Piece(PieceType.blank, PieceColour.blank));
                    }
                    squareIndex += (int)Char.GetNumericValue(c);
                }
                else if (Char.IsUpper(c))
                {
                    boardMap.Add(allSquares[squareIndex], new Piece(piecesDict[Char.ToLower(c)], PieceColour.white));
                    squareIndex++;
                }
                else if (Char.IsLower(c))
                {
                    boardMap.Add(allSquares[squareIndex], new Piece(piecesDict[c], PieceColour.black));
                    squareIndex++;
                }
            }
        }
        public void setStartingPosition()
        {
            addFenToBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        }

        public bool isWhiteToMove()
        {
            if (plyCount % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Piece[] getArrayOfPieces(PieceColour colour, PieceType type)
        {
            List<Piece> pieces = new List<Piece>();

            foreach (var kvp in boardMap)
            {
                if (kvp.Value.Type == type && kvp.Value.Colour == colour)
                {
                    pieces.Add(kvp.Value);
                }
            }
            return pieces.ToArray();
        }

        public string[] getEmptySquares()
        {
            List<string> squares = new List<string>();
            foreach (var kvp in boardMap)
            {
                if (kvp.Value.Type == PieceType.blank && kvp.Value.Colour == PieceColour.blank)
                {
                    squares.Add(kvp.Key);
                }
            }
            return squares.ToArray();
        }

        public string[] getSquaresContainingPiece(PieceType type, PieceColour colour)
        {
            List<string> squares = new List<string>();
            foreach (var kvp in boardMap)
            {
                if (kvp.Value.Type == type && kvp.Value.Colour == colour)
                {
                    squares.Add(kvp.Key);
                }
            }
            return squares.ToArray();
        }
        public Move[] generateLegalKnightMoves()
        {
            List<Move> moves = new List<Move>();          
            PieceColour knightColour = (isWhiteToMove()) ? PieceColour.white : PieceColour.black;           
            string[] squaresWithKnights = getSquaresContainingPiece(PieceType.knight, knightColour);    

            foreach (string square in squaresWithKnights)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (changeIsWithinBoard(knightXDeltas[i], knightYDeltas[i], square))
                    {
                        string newSquare = Square.offsetCoordinate(knightXDeltas[i], knightYDeltas[i], square);
                        if (boardMap[newSquare].Colour != knightColour)
                        {
                            moves.Add(new Move(square + newSquare, this));
                        }                   
                    }
                }          
            }
            return moves.ToArray();
        }

        public Move[] generateLegalBishopMoves()
        {
            List<Move> moves = new List<Move>();
            PieceColour bishopColour = isWhiteToMove() ? PieceColour.white : PieceColour.black;
            string[] squaresWithBishops = getSquaresContainingPiece(PieceType.bishop, bishopColour);

            foreach(string square in squaresWithBishops)
            {
                int[] maximumDiagonals = getMaximumDiagonalOffsets(square, bishopColour);
                for (int i = 0; i < 4; i++)
                {
                    for(int j = 1; j <= maximumDiagonals[i]; j++)
                    {
                        string targetSquare = Square.offsetCoordinate(diagonalXDeltas[i] * j, diagonalYDeltas[i] * j , square);
                        moves.Add(new Move(square + targetSquare, this));
                    }           
                }
            }

            return moves.ToArray();
        }
        
        public bool changeIsWithinBoard(int deltaX, int deltaY, string startingSquare)
        {
            int startingRank = int.Parse(startingSquare.Substring(1, 1));
            int startingFile = Square.getFileAsInt(Char.Parse(startingSquare.Substring(0, 1)));

            if ((startingRank + deltaY) <= 8 && (startingRank + deltaY) >= 1 && (startingFile + deltaX) <= 8 && (startingFile + deltaX) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int[] getMaximumDiagonalOffsets(string startingSquare, PieceColour colour)
        {
            int[] maximums = new int[4];
            
            for(int i = 0; i < 4; i++)
            {
                string previousSquare = startingSquare;
                int max = 0;
                bool isLegal = true;
                while(isLegal)
                {
                    if (changeIsWithinBoard(diagonalXDeltas[i], diagonalYDeltas[i], previousSquare))
                    {
                        string currentSquare = Square.offsetCoordinate(diagonalXDeltas[i], diagonalYDeltas[i], previousSquare);
                        if (boardMap[currentSquare].Colour == colour)
                        {
                            isLegal = false;
                        }
                        else if (boardMap[currentSquare].Colour == PieceColour.blank)
                        {
                            max++;
                        }
                        else
                        {
                            max++;
                            isLegal = false;
                        }
                        previousSquare = currentSquare;
                    }
                    else
                    {
                        isLegal = false;
                    }
                    
                }
                maximums[i] = max;
                
            }          
            return maximums;          
        }
    }
}
