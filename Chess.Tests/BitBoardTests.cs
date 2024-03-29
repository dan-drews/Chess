﻿using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessLibrary;

namespace Chess.Tests
{
    [TestClass]
    public class BitBoardTests
    {
        [TestMethod]
        public void BitBoard_GetPosition_H1_Returns0()
        {
            var bb = new FullBitBoard();
            var position = bb.GetPositionFromFileAndRank(Files.H, 1);
            Assert.AreEqual(0, position);
        }

        [TestMethod]
        public void BitBoard_GetPosition_A8_Returns63()
        {
            var bb = new FullBitBoard();
            var position = bb.GetPositionFromFileAndRank(Files.A, 8);
            Assert.AreEqual(63, position);
        }
    }
}
