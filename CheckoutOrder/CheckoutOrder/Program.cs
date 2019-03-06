using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program programToRun = new Program();
            CheckoutItem itemToScan = new CheckoutItem();
            programToRun.ScanItem(itemToScan);
        }

        public double ScanItem(CheckoutItem itemToScan)
        {
            return itemToScan.CurrentPrice;
        }
    }

    public class CheckoutItem
    {
        public string ItemName { get; set; }
        public double CurrentPrice { get; set; }
    }


}
