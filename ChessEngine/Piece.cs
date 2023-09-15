using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public enum PieceType
    {
        pawn = 0,
        knight = 1,
        bishop = 2,
        rook = 3,
        queen = 4,
        king = 5
    }
    public enum Colour
    {
        white,
        black
    }
    public class Piece
    {
        public PieceType Type { get; }
        public Colour Colour { get; }
        public Piece(PieceType type, Colour colour)
        {
            Type = type;
            Colour = colour;
        }
    }
}
