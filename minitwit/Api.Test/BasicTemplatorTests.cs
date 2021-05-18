using Xunit;
using System;

namespace Api.Test
{

    public class BasicTemplaterTests
    {
        [Fact]
        public void generateDateTimeStringTest()
        {
            var time = new DateTime(1000, 1, 2, 3, 4, 0);
            var expected = "- 1000-01-02 @ 03:04";
            var actual = BasicTemplater.generateDateTimeString(time);
            Assert.Equal(expected, actual);
        }
    }

}
