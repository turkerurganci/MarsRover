using System;
using System.Linq;
using Xunit;

namespace hepsiburada_turkerurganci_casestudy.test
{
    public class RoverManagerUnitTest
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        public void SetUpperRightCoordinates_UnExpectedCoordinates(int maxX, int maxY)
        {
            var roverManager = new RoverManager();
            var res = roverManager.SetUpperRightCoordinates(maxX, maxY);

            Assert.False(res.Success);
            Assert.Equal("Values must be grater than zero(0)", res.Message);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 100)]
        public void SetUpperRightCoordinates_AssertTrue(int maxX, int maxY)
        {
            var roverManager = new RoverManager();
            var res = roverManager.SetUpperRightCoordinates(maxX, maxY);

            Assert.True(res.Success);
        }

        [Fact]
        public void AddRover_AssertTrue()
        {
            var rover = new Rover();
            var roverManager = new RoverManager();

            roverManager.AddRover(rover);

            Assert.True(roverManager.GetRovers().Contains(rover));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("LRA")]
        [InlineData("ARN")]
        [InlineData("LRNA")]
        public void ApplyRoversCommands_UnexpectedCommand(string letters)
        {
            var rover = new Rover();
            var roverManager = new RoverManager();
            rover.SetPosition(1, 1, 'N');
            rover.SetLetters(letters);

            roverManager.AddRover(rover);

            var res = roverManager.ApplyRoversCommands();

            Assert.False(res.Success);
            Assert.Equal("Unexpected command", res.Message);
        }

        [Theory]
        [InlineData("MMRMMRMRRM")]
        public void ApplyRoversCommands_AssertTrue(string letters)
        {
            var rover = new Rover();
            var roverManager = new RoverManager();
            roverManager.SetUpperRightCoordinates(5, 5);
            rover.SetPosition(1, 1, 'N');
            rover.SetLetters(letters);

            roverManager.AddRover(rover);

            var res = roverManager.ApplyRoversCommands();

            Assert.True(res.Success, res.Message);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(6, 6)]
        [InlineData(1, 6)]
        public void CheckPosition_OutOfRegion(int initialX, int initialY)
        {
            var rover = new Rover();
            var roverManager = new RoverManager();
            roverManager.SetUpperRightCoordinates(5, 5);
            rover.SetPosition(initialX, initialY, 'N');

            var res = roverManager.CheckPosition(rover);

            Assert.False(res.Success);
            Assert.Equal("Rover was out of region", res.Message);
        }

        [Theory]
        [InlineData(1, 1)]
        public void CheckPosition_AssertTrue(int initialX, int initialY)
        {
            var rover = new Rover();
            var roverManager = new RoverManager();
            roverManager.SetUpperRightCoordinates(5, 5);
            rover.SetPosition(initialX, initialY, 'N');

            var res = roverManager.CheckPosition(rover);

            Assert.True(res.Success, res.Message);
        }


        [Theory]
        [InlineData(5, 5, 1, 2, 'N', "LMLMLMLMM", "1 3 N")]
        [InlineData(5, 5, 3, 3, 'E', "MMRMMRMRRM", "5 1 E")]
        public void TestCases_AssertTrue(int maxX, int maxY, int initialX, int initialY, char compassPointValue, string commandLetters, string outputValue)
        {
            var rover = new Rover();
            var roverManager = new RoverManager();

            roverManager.SetUpperRightCoordinates(maxX, maxY);
            rover.SetPosition(initialX, initialY, compassPointValue);
            rover.SetLetters(commandLetters);
            roverManager.AddRover(rover);
            roverManager.ApplyRoversCommands();
            var position = roverManager.GetRovers()?.Select(t => t.GetPosition()).First();

            Assert.Equal(outputValue, position);
        }
    }
}
