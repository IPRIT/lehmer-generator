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

            CLinearCongruentGenerator linearGenerator = new CLinearCongruentGenerator(1000);

            CBasicRandomValue basicGenerator1 = new CBasicRandomValue(linearGenerator);
            for (int i = 0; i < 10000; ++i)
                basicGenerator1.next();

            CBasicRandomValue basicGenerator2 = new CBasicRandomValue(linearGenerator);
            for (int i = 0; i < 10000; ++i)
                basicGenerator2.next();

            CBasicRandomValue basicGenerator3 = new CBasicRandomValue(linearGenerator);
            for (int i = 0; i < 10000; ++i)
                basicGenerator3.next();

            Console.WriteLine("--");

            CLinearCongruentGenerator linearGenerator2 = new CLinearCongruentGenerator(3);

            CBasicRandomValue basicGenerator12 = new CBasicRandomValue(linearGenerator2);
            for (int i = 0; i < 10000; ++i)
                basicGenerator12.next();

            CBasicRandomValue basicGenerator22 = new CBasicRandomValue(linearGenerator2);
            for (int i = 0; i < 10000; ++i)
                basicGenerator22.next();

            CBasicRandomValue basicGenerator32 = new CBasicRandomValue(linearGenerator2);
            for (int i = 0; i < 10000; ++i)
                basicGenerator32.next();

            Console.WriteLine("--");

            CLinearCongruentGenerator linearGenerator3 = new CLinearCongruentGenerator(3);

            CBasicRandomValue basicGenerator13 = new CBasicRandomValue(linearGenerator3);
            for (int i = 0; i < 10000; ++i)
                basicGenerator13.next();

            CBasicRandomValue basicGenerator23 = new CBasicRandomValue(linearGenerator3);
            for (int i = 0; i < 10000; ++i)
                basicGenerator23.next();

            CBasicRandomValue basicGenerator33 = new CBasicRandomValue(linearGenerator3);
            for (int i = 0; i < 10000; ++i)
                basicGenerator33.next();
        }
    }
}
