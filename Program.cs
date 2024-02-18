using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

using static System.Math;

namespace Matrix;

internal static class Extensions
{
	public static void SwapRows<T>(this T[,] matrix, Int32 y1, Int32 y2)
	{
		for (Int32 x = 0; x < matrix.GetLength(1); x++)
		{
			(matrix[y2, x], matrix[y1, x]) = (matrix[y1, x], matrix[y2, x]);
		}
	}
	public static Int32 PivotIndex(this Double[,] matrix, Int32 x, Int32 y)
	{
		Int32 minIndex = y;
		Double minValue = Abs(matrix[minIndex, x]);
		for (Int32 index = y + 1; index < matrix.GetLength(0); index++)
		{
			Double value = Abs(matrix[index, x]);
			if (value != 0 && value < minValue)
			{
				minIndex = index;
				minValue = value;
			}
		}
		return minIndex;
	}
}

internal class Program
{
	struct Point2D(Int32 x, Int32 y)
	{
		public Int32 X { get; set; } = x;
		public Int32 Y { get; set; } = y;
	}
	static String ReadInput()
	{
		StringBuilder builder = new();
		while (true)
		{
			Int32 code = Console.Read();
			if (code < 0) break;
			Char current = (Char) code;
			if (current == ';') break;
			builder.Append(current);
		}
		return builder.ToString();
	}
	static Double[][] ParseGraph(String input)
	{
		return input.Split(',').Select((row, y) =>
		{
			return row.Split(' ').Select((cell, x) =>
			{
				String value = cell.Trim();
				try
				{
					return Convert.ToDouble(value);
				}
				catch
				{
					throw new FormatException($"Unable to convert value '{value}' of cell [{x}, {y}] to number");
				}
			}).ToArray();
		}).ToArray();
	}
	static Point2D ValidateGraph(Double[][] graph)
	{
		if (graph.Length == 0) throw new ArgumentException($"Graph must have at least 1 row");
		Int32 width = graph[0].Length;
		for (Int32 y = 1; y < graph.Length; y++)
		{
			if (graph[y].Length != width) throw new ArgumentException($"Graph must be rectangular");
		}
		return new Point2D(width, graph.Length);
	}
	static Double[,] BuildMatix(Double[][] graph)
	{
		Point2D size = ValidateGraph(graph);
		Double[,] matix = new Double[size.Y, size.X];
		for (Int32 y = 0; y < size.Y; y++)
		{
			for (Int32 x = 0; x < size.X; x++)
			{
				matix[y, x] = graph[y][x];
			}
		}
		return matix;
	}
	static public void GaussianElimination(in Double[,] matrix)
	{
		Point2D size = new(matrix.GetLength(1), matrix.GetLength(0));
		for (Int32 i = 0; i < Min(size.X, size.Y); i++)
		{
			Point2D pivot = new(i, i);
			matrix.SwapRows(pivot.Y, matrix.PivotIndex(pivot.X, pivot.Y));
			Double target = matrix[pivot.Y, pivot.X];
			if (target == 0) continue;
			for (Int32 y = pivot.Y + 1; y < size.Y; y++)
			{
				Double factor = matrix[y, pivot.X] / target;
				for (Int32 x = 0; x < size.X; x++)
				{
					matrix[y, x] -= matrix[pivot.Y, x] * factor;
				}
			}
		}
	}
	static Double[][] BuildGraph(Double[,] matrix)
	{
		Point2D size = new(matrix.GetLength(1), matrix.GetLength(0));
		Double[][] graph = new Double[size.Y][];
		for (Int32 y = 0; y < size.Y; y++)
		{
			graph[y] = new Double[size.X];
			for (Int32 x = 0; x < size.X; x++)
			{
				graph[y][x] = matrix[y, x];
			}
		}
		return graph;
	}
	static String Stringify(Double[][] graph)
	{
		return $"{String.Join(",\n", graph.Select(row => String.Join(" ", row)))};";
	}
	static void Main()
	{
		ConsoleColor colorDefault = Console.ForegroundColor;
		ConsoleColor colorHighlight = ConsoleColor.Cyan;
		ConsoleColor colorAlert = ConsoleColor.Red;

		Console.ForegroundColor = colorHighlight;
		Console.Write($"Hi, user!\n");
		Console.Write("Enter matrix like this to invoke the algorithm:\n1 0 4 2,\n1 2 6 2,\n2 0 8 8,\n2 1 9 4;\n\n");
		Console.ForegroundColor = colorDefault;

		while (true)
		{
			try
			{
				String input = ReadInput();
				Double[][] graph = ParseGraph(input);
				Double[,] matrix = BuildMatix(graph);
				GaussianElimination(matrix);
				Console.ForegroundColor = colorHighlight;
				Console.Write($"\nResult is:\n{Stringify(BuildGraph(matrix))}\n\n");
				Console.ForegroundColor = colorDefault;
			}
			catch (Exception exception)
			{
				Console.ForegroundColor = colorHighlight;
				Console.Write($"\nAttemp elaminated with reason:");
				Console.ForegroundColor = colorAlert;
				Console.Write($" {exception.Message}\n\n");
				Console.ForegroundColor = colorDefault;
			}
		}
	}
}
