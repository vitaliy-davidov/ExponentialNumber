using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExponentialNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            Exponent exponent2 = Exponent.Parse("9.99E3"); //000005.43E10 == 0.0000543E15
            Exponent exponent3 = Exponent.Parse("0.01E3");//145000.00E10 == 1.4500000E15
            Exponent exponent4 = Exponent.Parse("-0.01E3");//145000.00E10 == 1.4500000E15
            Console.WriteLine(exponent4);
                                                            //145005.43E10 == 1.4500543E15 
            Console.WriteLine(exponent2);
            Console.WriteLine(exponent3);
            Console.WriteLine(exponent2 + exponent3);
        }
    }
}
