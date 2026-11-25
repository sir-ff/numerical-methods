using System;

class Program
{
    static (double[] x, int steps) Seidel(double[,] A, double[] b,
        double eps, int maxSteps = 100, bool printSteps = false)
    {
        int n = b.Length;
        double[] x = new double[n];
        for (int k = 1; k <= maxSteps; k++)
        {
            double d = 0;
            for (int i = 0; i < n; i++)
            {
                double old = x[i];
                double s = 0;
                for (int j = 0; j < n; j++)
                    if (j != i) s += A[i, j] * x[j];
                x[i] = (b[i] - s) / A[i, i];
                d = Math.Max(d, Math.Abs(x[i] - old));
            }
            if (printSteps)
            {
                string coords = string.Join(" ", Array.ConvertAll(x, val => $"{val,11:F5}"));
                Console.WriteLine($"{k,2}  {coords}  d = {d:E1}");
            }
            if (d < eps) return (x, k);
        }
        return (x, -1);
    }

    static (double[] x, int steps) SimpleIteration(double[,] A, double[] b,
        double eps, int maxSteps = 100)
    {
        int n = b.Length;
        double[] x = new double[n];
        double[] xNext = new double[n];

        for (int k = 1; k <= maxSteps; k++)
        {
            double d = 0;
            for (int i = 0; i < n; i++)
            {
                double s = 0;
                for (int j = 0; j < n; j++)
                    if (j != i) s += A[i, j] * x[j];
                xNext[i] = (b[i] - s) / A[i, i];
                d = Math.Max(d, Math.Abs(xNext[i] - x[i]));
            }
            Array.Copy(xNext, x, n);
            if (d < eps) return (x, k);
        }
        return (x, -1);
    }

    static void Main()
    {
        double eps = 1e-6;
        
        double[,] A1 = {
            { 10, 1, 2 },
            { 2, 10, 1 },
            { 1, 2, 10 }
        };
        double[] b1 = { 18, 25, 35 };

        Console.WriteLine("=== 1. Прогон рабочей системы (Зейдель) ===");
        var res1 = Seidel(A1, b1, eps, 100, printSteps: true);
        Console.WriteLine($"Ответ: ({res1.x[0]:F5}; {res1.x[1]:F5}; {res1.x[2]:F5}), шагов: {res1.steps}\n");

        Console.WriteLine("=== 2. Сравнение методов на трёх системах ===");
        double[,] A2 = {
            { 3, 1, 1 },
            { 1, 3, 1 },
            { 1, 1, 3 }
        };
        double[] b2 = { 8, 10, 12 };

        double[,] A3 = {
            { 3, 1 },
            { 1, 4 }
        };
        double[] b3 = { 5, 9 };

        var systems = new (string name, double[,] A, double[] b)[]
        {
            ("10x + y + 2z = 18; 2x + 10y + z = 25; x + 2y + 10z = 35", A1, b1),
            ("3x + y + z = 8; x + 3y + z = 10; x + y + 3z = 12", A2, b2),
            ("3x + y = 5; x + 4y = 9", A3, b3)
        };

        Console.WriteLine($"{"Система",-58} | {"Базовый",-8} | {"Зейдель",-8}");
        Console.WriteLine(new string('-', 80));
        foreach (var sys in systems)
        {
            int stepsBase = SimpleIteration(sys.A, sys.b, eps).steps;
            int stepsSeidel = Seidel(sys.A, sys.b, eps).steps;
            Console.WriteLine($"{sys.name,-58} | {stepsBase,-8} | {stepsSeidel,-8}");
        }
        Console.WriteLine();

        Console.WriteLine("=== 3. Ловушка (переставленные строки, лимит 10 шагов) ===");
        double[,] A_trap = {
            { 2, 10, 1 },
            { 10, 1, 2 },
            { 1, 2, 10 }
        };
        double[] b_trap = { 25, 18, 35 };
        var resTrap = Seidel(A_trap, b_trap, eps, maxSteps: 10, printSteps: true);
        Console.WriteLine($"Статус (шагов): {resTrap.steps} (метод разошёлся)\n");

        Console.WriteLine("=== 4. Цена точности (рабочая система) ===");
        double[] epsList = { 1e-2, 1e-4, 1e-6 };
        foreach (double e in epsList)
        {
            int sBase = SimpleIteration(A1, b1, e).steps;
            int sSeidel = Seidel(A1, b1, e).steps;
            Console.WriteLine($"eps = {e:E0} -> Базовый: {sBase} шагов, Зейдель: {sSeidel} шагов");
        }
    }
}
