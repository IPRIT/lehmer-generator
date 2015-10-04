# Lehmer Linear Congruent Pseudorandom Number Generator CRAY

The current generator named as *CRAY* allows to generate numbers included in the interval from 0 to 1 with period 2^46 and binary modulo exponentiation and multiplication.
The selected values for the Lehmer generator:

```a = 44 485 709 909 377, c = 0, m = 2^48, X0 = 1.```

#### You can create random variables with a single basic interval.
```c#
CBasicRandomValue basicGenerator = new CBasicRandomValue();
for (int i = 0; i < 100000; ++i)
    basicGenerator.next();
```

#### You can also create random variables using a factory, which divides produced sequence of numbers into several parts and gives a better result.

```c#
int seed = 1234;
CLinearCongruentGenerator linearGenerator = new CLinearCongruentGenerator(seed);

CBasicRandomValue basicGeneratorFirst = new CBasicRandomValue(linearGenerator);
for (int i = 0; i < 1 << 20; ++i)
    basicGeneratorFirst.next();
    
CBasicRandomValue basicGeneratorSecond = new CBasicRandomValue(linearGenerator);
for (int i = 0; i < 1 << 20; ++i)
    basicGeneratorSecond.next();
```
