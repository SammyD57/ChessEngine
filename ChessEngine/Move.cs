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
        public Piece pieceToMove;

        public Move(string coordNotation, Board board)
        {
            startSquare = coordNotation.Substring(0, 2);
            targetSquare = coordNotation.Substring(2, 2);
            pieceToMove = board.boardMap[startSquare];
        }
    }
}
