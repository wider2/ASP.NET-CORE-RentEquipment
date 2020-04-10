using Bondora2.Tools;
using Xunit;

namespace UnitTestApp.Tests
{
    public class UnitTestClass
    {
        private CalculateViewer calculateViewer;

        public UnitTestClass()
        {
            calculateViewer = new CalculateViewer(new Calculate());
        }

        [Fact]
        public void TestPrices()
        {
            decimal price = calculateViewer.CalculatePrices(3, 3);
            Assert.Equal(180, price);
        }


        [Fact]
        public void TestPoints()
        {
            int points = calculateViewer.CalculatePoints(3);
            System.Diagnostics.Debug.WriteLine("points found: " + points);
            Assert.Equal(1, points);

            int points2 = calculateViewer.CalculatePoints(1);
            Assert.Equal(2, points2);
        }


        [Fact]
        public void TestToken()
        {
            Token instance = new Token();
            string token = instance.CreateToken();
            Assert.NotEmpty(token);
        }
                
    }
}