using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class Program
    {
        public double TotalGroceryBill { get; set; }

        public Program()
        {
            TotalGroceryBill = 0;
        }
        static void Main(string[] args)
        {
            Program programToRun = new Program();
            CheckoutItem itemToScan = new CheckoutItem();
            programToRun.ScanItem(itemToScan);
        }

        public void ScanItem(CheckoutItem itemToScan)
        {
            TotalGroceryBill = TotalGroceryBill + itemToScan.CurrentPrice;
        }
    }

    public class CheckoutItem
    {
        public string ItemName { get; set; }
        public double CurrentPrice { get; set; }
    }


}
