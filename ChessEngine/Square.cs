using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChessEngine
{
    public static class Square
    {      
        public static bool isWhite(string square)
        {
            char file = getFile(square);
            int rank = getRank(square);
            
            if(rank % 2 == 0 && getFileAsInt(file) % 2 == 0)
            {
                return false;
            }
            else if(rank % 2 == 1 && getFileAsInt(file) % 2 == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static char getFile(string square)
        {
            return char.Parse(square.Substring(0, 1));
        }
        public static int getRank(string square)
        {
            return int.Parse(square.Substring(1, 1));
        }

        public static int getFileAsInt(char f)
        {
            return (int) f - 96;
        }
        public static char getIntAsFile(int f)
        {
            return Convert.ToChar(f + 96);
        }
        public static string squareFromInts(int rank, int file)
        {
            string square = getIntAsFile(file).ToString() + rank.ToString();
            return square;
        }
        public static string offsetCoordinate(int deltaX, int deltaY, string startingSquare)
        {
            string newCoordinate;
            int rank = int.Parse(startingSquare.Substring(1, 1));
            int file = Square.getFileAsInt(Char.Parse(startingSquare.Substring(0, 1)));
            rank += deltaY;
            file += deltaX;
            newCoordinate = Square.getIntAsFile(file).ToString() + rank.ToString();
            return newCoordinate;
        }
    }
}
