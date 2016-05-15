using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Kontur.Courses.Testing.Tdd
{
    public class Frame
    {
        public List<int> Rolls { get; } = new List<int>();
        public bool IsStrike => Rolls.Any() && Rolls[0] == 10;
        public bool IsSpare => !IsStrike && Rolls.Count() >= 2 && Rolls[0] + Rolls[1] == 10;
        public int Score => Rolls.Sum();
    }

    public class Game
    {
        private readonly IList<Frame> frames = new List<Frame>();

        public void Roll(int pins)
        {
            if (IsNextFrame())
                frames.Add(new Frame());
            foreach (var frame in AllNotFinishedFrames())
                frame.Rolls.Add(pins);

        }
        private bool IsNextFrame() =>
            !frames.Any()
            || (frames.Last().IsStrike || frames.Last().IsSpare || frames.Last().Rolls.Count() == 2)
            && frames.Count() < 10;

        private IEnumerable<Frame> AllNotFinishedFrames() => frames.Where(IsNotFinishedFrame);

        private static bool IsNotFinishedFrame(Frame f) => f.Rolls.Count < ((f.IsStrike || f.IsSpare) ? 3 : 2);

        public IList<Frame> GetFrames() => frames;
        public int Score => frames.Sum(x => x.Score);
    }

    [TestFixture]
    public class BowlingGame_Should
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game();
        }

        [Test]
        public void HaveEmptyFrame_BeforeAnyRolls()
        {
            Assert.AreEqual(new List<Frame>(), game.GetFrames());
        }

        [Test]
        public void HaveOneFrame_AfterFirstRoll()
        {
            GetRolls(5);
            Assert.AreEqual(1, game.GetFrames().Count);
        }

        [Test]
        public void Score_AfterTwoRolls()
        {
            GetRolls(5, 4);
            Assert.AreEqual(9, game.Score);
        }

        [Test]
        public void TwoFrames_AfterThreeRolls()
        {
            GetRolls(Enumerable.Repeat(4, 3).ToArray());
            Assert.AreEqual(2, game.GetFrames().Count);
        }

        [Test]
        public void Strike_AfterRoll()
        {
            GetRolls(10);
            Assert.IsTrue(game.GetFrames()[0].IsStrike);
        }

        [Test]
        public void Spare_After2Rolls()
        {
            GetRolls(6, 4);
            Assert.IsTrue(game.GetFrames()[0].IsSpare);
        }

        [Test]
        public void NotRoll_AfterFinish()
        {
            GetRolls(Enumerable.Repeat(4, 20).ToArray());
            var score = game.Score;
            GetRolls(4);
            Assert.AreEqual(game.Score, score);
        }

        [Test]
        public void Score_AfterStrike()
        {
            GetRolls(10, 4, 4);
            Assert.AreEqual(26, game.Score);
        }

        [Test]
        public void Score_AfterSpare()
        {
            GetRolls(6, 4, 4, 5);
            Assert.AreEqual(23, game.Score);
        }

        [Test]
        public void Score_WhenSampleGame()
        {
            GetRolls(1, 4, 4, 5, 6, 4, 5, 5, 10, 0, 1, 7, 3, 6, 4, 10, 2, 8, 6);
            Assert.AreEqual(133, game.Score);
        }

        [Test]
        public void Score_After3RollsTotal10()
        {
            GetRolls(5, 2, 3);
            Assert.AreEqual(10, game.Score);
        }

        private void GetRolls(params int[] rolls)
        {
            foreach (var roll in rolls)
            {
                game.Roll(roll);
            }
        }
    }
}
