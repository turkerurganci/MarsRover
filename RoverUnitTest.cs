using System;
using Xunit;

namespace hepsiburada_turkerurganci_casestudy.test
{
    public class RoverUnitTest
    {
        [Theory, InlineData(0, 0, 'H')]
        public void SetPosition_NotExistCompassPoint(int initialX, int initialY, char initialCompassPoint)
        {
            var rover = new Rover();
            var res = rover.SetPosition(initialX, initialY, initialCompassPoint);

            Assert.False(res.Success);
            Assert.Equal("No compass for given value", res.Message);
        }

        [Theory, InlineData(0, 0, 'N')]
        public void SetPosition_AssertTrue(int initialX, int initialY, char initialCompassPoint)
        {
            var rover = new Rover();
            var res = rover.SetPosition(initialX, initialY, initialCompassPoint);

            Assert.True(res.Success);
        }
    }
}
