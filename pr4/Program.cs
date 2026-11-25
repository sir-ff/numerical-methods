using System;

class Program
{
    static double[] JacobiStep(double[,] A, double[] b, double[] x)
    {
        int n = b.Length;
        double[] next = new double[n];
        for (int i = 0; i < n; i++)
        {
            double s = 0;
            for (int j = 0; j < n; j++)
                if (j != i) s += A[i, j] * x[j];
            next[i] = (b[i] - s) / A[i, i];
        }
        return next;
    }

    static (double[] x, int steps) Jacobi(double[,] A, double[] b,
        double eps, int maxSteps = 100, bool printSteps = true)
    {
        int n = b.Length;
        double[] x = new double[n];
        for (int k = 1; k <= maxSteps; k++)
        {
            double[] next = JacobiStep(A, b, x);
            double d = 0;
            for (int i = 0; i < n; i++)
                d = Math.Max(d, Math.Abs(next[i] - x[i]));
            x = next;

            if (printSteps)
            {
                string coords = string.Join(" ", Array.ConvertAll(x, val => $"{val,10:F5}"));
                Console.WriteLine($"{k,2}  {coords}  d = {d:E1}");
            }

            if (d < eps) return (x, k);
        }
        return (x, -1);
    }

    static void Main()
    {
        Console.WriteLine("=== 1. Прогон рабочей системы (eps = 1e-6) ===");
        double[,] A1 = {
            { 10, 1, 2 },
            { 2, 10, 1 },
            { 1, 2, 10 }
        };
        double[] b1 = { 18, 25, 35 };
        var (res1, steps1) = Jacobi(A1, b1, 1e-6);
        Console.WriteLine($"Ответ: ({res1[0]:F5}; {res1[1]:F5}; {res1[2]:F5}), шагов: {steps1}\n");

        Console.WriteLine("=== 2. Цена точности ===");
        double[] epsilons = { 1e-2, 1e-4, 1e-6 };
        foreach (double eps in epsilons)
        {
            var (_, steps) = Jacobi(A1, b1, eps, printSteps: false);
            Console.WriteLine($"Точность {eps:E0} -> итераций: {steps}");
        }
        Console.WriteLine();

        Console.WriteLine("=== 3. Разлёт системы (лимит 10 шагов) ===");
        double[,] A3 = {
            { 1, 3 },
            { 5, 1 }
        };
        double[] b3 = { 14, 14 };
        var (res3, steps3) = Jacobi(A3, b3, 1e-6, maxSteps: 10);
        Console.WriteLine($"Статус: {(steps3 == -1 ? "Разлёт (лимит исчерпан)" : "Сошлось")}\n");

        Console.WriteLine("=== 4. Своя система 3x3 (ожидается: 3; -2; 4) ===");
        double[,] A4 = {
            { 10,  2, -1 },
            {  3, 12,  2 },
            { -2,  1, 15 }
        };
        double[] b4 = { 22, -7, 52 };
        var (res4, steps4) = Jacobi(A4, b4, 1e-6);
        Console.WriteLine($"Ответ: ({res4[0]:F5}; {res4[1]:F5}; {res4[2]:F5}), шагов: {steps4}");

        double eq1 = A4[0, 0] * res4[0] + A4[0, 1] * res4[1] + A4[0, 2] * res4[2];
        double eq2 = A4[1, 0] * res4[0] + A4[1, 1] * res4[1] + A4[1, 2] * res4[2];
        double eq3 = A4[2, 0] * res4[0] + A4[2, 1] * res4[1] + A4[2, 2] * res4[2];
        Console.WriteLine($"Проверка подстановкой: {eq1:F5} = {b4[0]}; {eq2:F5} = {b4[1]}; {eq3:F5} = {b4[2]}");
    }
}

