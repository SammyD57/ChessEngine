using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public enum SquareColour
    {
        light,
        dark
    }



    public class Square
    {
        public SquareColour Colour { get; }
        public Square(SquareColour colour)
        {
            Colour = colour;
        }
    }
}
