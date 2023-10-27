using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class Move
    {

        public string startSquare;
        public string targetSquare;
        public string coordinateNotation;
        public Piece pieceToMove;
        public PieceType promotionPiece;

        public Move(string coordNotation, Board board)
        {
            coordinateNotation = coordNotation;
            startSquare = coordNotation.Substring(0, 2);
            targetSquare = coordNotation.Substring(2, 2);
            pieceToMove = board.boardMap[startSquare];                                
        }
        public bool isEnPassant(Board board)
        {
           if (pieceToMove.Type == PieceType.pawn && board.boardMap[targetSquare].Type == PieceType.blank)
           {
                return true;
           }
           else
           {
                return false;
           }
        }
        public bool isPromotionMove()
        {
            if (coordinateNotation.Contains('='))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool isDoublePawnMove()
        {
            if (MathF.Abs(Square.getRank(targetSquare) - Square.getRank(startSquare)) == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public PieceType getPromotionPiece(Board board)
        {
           return board.piecesDict[Char.ToLower(Char.Parse(this.coordinateNotation.Substring(5, 1)))];
        }
    }
}
