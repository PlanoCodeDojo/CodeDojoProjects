using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace BotCleanLarge
{
    [TestFixture]
    public class Tests
    {
        private IBot _bot;

        [Test]
        public void test_case_1()
        {
            //Arrange
            var matrix = new string[]
                {
                    "bd---",
                    "-----",
                    "-----",
                    "-----",
                    "-----"
                };
            _bot = new Bot();

            //Act
            string move = _bot.next_move(0,0,5,5, matrix);

            //Assert
            move.Should().Be("RIGHT");
        }

        [Test]
        public void test_case_2()
        {
            //Arrange
            var matrix = new string[]
                {
                    "db---",
                    "-----",
                    "-----",
                    "-----",
                    "-----"
                };
            _bot = new Bot();


            //Act
            string move = _bot.next_move(0, 1, 5, 5, matrix);

            //Assert
            move.Should().Be("LEFT");

        }

        [Test]
        public void when_bot_lands_on_dirty_then_next_move_is_clean()
        {
            //Arrange
            var matrix = new string[]
                {
                    "db---",
                    "-----",
                    "-----",
                    "-----",
                    "-----"
                };
            _bot = new Bot();
            _bot.next_move(0, 1, 5, 5, matrix);
            var move = _bot.next_move(_bot.CurrentBotPosition.Column, _bot.CurrentBotPosition.Row, 5, 5, _bot.MatrixState);

            ////Assert
            move.Should().Be("CLEAN");
            AssertMatrixIsClean();
        }

        [Test]
        public void test_case_3()
        {
            //Arrange
            var matrix = new string[]
                {
                    "b---d",
                    "d---d",
                    "-----",
                    "-----",
                    "-----"
                };
            _bot = new Bot();
            
            //Act

            _bot.next_move(0, 0, 5, 5, matrix);

            for (int move = 1; move < 9; move++)
            {
                var m = _bot.next_move(_bot.CurrentBotPosition.Row, _bot.CurrentBotPosition.Column, 5, 5, _bot.MatrixState);
            }

            AssertMatrixIsClean();

        }

        [Test]
        public void when_finding_the_closest_edge_i_expect_values_0_4()
        {

            //Arrange
            var matrix = new string[]
                {
                    "----d",
                    "-----",
                    "---d-",
                    "d-d-b",
                    "-----"
                };
            _bot = new Bot();

            //Act

            _bot.next_move(3, 4, 5, 5, matrix);

            //_bot.next_move(3, 4, 5, 5, matrix);
            _bot.ClosestEdge.Position.Row.Should().Be(0);
            _bot.ClosestEdge.Position.Column.Should().Be(4);
        }

        [Test]
        public void test_case_4()
        {

            //Arrange
            var matrix = new string[]
                {
                    "----d",
                    "-----",
                    "---d-",
                    "d-d-b",
                    "-----"
                };
            _bot = new Bot();

            //Act

            _bot.next_move(3, 4, 5, 5, matrix);

            for (int move = 1; move < 14; move++)
            {
                var m = _bot.next_move(_bot.CurrentBotPosition.Row, _bot.CurrentBotPosition.Column, 5, 5, _bot.MatrixState);
            }

            AssertMatrixIsClean();
        }


        private void AssertMatrixIsClean()
        {
            foreach (var row in _bot.MatrixState)
            {
                foreach (var character in row)
                {
                    if(character == 'b') continue;
                    character.Should().Be('-');
                }
            }
        }





    }
}
