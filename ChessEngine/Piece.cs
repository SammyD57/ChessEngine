using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public enum PieceType
    {
        pawn,
        knight,
        bishop,
        rook,
        queen,
        king,
        blank
    }
    public enum PieceColour
    {
        white,
        black,
        blank
    }
    public class Piece
    {
        public PieceType Type { get; }
        public PieceColour Colour { get; }
        public int numMovesMade = 0;
        public Piece(PieceType type, PieceColour colour)
        {
            Type = type;
            Colour = colour;
        }
    }
}
