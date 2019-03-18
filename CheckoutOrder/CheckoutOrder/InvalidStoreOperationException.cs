using System;

namespace CheckoutOrder
{
    public class InvalidStoreOperationException : Exception
    {
        public InvalidStoreOperationException(string message) : base(message)
        {
        }
    }
}
