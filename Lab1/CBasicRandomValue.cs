using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class CBasicRandomValue
    {
        protected long X0;
        protected long m; //281 474 976 710 656 
        protected long a;
        protected long c;
        protected long T; //70 368 744 177 664

        protected long Xk;
        protected long curIndexState;

        public CBasicRandomValue(long X0 = 1, long m = 1L << 48, long a = 44485709377909, long c = 0, long T = 1L << 46)
        {
            this.X0 = X0;
            this.m = m;
            this.a = a;
            this.c = c;
            this.T = T;
            Xk = X0;
            curIndexState = 0;
        }

        public CBasicRandomValue(CLinearCongruentGenerator source)
        {
            try {
                X0 = source.StartState;
                m = source.Modulo;
                a = source.A;
                c = source.C;
                T = source.Period;
                Xk = X0;
                curIndexState = 0;
            } catch (Exception err) {
                Console.WriteLine(err.Message);
            }
        }

        public double next()
        {
            if (capacity() <= 0)
                throw new Exception("Capacity has reached the limit.");
            Xk = (multWithModulo(a, Xk, m) + c % m) % m;
            ++curIndexState;
            return Xk / (double)m;
        }

        public long getState() => Xk;

        public long capacity() => T - curIndexState;

        public void reset()
        {
            Xk = X0;
            curIndexState = 0;
        }

        protected long gcd(long a, long b) => b == 0 ? a : gcd(b, a % b);

        public long computeT()
        {
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();

            long T = 0;
            long Xk = (multWithModulo(a, X0, m) + c % m) % m;
            while (Xk != X0)
            {
                Xk = (multWithModulo(a, Xk, m) + c % m) % m;
                ++T;
                if (T % (1 << 20) == 0)
                {
                    Console.WriteLine($"[{myStopwatch.Elapsed}]: {T}");
                }
            }
            myStopwatch.Stop();
            return T;
        }

        protected long multWithModulo(long a, long b, long m)
        {
            if (b == 0 || a == 0)
                return 0;
            if (b == 1)
                return a;
            if ((b & 1) != 0)
                return (multWithModulo(a, b - 1, m) % m + a % m) % m;
            return ((multWithModulo(a, b / 2, m) % m) * 2) % m;
        }

        protected long powWithModulo(long a, long b, long m)
        {
            if (a == 0)
                return 0; 
            if (b == 0)
                return 1;
            if ((b & 1) != 0)
                return multWithModulo(powWithModulo(a, b - 1, m), a, m) % m;
            long partial = powWithModulo(a, b / 2, m);
            return multWithModulo(partial, partial, m) % m;
        }

        public double Skip(long k)
        {
            if (capacity() - k <= 0)
                throw new Exception("Capacity has reached the limit.");
            Xk = (powWithModulo(a, k, m) * Xk % m) % m;
            curIndexState += k;
            return Xk / (double)m;
        }

        int readyAsyncResults,
            curStartParallelIndex,
            blockSize = 30;
        bool periodFound = false;

        public void computePeriodAsParallel(int start)
        {
            if (periodFound)
                return;
            List<Action> actionsList = new List<Action>();
            readyAsyncResults = 0;
            curStartParallelIndex = start;
            for (long i = start; i < start + blockSize; ++i)
            {
                /* замкнем переменную в контекст Action */
                long v = i * (long)1e6;
                if (v > T)
                    break;
                actionsList.Add(() => findPeriodAsParallel(v));
            }
            Parallel.ForEach(actionsList, (o => o()));
        }

        protected void findPeriodAsParallel(long k)
        {
            CBasicRandomValue random = new CBasicRandomValue();
            random.Skip(k - 1);

            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();

            long T = k;
            long Xk = (multWithModulo(a, random.getState(), m) + c % m) % m;
            while (Xk != X0 && T <= k + 1000000)
            {
                Xk = (multWithModulo(a, Xk, m) + c % m) % m;
                ++T;
                if (T % (1 << 20) == 0)
                {
                    Console.WriteLine($"[{k}] [{myStopwatch.Elapsed}]: {T}");
                }
            }

            if (Xk == X0) {
                StreamWriter stream = new StreamWriter($@".\period_{k}.txt", false, Encoding.UTF8);
                stream.WriteLine($"Period = {T}");
                stream.Close();
                Console.WriteLine($"[{k}]: Found!");
                periodFound = true;
            } else {
                Console.WriteLine($"[{k} - {T}] [{DateTime.Now} {myStopwatch.Elapsed}]: Not found.");
            }

            readyAsyncResults++;
            if (readyAsyncResults >= blockSize)
                computePeriodAsParallel(curStartParallelIndex + blockSize);
            
            myStopwatch.Stop();
        }

        public void checkFirstApproval() => Console.WriteLine(
            gcd(X0, m) == 1 ?
            "1) Первое условие для достижения максимального периода выполнено." :
            "Первое условие не выполнено. Максимальный период не достигается."
        );


        CBasicRandomValue otherRandomValueSource;
        Dictionary<long, long> fact = new Dictionary<long, long>();

        public void checkSecondApproval(CBasicRandomValue rndSource)
        {
            /**
             * otherRandomValueSource = rndSource;
             * numberFactorize(m);
             * foreach (var prime in fact)
             * {
             *     Console.WriteLine($"{prime.Key} {prime.Value}");
             * }
             * удостоверяемся, что факторизация модуля это 48 двоек (2^48).
             * => T должно равняться h(2^48) = 2^46, так как e >= 3.
             */
             
            // Проверяем, является ли число а первообразным по модулю m:
            // Исходя из условий первообразности, сразу отбрасываем первые 3 условия, так как e > 3.
            // Проверим 4ое условие.

            if (a % 8 == 3 || a % 8 == 5)
                /* 4ое условие выполняется => максимальный период достигается (T = 2^46)*/
                Console.WriteLine("2) Число а — первообразный элемент по модулю m.");
            else
                Console.WriteLine("Второе условие не выполнено.");
        }

        protected void numberFactorize(long number)
        {
            if (number != 1)
            {
                // проверяем, не простое ли число
                if (isPrime(number))
                {
                    if (fact.ContainsKey(number))
                        fact[number]++;
                    else
                        fact[number] = 1;
                } 
                else
                {
                    // если число достаточно маленькое, то его разлагаем простым перебором
                    if (number < 1000 * 1000)
                    {
                        long div = primeDivTrivial(number, 20000);
                        if (fact.ContainsKey(div))
                            fact[div]++;
                        else
                            fact[div] = 1;
                        numberFactorize(number / div);
                    }
                    else
                    {
                        // число большое, запускаем на нем алгоритмы факторизации
                        long div;
                        div = fermatMethod(number);
                        numberFactorize(div);
                        numberFactorize(number / div);
                    }
                }
            }
        }

        private long fermatMethod(long n)
        {
            long x = (long)Math.Floor(Math.Sqrt(n)), y = 0, r = x * x - y * y - n;
            while (true)
            {
                if (r == 0)
                    return x != y ? x - y : x + y;
                else
                {
                    if (r > 0)
                    {
                        r -= y + y + 1;
                        ++y;
                    }
                    else
                    {
                        r += x + x + 1;
                        ++x;
                    }
                }
            }
        }

        private bool isPrime(long number)
        {
            if (number == 2)
                return true;
            for (int i = 0; i < 100; i++)
            {
                otherRandomValueSource.next();
                long a = (otherRandomValueSource.getState() % (number - 2)) + 2;
                if (gcd(a, number) != 1)
                    return false;
                if (powWithModulo(a, number - 1, number) != 1)
                    return false;
            }
            return true;
        }

        private long primeDivTrivial(long n, long m)
        {
            // сначала проверяем тривиальные случаи
            if (n == 2 || n == 3)
                return 1;
            if (n < 2)
                return 0;
            if (n % 2 == 0)
                return 2;

            // генерируем простые от 3 до m
            List<long> primes = getPrimes(m);

            // делим на все простые
            foreach (long iter in primes)
            {
                long div = iter;
                if (div * div > n)
                    break;
                else 
                    if (n % div == 0)
                        return div;
            }

            if (n < m * m)
                return 1;
            return 0;
        }

        List<long> computedPrimes = new List<long>();

        private List<long> getPrimes(long m)
        {
            if (computedPrimes.Count != 0)
                return computedPrimes;

            List<long> pr = new List<long>();
            List<long> lp = new List<long>();
            for (int i = 0; i < m + 1; ++i)
                lp.Add(0);
            List<long> primes = new List<long>();

            for (int i = 2; i <= m; ++i)
            {
                if (lp[i] == 0)
                {
                    pr.Add(i);
                    lp[i] = i;
                }
                for (int j = 0; j < pr.Count && i * pr[j] <= m && lp[i] >= pr[j]; ++j)
                    lp[i * (int)pr[j]] = pr[j];
            }

            for (int i = 3; i < lp.Count; ++i)
                if (i == lp[i])
                    primes.Add(i);

            computedPrimes.Clear();
            computedPrimes.AddRange(primes);

            return primes;
        }
    }
}
