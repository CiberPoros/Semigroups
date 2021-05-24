using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Semigroups
{
    [Serializable]
    internal struct Matrix
    {
        public static bool WriteOperationsToConsole { get; set; } = false;

        [JsonProperty]
        private readonly int[,] _value;

        private int _height;
        private int _width;

        [JsonIgnore]
        public int Height => _height;
        [JsonIgnore]
        public int Width => _width;

        public Matrix(int[,] value)
        {
            _height = value.GetLength(0);
            _width = value.GetLength(1);

            _value = new int[_height, _width];
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _value[i, j] = FieldOrder is null 
                        ? value[i, j]
                        : ((value[i, j] % FieldOrder.Value) + FieldOrder.Value) % FieldOrder.Value;
                }
            }
        }

        public static int? FieldOrder { get; set; } = 2;

        public int this[int index1, int index2] => _value[index1, index2];

        public static void WriteInfoAboutOperation(List<Matrix> matrices, List<char> operations, List<bool> openScobe = null, List<bool> closeScobe = null)
        {
            var mid = (int)(matrices.Average(x => x.Height) / 2);
            var height = matrices.Max(x => x.Height);
            var builders = new StringBuilder[height];
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i] = new StringBuilder();
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < matrices.Count; j++)
                {
                    AddMatrixRow(matrices[j], i, j);

                    if (j == matrices.Count - 1)
                    {
                        break;
                    }

                    builders[i].Append(' ');
                    builders[i].Append(i == mid ? operations[j] : ' ');
                    builders[i].Append(' ');
                }
            }   

            foreach (var builder in builders)
            {
                Console.WriteLine(builder.ToString());
            }
            Console.WriteLine();

            void AddMatrixRow(Matrix matrix, int rowIndex, int matrixIndex)
            {
                if (openScobe is not null && openScobe[matrixIndex])
                    builders[rowIndex].Append('(');

                builders[rowIndex].Append(rowIndex < matrix.Height ? '|' : ' ');
                for (int j = 0; j < matrix.Width; j++)
                {
                    builders[rowIndex].Append(rowIndex < matrix.Height ? matrix[rowIndex, j] : ' ');

                    if (j == matrix.Width - 1)
                    {
                        break;
                    }

                    builders[rowIndex].Append(' ');
                }
                builders[rowIndex].Append(rowIndex < matrix.Height ? '|' : ' ');

                if (closeScobe is not null && closeScobe[matrixIndex])
                    builders[rowIndex].Append(')');
            }
        }

        public bool IsEmptyMatrix()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (this[i, j] == 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Matrix GetSummNeutral() => new(new int[Height, Width]);
        public Matrix GetMultiplicationNeutral()
        {
            if (Height != Width)
            {
                throw new Exception("Unit matrix exists only for square matrix.");
            }

            var array = new int[Height, Width];

            for (int i = 0; i < Height; i++)
            {
                array[i, i] = 1;
            }

            return new Matrix(array);
        }

        public Matrix GetSummInverce() => -this;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('(');
            for (int i = 0; i < Height; i++)
            {
                if (i != 0)
                {
                    stringBuilder.Append(' ');
                }

                for (int j = 0; j < Width; j++)
                {
                    stringBuilder.Append(this[i, j]);

                    if (j < Width - 1)
                    {
                        stringBuilder.Append(' ');
                    }            
                }

                if (i < Height - 1)
                {
                    stringBuilder.Append(';');
                }
            }

            stringBuilder.Append(')');

            return stringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Matrix matrix))
            {
                return false;
            }

            if (Width != matrix.Width || Height != matrix.Height)
            {
                return false;
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (this[i, j] != matrix[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var res = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    res ^= this[i, j].GetHashCode();
                }
            }

            return res;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            _height = _value.GetLength(0);
            _width = _value.GetLength(1);
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            int height = Math.Max(left.Height, right.Height), width = Math.Max(left.Width, right.Width);
            var array = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < left.Height && j < left.Width)
                    {
                        array[i, j] += left[i, j];
                    }

                    if (i < right.Height && j < right.Width)
                    {
                        array[i, j] += right[i, j];
                    }
                }
            }

            var result = new Matrix(array);
            WriteToConsoleOperation(left, right, result, '+');
            return result;
        }

        public static Matrix operator -(Matrix single)
        {
            var array = new int[single.Height, single.Width];

            for (int i = 0; i < single.Height; i++)
            {
                for (int j = 0; j < single.Width; j++)
                {
                    array[i, j] = -single[i, j];
                }
            }

            var result = new Matrix(array);
            WriteToConsoleOperation(single, result, '-');
            return result;
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            int height = Math.Max(left.Height, right.Height), width = Math.Max(left.Width, right.Width);
            var array = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < left.Height && j < left.Width)
                    {
                        array[i, j] += left[i, j];
                    }

                    if (i < right.Height && j < right.Width)
                    {
                        array[i, j] -= right[i, j];
                    }
                }
            }

            var result = new Matrix(array);
            WriteToConsoleOperation(left, right, result, '-');
            return result;
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Width != right.Height)
            {
                throw new Exception($"{nameof(left.Width)} must be equal {nameof(right.Height)}.");
            }

            var array = new int[left.Height, right.Width];

            for (int i = 0; i < left.Height; i++)
            {
                for (int j = 0; j < right.Width; j++)
                {
                    for (int k = 0; k < left.Width; k++)
                    {
                        array[i, j] += left[i, k] * right[k, j];
                    }
                }
            }

            var result = new Matrix(array);
            WriteToConsoleOperation(left, right, result, '*');
            return result;
        }

        private static void WriteToConsoleOperation(Matrix single, Matrix result, char operation)
        {
            if (!WriteOperationsToConsole)
                return;

            var center = single.Height / 2;

            var height = Math.Max(single.Height, result.Height);
            var width = Math.Max(single.Width, result.Width);
            var builders = new StringBuilder[height];
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i] = new StringBuilder();
            }

            for (int i = 0; i < height; i++)
            {
                builders[i].Append(i == center ? operation : ' ');
                builders[i].Append(' ');

                AddMatrixRow(single, i);

                builders[i].Append(' ');
                builders[i].Append(i == center ? '=' : ' ');
                builders[i].Append(' ');

                AddMatrixRow(result, i);
            }

            foreach (var builder in builders)
            {
                Console.WriteLine(builder.ToString());
            }
            Console.WriteLine();

            void AddMatrixRow(Matrix matrix, int rowIndex)
            {
                builders[rowIndex].Append(rowIndex < matrix.Height ? '|' : ' ');
                for (int j = 0; j < matrix.Width; j++)
                {
                    builders[rowIndex].Append(rowIndex < matrix.Height ? matrix[rowIndex, j] : ' ');

                    if (j == matrix.Width - 1)
                    {
                        break;
                    }

                    builders[rowIndex].Append(' ');
                }
                builders[rowIndex].Append(rowIndex < matrix.Height ? '|' : ' ');
            }
        }

        private static void WriteToConsoleOperation(Matrix left, Matrix right, Matrix result, char operation)
        {
            if (!WriteOperationsToConsole)
                return;

            var center = Math.Max(left.Height, right.Height) / 2;

            var height = Math.Max(Math.Max(left.Height, right.Height), result.Height);
            var width = Math.Max(Math.Max(left.Width, right.Width), result.Width);
            var builders = new StringBuilder[height];
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i] = new StringBuilder();
            }

            for (int i = 0; i < height; i++)
            {
                AddMatrixRow(left, i);

                builders[i].Append(' ');
                builders[i].Append(i == center ? operation : ' ');
                builders[i].Append(' ');

                AddMatrixRow(right, i);

                builders[i].Append(' ');
                builders[i].Append(i == center ? '=' : ' ');
                builders[i].Append(' ');

                AddMatrixRow(result, i);
            }

            foreach (var builder in builders)
            {
                Console.WriteLine(builder.ToString());
            }
            Console.WriteLine();

            void AddMatrixRow(Matrix matrix, int rowIndex)
            {
                builders[rowIndex].Append(rowIndex < matrix.Height ? '|' : ' ');
                for (int j = 0; j < matrix.Width; j++)
                {
                    builders[rowIndex].Append(rowIndex < matrix.Height ? matrix[rowIndex, j] : ' ');

                    if (j == matrix.Width - 1)
                    {
                        break;
                    }

                    builders[rowIndex].Append(' ');
                }
                builders[rowIndex].Append(rowIndex < matrix.Height ? '|' : ' ');
            }
        }
    }
}
