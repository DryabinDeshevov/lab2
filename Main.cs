using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab1_OOP
{
    // Класс "Размер"
    public class Dimension
    {
        public double Value { get; set; }

        public Dimension(double value)
        {
            Value = value;
        }

        public virtual double Calculate() => Value;
    }

    // Класс "Связный размер"
    public class LinkedDimension : Dimension
    {
        private readonly Dimension _left;
        private readonly Dimension _right;
        private readonly char _operation;

        public LinkedDimension(Dimension left, Dimension right, char operation) 
            : base(0)
        {
            _left = left;
            _right = right;
            _operation = operation;
        }

        public override double Calculate()
        {
            double a = _left.Calculate();
            double b = _right.Calculate();

            return _operation switch
            {
                '+' => a + b,
                '-' => a - b,
                '*' => a * b,
                '/' => a / b,
                '^' => Math.Pow(a, b),
                _ => throw new Exception("Неизвестная операция: " + _operation)
            };
        }
    }

    // Класс "Размерная цепь"
    public class DimensionChain
    {
        private readonly List<Dimension> _dimensions = new List<Dimension>();

        public void Add(Dimension dimension) => _dimensions.Add(dimension);

        public double Calculate()
        {
            double result = 0;
            foreach (var dim in _dimensions)
                result += dim.Calculate(); // пример: суммируем все элементы
            return result;
        }
    }

    // Программа
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите выражение:");
            string input = Console.ReadLine();

            try
            {
                Dimension expr = ExpressionParser.Parse(input);
                Console.WriteLine("Результат: " + expr.Calculate().ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }

    // Парсер выражений -> дерево объектов Dimension/LinkedDimension
    public static class ExpressionParser
    {
        public static Dimension Parse(string expression)
        {
            int index = 0;
            return ParseExpression(expression.Replace(" ", ""), ref index);
        }

        private static Dimension ParseExpression(string expr, ref int index)
        {
            Dimension result = ParseTerm(expr, ref index);

            while (index < expr.Length && (expr[index] == '+' || expr[index] == '-'))
            {
                char op = expr[index++];
                Dimension right = ParseTerm(expr, ref index);
                result = new LinkedDimension(result, right, op);
            }

            return result;
        }

        private static Dimension ParseTerm(string expr, ref int index)
        {
            Dimension result = ParseFactor(expr, ref index);

            while (index < expr.Length && (expr[index] == '*' || expr[index] == '/'))
            {
                char op = expr[index++];
                Dimension right = ParseFactor(expr, ref index);
                result = new LinkedDimension(result, right, op);
            }

            return result;
        }

        private static Dimension ParseFactor(string expr, ref int index)
        {
            Dimension result = ParsePrimary(expr, ref index);

            while (index < expr.Length && expr[index] == '^')
            {
                index++;
                Dimension right = ParsePrimary(expr, ref index);
                result = new LinkedDimension(result, right, '^');
            }

            return result;
        }
		private static Dimension ParsePrimary(string expr, ref int index)
        {
            if (expr[index] == '(')
            {
                index++;
                Dimension result = ParseExpression(expr, ref index);
                if (index >= expr.Length || expr[index] != ')')
                    throw new Exception("Не хватает закрывающей скобки");
                index++;
                return result;
            }

            if (char.IsDigit(expr[index]) || expr[index] == '.')
                return ParseNumber(expr, ref index);

            throw new Exception("Неожиданный символ: " + expr[index]);
        }

        private static Dimension ParseNumber(string expr, ref int index)
        {
            int start = index;
            while (index < expr.Length && (char.IsDigit(expr[index]) || expr[index] == '.'))
                index++;

            double value = double.Parse(expr.Substring(start, index - start), CultureInfo.InvariantCulture);
            return new Dimension(value);
        }
    }
}