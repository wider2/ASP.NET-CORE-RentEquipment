
namespace Bondora.Api.Tools
{
    public class CalculateViewer
    {
        internal ICalculate _calculate;
        internal CalculateViewer(ICalculate calculate)
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
