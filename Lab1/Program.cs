using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            //CBasicRandomValue random1 = new CBasicRandomValue();
            //random1.computePeriodAsParallel(70368714);

            //random1.checkFirstApproval();
            //CBasicRandomValue rndSource = new CBasicRandomValue();
            //random1.checkSecondApproval(rndSource);

            CLinearCongruentGenerator linearGenerator = new CLinearCongruentGenerator(300);

            CBasicRandomValue basicGenerator = new CBasicRandomValue(linearGenerator);
            for (int j = 0; j < 1e6; ++j)
                basicGenerator.next();

            Console.WriteLine("--");
        }
    }
}
