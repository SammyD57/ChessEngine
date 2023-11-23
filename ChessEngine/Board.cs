using ChessEngine.Utilities;
using System.Drawing;

namespace ChessEngine
{
    public class Board
    {
        public int plyCount { get; set; }
        public int fullMovesCount { get; set; }
        public int fiftyMoveRuleCount { get; set; }
        public Dictionary<string, Piece> boardMap { get; set; }
        public string? enPassantSquare { get; set; }
        public bool isWhiteToMove { get; set; }
        public int colourMultiplier { get; set; }
        public PieceColour colourToMove { get; set; }
        public List<Move> moveLog { get; set; }
        public List<Board> boardStatesHistory { get; set; }

        public bool whiteHasKingsideCastleRight, whiteHasQueensideCastleRight, blackHasKingsideCastleRight, blackHasQueensideCastleRight;

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

        public float[] pieceValues = { 1, 3, 3.25f, 5, 9 };

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
            plyCount = 0;
            fullMovesCount = 1;
            boardMap = new Dictionary<string, Piece>();
            moveLog = new List<Move>();
            boardStatesHistory = new List<Board>();
            colourMultiplier = 1;
            colourToMove = PieceColour.white;
        }

        public void MakeMove(Move move)
        {
            boardStatesHistory.Add(this);
            moveLog.Add(move);


            if (!move.isPromotionMove())
            {
                boardMap[move.targetSquare] = boardMap[move.startSquare];
                boardMap[move.startSquare] = new Piece(PieceType.blank, PieceColour.blank);
            }
            else
            {
                boardMap[move.targetSquare] = new Piece(move.promotionPiece, isWhiteToMove ? PieceColour.white: PieceColour.black);
                boardMap[move.startSquare] = new Piece(PieceType.blank, PieceColour.blank);
            }

            move.pieceToMove.numMovesMade++;
            plyCount++;
            isWhiteToMove = !isWhiteToMove;
            colourMultiplier *= -1;
            colourToMove = isWhiteToMove ? PieceColour.white : PieceColour.black;

            //Set en passant square
            if (move.isDoublePawnMove())
            {
                int yOffset = (move.pieceToMove.Colour == PieceColour.white) ? 1 : -1;
                enPassantSquare = Square.offsetCoordinate(0, yOffset, move.startSquare);                
            }        
            else
            {
                enPassantSquare = string.Empty;
            }
        }
        public void undoMove(int steps)
        {
            Board previousBoard = boardStatesHistory[boardStatesHistory.Count - steps];

            this.boardMap = previousBoard.boardMap;
            this.plyCount = previousBoard.plyCount;
            this.fullMovesCount = previousBoard.fullMovesCount;
            this.fiftyMoveRuleCount = previousBoard.fiftyMoveRuleCount;
            this.enPassantSquare = previousBoard.enPassantSquare;
            this.isWhiteToMove = previousBoard.isWhiteToMove;
            this.moveLog = previousBoard.moveLog;
            this.boardStatesHistory = previousBoard.boardStatesHistory;
            this.whiteHasKingsideCastleRight = previousBoard.whiteHasKingsideCastleRight;
            this.whiteHasQueensideCastleRight = previousBoard.whiteHasQueensideCastleRight;
            this.blackHasKingsideCastleRight = previousBoard.blackHasKingsideCastleRight;
            this.blackHasQueensideCastleRight = previousBoard.blackHasQueensideCastleRight;
        }
      
        public void SetStartingPosition()
        {
            FenUtility.AddFen(this, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void UpdateAttackDefendMap()
        {
            var legalMoves = GenerateAllLegalMoves();
            var pawnAttackSquares = GetPawnAttackSquares(false);

            foreach(Move move in legalMoves)
            {
                if (move.pieceToMove.Type != PieceType.pawn)
                {
                    attackDefendMap[move.targetSquare] += pieceValues[(int) move.pieceToMove.Type] * colourMultiplier;
                }
            }
            foreach(string square in pawnAttackSquares)
            {
                attackDefendMap[square] += colourMultiplier;
            }
        }

        public List<string> GetSquaresContainingPiece(PieceType type, PieceColour colour)
        {
            var squares = new List<string>();
            foreach (var kvp in boardMap)
            {
                if (kvp.Value.Type == type && kvp.Value.Colour == colour)
                {
                    squares.Add(kvp.Key);
                }
            }
            return squares;
        }
        public string GetKingSquare(PieceColour colour)
        {
            foreach (var kvp in boardMap)
            {
                if (kvp.Value.Type == PieceType.king && kvp.Value.Colour == colour)
                {
                    return kvp.Key;
                }
            }
            return string.Empty;
        }

        public List<Move> GenerateAllLegalMoves()
        {
            var pawnMoves = GenerateLegalPawnMoves();            
            var knightMoves = GenerateLegalKnightMoves();
            var bishopMoves = GenerateLegalBishopMoves();
            var rookMoves = GenerateLegalRookMoves();
            var queenMoves = GenerateLegalQueenMoves();
            //add King moves 
            return pawnMoves.Concat(knightMoves).Concat(bishopMoves).Concat(rookMoves).Concat(queenMoves).ToList();
        }
        
        public List<Move> GenerateCaptureMoves()
        {
                var legalMoves = GenerateAllLegalMoves();
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

        public bool IsInCheck(PieceColour kingColour)
        {           
            string kingSquare = GetKingSquare(kingColour);

            //Knight Checks
            for (int i = 0; i < 8; i++)
            {
                if (ChangeIsWithinBoard(knightXDeltas[i], knightYDeltas[i], kingSquare))
                {
                    string newSquare = Square.offsetCoordinate(knightXDeltas[i], knightYDeltas[i], kingSquare);
                    if (boardMap[newSquare].Type == PieceType.knight && boardMap[newSquare].Colour != kingColour)
                    {
                        return true;
                    }
                }
            }
            //Rook and Queen Checks
            for (int i = 0; i < 4; i++)
            {
                string previousSquare = kingSquare;
                bool isLegal = true;
                while (isLegal)
                {
                    if (ChangeIsWithinBoard(rookXDeltas[i], rookYDeltas[i], previousSquare))
                    {
                        string currentSquare = Square.offsetCoordinate(rookXDeltas[i], rookYDeltas[i], previousSquare);
                        Piece pieceOnSquare = boardMap[currentSquare];
                        if (pieceOnSquare.Colour != kingColour && (pieceOnSquare.Type == PieceType.rook || pieceOnSquare.Type == PieceType.queen))
                        {
                            return true;
                        }
                        previousSquare = currentSquare;
                    }
                    else { break; }
                }
            }
            //Bishop and Queen Checks
            for (int i = 0; i < 4; i++)
            {
                string previousSquare = kingSquare;
                bool isLegal = true;
                while (isLegal)
                {
                    if (ChangeIsWithinBoard(diagonalXDeltas[i], diagonalYDeltas[i], previousSquare))
                    {
                        string currentSquare = Square.offsetCoordinate(diagonalXDeltas[i], diagonalYDeltas[i], previousSquare);
                        Piece pieceOnSquare = boardMap[currentSquare];
                        if (pieceOnSquare.Colour != kingColour && (pieceOnSquare.Type == PieceType.bishop || pieceOnSquare.Type == PieceType.queen))
                        {
                            return true;
                        }
                        previousSquare = currentSquare;
                    }
                    else { break; }
                }
            }
            //Pawn Checks
            int y = colourMultiplier;
            for(int x  = -1; x < 2; x += 2)
            {
                if (ChangeIsWithinBoard(x, y, kingSquare))
                {
                    Piece pieceOnSquare = boardMap[Square.offsetCoordinate(x, y, kingSquare)];
                    if (pieceOnSquare.Type == PieceType.pawn && pieceOnSquare.Colour != kingColour) { return true; }
                }                
            }            
            return false;
        }

        public List<string> GetPawnAttackSquares(bool uniqueOnly)
        {
            var pawnAttackSquares = new List<string>();
            var pawnSquares = GetSquaresContainingPiece(PieceType.pawn, isWhiteToMove ? PieceColour.white : PieceColour.black);
            int yChange = colourMultiplier;
            foreach (string square in pawnSquares)
            {
                for(int i = -1; i < 2; i += 2)
                {
                    if (ChangeIsWithinBoard(i, yChange, square))
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
        public List<Move> GenerateLegalPawnMoves()
        {
            var moves = new List<Move>();
            PieceColour pawnColour = colourToMove;
            int yChange = colourMultiplier;
            var squaresWithPawns = GetSquaresContainingPiece(PieceType.pawn, pawnColour);
            char[] promotionPieces = { 'n', 'b', 'r', 'q' };

            foreach (var square in squaresWithPawns)
            {
                if (ChangeIsWithinBoard(0, yChange, square) && boardMap[Square.offsetCoordinate(0, yChange, square)].Type == PieceType.blank)
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
                    if (ChangeIsWithinBoard(i, yChange, square) && (boardMap[Square.offsetCoordinate(i, yChange, square)].Type != PieceType.blank || Square.offsetCoordinate(i, yChange, square) == enPassantSquare))
                    {
                        string targetSquare = Square.offsetCoordinate(i, yChange, square);
                        moves.Add(new Move(square + targetSquare, this));
                    }
                }
            }
            return FilterIllegalCheckMoves(moves);
        }

        public List<Move> GenerateLegalKnightMoves()
        {
            var moves = new List<Move>();          
            PieceColour knightColour = colourToMove;           
            var squaresWithKnights = GetSquaresContainingPiece(PieceType.knight, knightColour);    

            foreach (string square in squaresWithKnights)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (ChangeIsWithinBoard(knightXDeltas[i], knightYDeltas[i], square))
                    {
                        string newSquare = Square.offsetCoordinate(knightXDeltas[i], knightYDeltas[i], square);
                        if (boardMap[newSquare].Colour != knightColour)
                        {
                            moves.Add(new Move(square + newSquare, this));
                        }                   
                    }
                }          
            }
            return FilterIllegalCheckMoves(moves);
        }

        public List<Move> GenerateLegalBishopMoves()
        {
            var moves = new List<Move>();
            PieceColour bishopColour = colourToMove;
            var squaresWithBishops = GetSquaresContainingPiece(PieceType.bishop, bishopColour);

            foreach(string square in squaresWithBishops)
            {
                int[] maximumDiagonals = GetMaximumDiagonalOffsets(square, bishopColour);
                for (int i = 0; i < 4; i++)
                {
                    for(int j = 1; j <= maximumDiagonals[i]; j++)
                    {
                        string targetSquare = Square.offsetCoordinate(diagonalXDeltas[i] * j, diagonalYDeltas[i] * j , square);
                        moves.Add(new Move(square + targetSquare, this));
                    }           
                }
            }

            return FilterIllegalCheckMoves(moves);
        }
        
        public List<Move> GenerateLegalRookMoves()
        {
            var moves = new List<Move>();
            PieceColour rookColour = colourToMove;
            var squaresWithRooks = GetSquaresContainingPiece(PieceType.rook, rookColour);

            foreach (string square in squaresWithRooks)
            {
                int[] maximumStraights = GetMaximumStraightOffsets(square, rookColour);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 1; j <= maximumStraights[i]; j++)
                    {
                        string targetSquare = Square.offsetCoordinate(rookXDeltas[i] * j, rookYDeltas[i] * j, square);
                        moves.Add(new Move(square + targetSquare, this));
                    }
                }
            }
            return FilterIllegalCheckMoves(moves);
        }

        public List<Move> GenerateLegalQueenMoves()
        {
            var moves = new List<Move>();
            PieceColour queenColour = colourToMove;
            var squaresWithQueen = GetSquaresContainingPiece(PieceType.queen, queenColour);   
            
            foreach (var square in squaresWithQueen)
            {
                int[] queenOffsets = GetCombinedStraightAndDiagonalOffsets(square, queenColour);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 1; j <= queenOffsets[i]; j++)
                    {
                        string targetSquare = Square.offsetCoordinate(royalXDeltas[i] * j, royalYDeltas[i] * j, square);
                        moves.Add(new Move(square + targetSquare, this));
                    }
                }
            }
            return FilterIllegalCheckMoves(moves); 
        }

        public List<Move> FilterIllegalCheckMoves(List<Move> moves)
        {
            var filteredMoves = new List<Move>();
            PieceColour colourToMove;
            foreach (var move in moves)
            {
                this.MakeMove(move);
                if (!IsInCheck(colourToMove))
                {
                    filteredMoves.Add(move);
                }
                this.undoMove(1);
            }
            return filteredMoves;
        }

        public bool ChangeIsWithinBoard(int deltaX, int deltaY, string startingSquare)
        {
            int startingRank = int.Parse(startingSquare.Substring(1, 1));
            int startingFile = Square.getFileAsInt(Char.Parse(startingSquare.Substring(0, 1)));
            bool isOnFileWithinBoard = ((startingRank + deltaY) <= 8 && (startingRank + deltaY) >= 1); 
            bool isOnRankWithinBoard = ((startingFile + deltaX) <= 8 && (startingFile + deltaX) >= 1);
            if (isOnFileWithinBoard && isOnRankWithinBoard) 
            {
                return true;
            }
            return false;
        }
        public int[] GetMaximumDiagonalOffsets(string startingSquare, PieceColour colour)
        {
            int[] maximums = new int[4];
            
            for(int i = 0; i < 4; i++)
            {
                string previousSquare = startingSquare;
                int max = 0;
                bool isLegal = true;
                while(isLegal)
                {
                    if (ChangeIsWithinBoard(diagonalXDeltas[i], diagonalYDeltas[i], previousSquare))
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
        public int[] GetMaximumStraightOffsets(string startingSquare, PieceColour colour)
        {
            int[] maximums = new int[4];
            for(int i = 0; i < 4; i++)
            {
                string previousSquare = startingSquare;
                int max = 0;
                bool isLegal = true;
                while (isLegal)
                {
                    if (ChangeIsWithinBoard(rookXDeltas[i], rookYDeltas[i], previousSquare))
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

        public int[] GetCombinedStraightAndDiagonalOffsets(string startingSquare, PieceColour colour)
        {
            var maximums = new List<int>();
            int[] diagonals = GetMaximumDiagonalOffsets(startingSquare, colour);
            int[] straights = GetMaximumStraightOffsets(startingSquare, colour);

            for(int i = 0; i < 4; i++)
            {
                maximums.Add(diagonals[i]);
                maximums.Add(straights[i]);
            }
            return maximums.ToArray();
        }
    }
}