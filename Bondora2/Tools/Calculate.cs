
namespace Bondora2.Tools
{
    public interface ICalculate
    {
        decimal CalcPrices(int type, int days);

        int CalcPoints(int type);
    }

    public class Calculate : ICalculate
    {
        public decimal CalcPrices(int type, int days)
        {
            decimal price=0;
            int rentalFee = 100, premiumFee = 60, regularFee = 40;

            if (type == 1) price = rentalFee + (premiumFee * days);
            if (type == 2) price = rentalFee + ((days <= 2) ? (premiumFee * days): 0) + ((days >2) ? (regularFee * days): 0);
            if (type == 3) price = ((days <= 3) ? (premiumFee * days) : 0) + ((days > 3) ? (regularFee * days) : 0);

            return price;
        }

        public int CalcPoints(int type)
        {
            int loyaltyPoints = 0;

            if (type == 1)
            {
                loyaltyPoints = 2;
            }
            else
            {
                loyaltyPoints = 1;
            }
            return loyaltyPoints;
        }

    }
}
