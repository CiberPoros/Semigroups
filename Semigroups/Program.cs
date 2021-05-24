using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Semigroups
{
    internal class Program
    {
        private const string MatricesFileName = "matrices.txt";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var modulo = ReadModulo();
            var operation = ReadOperationType();

            Matrix.FieldOrder = modulo;

            var matrixesString = string.Empty;

            try
            {
                matrixesString = File.ReadAllText(MatricesFileName).Replace($"{Environment.NewLine}   ", ";");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл {MatricesFileName} не найден.");
                return;
            }
            catch
            {
                Console.WriteLine("Что-то пошло не так. Пожалуйста, попробуйте еще раз.");
                return;
            }

            var matrices = new List<Matrix>();
            try
            {
                matrices = JsonConvert.DeserializeObject<List<Matrix>>(matrixesString, new MatrixConverter());
            }
            catch
            {
                Console.WriteLine($"Ошибка формата данных в файле {MatricesFileName}. Пожалуйста, проверьте правильность введенных данных.");
                return;
            }


            var logic = new CoreLogic();

            Console.WriteLine("Программа начала вычисление...");
            Console.WriteLine();
            var result = operation switch
            {
                OperationType.SUMMATION => logic.MakeSemigroups(matrices, (x, y) => x + y),
                OperationType.MULTIPLICATION => logic.MakeSemigroups(matrices, (x, y) => x * y)
            };

            Console.WriteLine("Построенная полугруппа: ");
            Console.WriteLine();

            foreach (var matrix in result)
            {
                for (int i = 0; i < matrix.Height; i++)
                {
                    for (int j = 0; j < matrix.Width; j++)
                    {
                        Console.Write(matrix[i, j]);
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }

        private static int ReadModulo()
        {
            Console.WriteLine("Введите модуль: ");
            
            for (; ; )
            {
                var input = Console.ReadLine();

                if (!int.TryParse(input, out var modulo) || modulo <= 0)
                {
                    Console.WriteLine("Ожидалось положительное число. Повторите попытку...");
                    Console.WriteLine();
                    continue;
                }

                return modulo;
            }
        }

        private static OperationType ReadOperationType()
        {
            Console.WriteLine("Задайте операцию:");
            Console.WriteLine("1. Сложение матриц;");
            Console.WriteLine("2. Умножение матриц...");
            Console.WriteLine();

            for (; ; )
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        return OperationType.SUMMATION;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        return OperationType.MULTIPLICATION;
                }
            }
        }
    }
}
