using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("=== ЧАСТЬ 1. ПРОВЕРКА ПРОГРАММЫ ===\n");

        double[] xs1 = { 1, 2, 4 };
        double[] ys1 = { 3.5, 4.6, 6.7 };

        double[] l3 = Basis(xs1, 3);
        double sumL = 0;
        Console.WriteLine("Базисные значения в точке x = 3:");
        for (int i = 0; i < l3.Length; i++)
        {
            Console.WriteLine($"  l_{i}(3) = {l3[i]:F6}");
            sumL += l3[i];
        }
        Console.WriteLine($"Сумма всех базисов: {sumL:F6}");
        Console.WriteLine($"Итоговое P(3) = {Lagrange(xs1, ys1, 3):F4}\n");

        Console.WriteLine("Проверка совпадения в узлах (должны быть нули):");
        for (int i = 0; i < xs1.Length; i++)
        {
            double diff = Lagrange(xs1, ys1, xs1[i]) - ys1[i];
            Console.WriteLine($"  x = {xs1[i]}: разница = {diff}");
        }
        Console.WriteLine();

        double[] xsPara = { 2, 3, 5 };
        double[] ysPara = { 4, 9, 25 };
        double[] testPointsPara = { 2.5, 3.8, 4.4 };

        Console.WriteLine("Проверка на параболе y = x^2:");
        foreach (double x in testPointsPara)
        {
            double p = Lagrange(xsPara, ysPara, x);
            double exact = x * x;
            Console.WriteLine($"  x = {x:F1}: программа дала {p:F6}, точное x^2 = {exact:F6}, разница = {Math.Abs(p - exact):E2}");
        }
        Console.WriteLine();

        Console.WriteLine("=== ЧАСТЬ 2. ЭКСПЕРИМЕНТЫ ===\n");

        double[] xs2 = { 1, 2, 4, 6 };
        double[] ys2 = { 3.5, 4.6, 6.7, 9.8 };
        double[] comparePoints = { 1.5, 2.5, 3.0, 3.5, 5.0 };

        Console.WriteLine("Сравнение расчётов по 3 точкам и по 4 точкам:");
        foreach (double x in comparePoints)
        {
            double p3 = Lagrange(xs1, ys1, x);
            double p4 = Lagrange(xs2, ys2, x);
            Console.WriteLine($"  x = {x:F1}: по 3 точкам = {p3:F4} | по 4 точкам = {p4:F4}");
        }
        Console.WriteLine();

        double[] sinXs3 = { 0.0, 0.5, 1.0 };
        double[] sinYs3 = new double[sinXs3.Length];
        for (int i = 0; i < sinXs3.Length; i++) sinYs3[i] = Math.Sin(sinXs3[i]);

        double[] sinXs4 = { 0.0, 0.5, 1.0, 1.5 };
        double[] sinYs4 = new double[sinXs4.Length];
        for (int i = 0; i < sinXs4.Length; i++) sinYs4[i] = Math.Sin(sinXs4[i]);

        Console.WriteLine("Насколько сильно программа ошибается по сравнению с настоящим sin(x):");
        Console.WriteLine("   x   | Ошибка (3 точки) | Ошибка (4 точки)");
        Console.WriteLine("-------+------------------+-----------------");

        double maxErr3 = 0;
        double maxErr4 = 0;

        for (int k = 0; k <= 15; k++)
        {
            double x = Math.Round(k * 0.1, 1);
            double exactSin = Math.Sin(x);
            double err3 = Math.Abs(Lagrange(sinXs3, sinYs3, x) - exactSin);
            double err4 = Math.Abs(Lagrange(sinXs4, sinYs4, x) - exactSin);

            if (x <= 1.0)
            {
                if (err3 > maxErr3) maxErr3 = err3;
                if (err4 > maxErr4) maxErr4 = err4;
            }

            string note = x > 1.0 ? " (за пределами старой таблицы)" : "";
            Console.WriteLine($"  {x:F1}  |     {err3:F5}      |     {err4:F5}{note}");
        }

        Console.WriteLine($"\nСамая большая ошибка на отрезке [0; 1] для 3 точек: {maxErr3:F5}");
        Console.WriteLine($"Самая большая ошибка на отрезке [0; 1] для 4 точек: {maxErr4:F5}");
    }

    static double[] Basis(double[] xs, double x)
    {
        double[] li = new double[xs.Length];
        for (int i = 0; i < xs.Length; i++)
        {
            li[i] = 1;
            for (int j = 0; j < xs.Length; j++)
                if (j != i)
                    li[i] *= (x - xs[j]) / (xs[i] - xs[j]);
        }
        return li;
    }

    static double Lagrange(double[] xs, double[] ys, double x)
    {
        double[] li = Basis(xs, x);
        double p = 0;
        for (int i = 0; i < xs.Length; i++) p += ys[i] * li[i];
        return p;
    }
}
