using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class InvalidStoreOperationException : Exception
    {
        public InvalidStoreOperationException(string message) : base(message)
        {
        }
    }
}
