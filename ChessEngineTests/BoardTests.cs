using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ChessEngine;

namespace ChessEngineTests
{
    public class BoardTests
    {
        [Theory]
        [InlineData("e4", PieceColour.white, "8/8/6P1/8/4B3/8/2p5/K1k4p", new[] {1,3,2,4})]
        [InlineData("a1", PieceColour.white, "8/8/8/8/8/8/8/B7", new[] { 7, 0, 0, 0 })]
        [InlineData("b5", PieceColour.black, "8/8/r7/1b6/k7/8/4B3/5K2", new[] { 3, 3, 0, 0 })]
        [InlineData("g3", PieceColour.black, "1R6/3k4/8/4p3/8/6b1/5P2/4K3", new[] { 1, 1, 1, 1 })]
        public void Board_getMaximumDiagonalOffset_TestCases(string square, PieceColour colour, string fen, int[] expected)
        {
            Board board = new Board();
            board.addFenToBoard(fen);
            int[] result = board.getMaximumDiagonalOffsets(square, colour);
            Assert.Equal(expected, result);          
        }
    }
}
