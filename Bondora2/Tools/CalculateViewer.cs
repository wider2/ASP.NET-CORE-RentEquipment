
namespace Bondora2.Tools
{
    public class CalculateViewer
    {
        private ICalculate _calculate;
        public CalculateViewer(ICalculate calculate)
        {
            _calculate = calculate;
        }

        public decimal CalculatePrices(int type, int days)
        {
            return _calculate.CalcPrices(type, days);
        }

        public int CalculatePoints(int type)
        {
            return _calculate.CalcPoints(type);
        }
    }
}
