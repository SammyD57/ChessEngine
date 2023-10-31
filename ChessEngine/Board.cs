namespace ChessEngine
{
    public class Board
    {
        public int plyCount = 0;
        public int fullMovesCount = 1;
        public Dictionary<string, Piece> boardMap = new Dictionary<string, Piece>();
        public string? enPassantSquare;
        public bool isWhiteToMove = true;

        public bool whiteHasKingsideCastleRight, whiteHasQueensideCastleRight, blackHasKingsideCastleRight, blackHasQueensideCastleRight;
        public int fiftyMoveRuleCount = 0;

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

        public Dictionary<char, PieceType> piecesDict = new Dictionary<char, PieceType>
        {
            {'p', PieceType.pawn },
            {'n', PieceType.knight },
            {'b', PieceType.bishop },
            {'r', PieceType.rook },
            {'q', PieceType.queen },
            {'k', PieceType.king }
        };

        public Dictionary<PieceType, float> pieceValues = new Dictionary<PieceType, float>
        {
            {PieceType.pawn, 1 },
            {PieceType.knight, 3 },
            {PieceType.bishop, 3.25f },
            {PieceType.rook, 5 },
            {PieceType.queen, 9 }
        }; 

        public Dictionary<string, float> attackDefendMap = new Dictionary<string, float>();
          
        private readonly int[] diagonalXDeltas = { 1, 1, -1, -1 };
        private readonly int[] diagonalYDeltas = { 1, -1, -1, 1 };

        private readonly int[] knightXDeltas = { 1, 2, 2, 1, -1, -2, -2, -1 };
        private readonly int[] knightYDeltas = { 2, 1, -1, -2, -2, -1, 1, 2 };

        private readonly int[] rookXDeltas = {1, 0, -1, 0 };
        private readonly int[] rookYDeltas = {0, -1, 0, 1 };

        private readonly int[] royalXDeltas = { 1, 1, 1, 0, -1, -1, -1, 0 };
        private readonly int[] royalYDeltas = { 1, 0, -1, -1, -1, 0, 1, 1 };

        public Board()
        {
            foreach(string square in allSquares)
            {
                attackDefendMap.Add(square, 0);
            }

        }

        public void makeMove(Move move)
        {
            boardMap[move.targetSquare] = boardMap[move.startSquare];
            boardMap[move.startSquare] = new Piece(PieceType.blank, PieceColour.blank);
            move.pieceToMove.numMovesMade++;
            plyCount++;
            isWhiteToMove = !isWhiteToMove;
            updateAttackDefendMap();

            if (move.isDoublePawnMove())
            {
                int yOffset = move.pieceToMove.Colour == PieceColour.white ? 1 : -1;
                enPassantSquare = Square.offsetCoordinate(0, yOffset, move.startSquare);                
            }
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

            string activeColour = fenSections[1];
            if(activeColour == "w")
            {
                isWhiteToMove = true;
            }
            else
            {
                isWhiteToMove = false;
            }

            char[] castlingRights = fenSections[2].ToCharArray();
            foreach (char c in castlingRights)
            {
                switch (c)
                {
                    case 'K':
                        whiteHasKingsideCastleRight = true; 
                        break;
                    case 'Q':
                        whiteHasQueensideCastleRight = true;
                        break;
                    case 'k':
                        blackHasKingsideCastleRight = true;
                        break;
                    case 'q':
                        blackHasQueensideCastleRight = true;
                        break; 
                }
            }

            if (fenSections[3] != "-")
            {
                enPassantSquare = fenSections[3];
            }
            fiftyMoveRuleCount = int.Parse(fenSections[4]);
            fullMovesCount = int.Parse(fenSections[5]);
        }
        public void setStartingPosition()
        {
            addFenToBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void updateAttackDefendMap()
        {
            int colourMultiplier = isWhiteToMove ? 1 : -1;
            var legalMoves = generateAllLegalMoves();
            var pawnAttackSquares = getPawnAttackSquares(false);

            foreach(Move move in legalMoves)
            {
                if (move.pieceToMove.Type != PieceType.pawn)
                {
                    attackDefendMap[move.targetSquare] += pieceValues[move.pieceToMove.Type] * colourMultiplier;
                }
            }
            foreach(string square in pawnAttackSquares)
            {
                attackDefendMap[square] += colourMultiplier;
            }
        }

        public Piece[] getArrayOfPieces(PieceColour colour, PieceType type)
        {
            var pieces = new List<Piece>();

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
            var squares = new List<string>();
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
            var squares = new List<string>();
            foreach (var kvp in boardMap)
            {
                if (kvp.Value.Type == type && kvp.Value.Colour == colour)
                {
                    squares.Add(kvp.Key);
                }
            }
            return squares.ToArray();
        }
        public List<Move> generateAllLegalMoves()
        {
            var pawnMoves = generateLegalPawnMoves();            
            var knightMoves = generateLegalKnightMoves();
            var bishopMoves = generateLegalBishopMoves();
            var rookMoves = generateLegalRookMoves();
            var queenMoves = generateLegalQueenMoves();
            //add King moves 
            return pawnMoves.Concat(knightMoves).Concat(bishopMoves).Concat(rookMoves).Concat(queenMoves).ToList();
        }
        
        public List<Move> generateCaptureMoves()
        {
                var legalMoves = generateAllLegalMoves();
                var captureMoves = new List<Move>();

            foreach(Move move in legalMoves)
            {
                if (move.isCaptureMove(this))
                {
                    captureMoves.Add(move);
                }
            }
            return captureMoves;
        }
        public List<string> getPawnAttackSquares(bool uniqueOnly)
        {
            var pawnAttackSquares = new List<string>();
            string[] pawnSquares = getSquaresContainingPiece(PieceType.pawn, isWhiteToMove ? PieceColour.white : PieceColour.black);
            int yChange = (isWhiteToMove ? 1 : -1);
            foreach (string square in pawnSquares)
            {
                for(int i = -1; i < 2; i += 2)
                {
                    if (changeIsWithinBoard(i, yChange, square))
                    {
                        string targetSquare = Square.offsetCoordinate(i, yChange, square);
                        if (uniqueOnly && !pawnAttackSquares.Contains(targetSquare))
                        {
                            pawnAttackSquares.Add(targetSquare);
                        }
                        else if(!uniqueOnly)
                        {
                            pawnAttackSquares.Add(targetSquare);
                        }
                    }                      
                }  
            }
            return pawnAttackSquares;
        }
        public List<Move> generateLegalPawnMoves()
        {
            var moves = new List<Move>();
            PieceColour pawnColour = (isWhiteToMove) ? PieceColour.white : PieceColour.black;
            int yChange = (isWhiteToMove ? 1 : -1);
            string[] squaresWithPawns = getSquaresContainingPiece(PieceType.pawn, pawnColour);
            char[] promotionPieces = { 'n', 'b', 'r', 'q' };

            foreach (var square in squaresWithPawns)
            {
                if (changeIsWithinBoard(0, yChange, square) && boardMap[Square.offsetCoordinate(0, yChange, square)].Type == PieceType.blank)
                {
                    string targetSquare = Square.offsetCoordinate(0, yChange, square);
                    int rank = int.Parse(targetSquare.Substring(1, 1));
                    //promotion white
                    if (rank == 8)
                    {
                        foreach (char promotionPiece in promotionPieces)
                        {
                            moves.Add(new Move(square + targetSquare + "=" + Char.ToUpper(promotionPiece), this));
                        }
                    }
                    //promotion black
                    else if (rank == 1)
                    {
                        foreach (char promotionPiece in promotionPieces)
                        {
                            moves.Add(new Move(square + targetSquare + "=" + promotionPiece, this));
                        }
                    }
                    //Move 1 square
                    else
                    {
                        moves.Add(new Move(square + targetSquare, this));
                        //Move 2 squares
                        if (boardMap[square].numMovesMade == 0 && boardMap[Square.offsetCoordinate(0, yChange * 2, square)].Type == PieceType.blank)
                        {
                            targetSquare = Square.offsetCoordinate(0, yChange * 2, square);
                            moves.Add(new Move(square + targetSquare, this));
                        }
                    }

                }
                //Captures + EnPassant
                for (int i = -1; i < 2; i += 2)
                {
                    if (changeIsWithinBoard(i, yChange, square) && (boardMap[Square.offsetCoordinate(i, yChange, square)].Type != PieceType.blank || Square.offsetCoordinate(i, yChange, square) == enPassantSquare))
                    {
                        string targetSquare = Square.offsetCoordinate(i, yChange, square);
                        moves.Add(new Move(square + targetSquare, this));
                    }
                }
            }
            return moves;
        }

        public List<Move> generateLegalKnightMoves()
        {
            var moves = new List<Move>();          
            PieceColour knightColour = (isWhiteToMove) ? PieceColour.white : PieceColour.black;           
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
            return moves;
        }

        public List<Move> generateLegalBishopMoves()
        {
            var moves = new List<Move>();
            PieceColour bishopColour = isWhiteToMove ? PieceColour.white : PieceColour.black;
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

            return moves;
        }
        
        public List<Move> generateLegalRookMoves()
        {
            var moves = new List<Move>();
            PieceColour rookColour = isWhiteToMove ? PieceColour.white : PieceColour.black;
            string[] squaresWithRooks = getSquaresContainingPiece(PieceType.rook, rookColour);

            foreach (string square in squaresWithRooks)
            {
                int[] maximumStraights = getMaximumStraightOffsets(square, rookColour);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 1; j <= maximumStraights[i]; j++)
                    {
                        string targetSquare = Square.offsetCoordinate(rookXDeltas[i] * j, rookYDeltas[i] * j, square);
                        moves.Add(new Move(square + targetSquare, this));
                    }
                }
            }
            return moves;
        }

        public List<Move> generateLegalQueenMoves()
        {
            var moves = new List<Move>();
            PieceColour queenColour = isWhiteToMove ? PieceColour.white : PieceColour.black;
            string[] squaresWithQueen = getSquaresContainingPiece(PieceType.queen, queenColour);   
            
            foreach (var square in squaresWithQueen)
            {
                int[] queenOffsets = getCombinedStraightAndDiagonalOffsets(square, queenColour);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 1; j <= queenOffsets[i]; j++)
                    {
                        string targetSquare = Square.offsetCoordinate(royalXDeltas[i] * j, royalYDeltas[i] * j, square);
                        moves.Add(new Move(square + targetSquare, this));
                    }
                }
            }
            return moves; 
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
        public int[] getMaximumStraightOffsets(string startingSquare, PieceColour colour)
        {
            int[] maximums = new int[4];
            for(int i = 0; i < 4; i++)
            {
                string previousSquare = startingSquare;
                int max = 0;
                bool isLegal = true;
                while (isLegal)
                {
                    if (changeIsWithinBoard(rookXDeltas[i], rookYDeltas[i], previousSquare))
                    {
                        string currentSquare = Square.offsetCoordinate(rookXDeltas[i], rookYDeltas[i], previousSquare);
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

        public int[] getCombinedStraightAndDiagonalOffsets(string startingSquare, PieceColour colour)
        {
            var maximums = new List<int>();
            int[] diagonals = getMaximumDiagonalOffsets(startingSquare, colour);
            int[] straights = getMaximumStraightOffsets(startingSquare, colour);

            for(int i = 0; i < 4; i++)
            {
                maximums.Add(diagonals[i]);
                maximums.Add(straights[i]);
            }
            return maximums.ToArray();
        }
    }
}