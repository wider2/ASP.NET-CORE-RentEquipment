using System;

namespace Bondora.Api.Data
{
    public class CustomerException : Exception
    {
        public CustomerException()
        {
        }

        public CustomerException(string name)
            : base(String.Format("Invalid Customer: {0}", name))
        {
        }

    }

}
