using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class CLinearCongruentGenerator
    {
        protected int n;
        const int defaultN = 1;

        protected long Xstart { get; } = 1;
        protected long m { get; } = 1L << 48; //281 474 976 710 656 
        protected long a { get; } = 44485709377909;
        protected long c { get; } = 0;
        protected long T { get; set; } = 1L << 46; //70 368 744 177 664

        protected long Xk;

        protected long[] startPoints;
        protected long curPoint;

        public CLinearCongruentGenerator()
        {
            n = defaultN;
            Xk = Xstart;
            generateStartPoints();
            T /= n;
        }

        public CLinearCongruentGenerator(int n)
        {
            this.n = n;
            Xk = Xstart;
            generateStartPoints();
            T /= n;
        }

        private void generateStartPoints()
        {
            long internalPeriod = T / n;
            startPoints = new long[n];
            startPoints[0] = Xk;
            List<Action> actionsList = new List<Action>();
            for (int i = 1; i < n; ++i)
            {
                int v = i;
                actionsList.Add(() => {
                    startPoints[v] = GetSkipResult(v * internalPeriod);
                });
            }
            Parallel.ForEach(actionsList, (action => action.DynamicInvoke()));
            curPoint = 0;
        }

        protected long GetSkipResult(long k)
        {
            return ((powWithModulo(a, k, m) * Xk % m) % m);
        }

        protected double SkipNext(long k)
        {
            Xk = (powWithModulo(a, k, m) * Xk % m) % m;
            return Xk / (double)m;
        }

        public long StartState
        {
            get
            {
                if (curPoint >= n)
                    throw new Exception("The maximum number of random values from current linear generator has been reached.");
                return startPoints[curPoint++];
            }
        }

        public long Modulo
        {
            get { return m; }
        }

        public long A
        {
            get { return a; }
        }

        public long C
        {
            get { return c; }
        }

        public long Period
        {
            get { return T; }
        }

        protected long multWithModulo(long a, long b, long m)
        {
            if (b == 0 || a == 0)
                return 0;
            if (b == 1)
                return a;
            if ((b & 1) != 0)
                return (multWithModulo(a, b - 1, m) % m + a % m) % m;
            return ((multWithModulo(a, b >> 1, m) % m) * 2) % m;
        }

        protected long powWithModulo(long a, long b, long m)
        {
            if (a == 0)
                return 0;
            if (b == 0)
                return 1;
            if ((b & 1) != 0)
                return multWithModulo(powWithModulo(a, b - 1, m), a, m) % m;
            long partial = powWithModulo(a, b >> 1, m);
            return multWithModulo(partial, partial, m) % m;
        }
    }
}
