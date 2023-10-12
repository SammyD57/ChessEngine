using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ChessEngine;
using FluentAssertions;

namespace ChessEngineTests
{
    public class BoardTests
    {
        [Fact]
        public void Board_plyCount_isValid()
        {
            Board board = new Board();

            board.plyCount.Should().BeGreaterThanOrEqualTo(0);
        }
    }
}
