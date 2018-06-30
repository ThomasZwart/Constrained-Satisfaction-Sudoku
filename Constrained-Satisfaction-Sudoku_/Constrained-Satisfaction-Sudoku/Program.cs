using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constrained_Satisfaction_Sudoku
{
    class Program
    {
        static void Main()
        {
            int[,] input = ReadSudoku();
            Sudoku sudoku = new Sudoku(input);
            Console.WriteLine();

            Sudoku newSudoku = ChronologicalBacktracking(sudoku);
            newSudoku.WriteSudoku();


            Console.ReadLine();            
        }

        static private Sudoku ChronologicalBacktracking(Sudoku sudoku)
        {
            Stack<Sudoku> stack = new Stack<Sudoku>();
            stack.Push(sudoku);

            while (stack.Count != 0)
            {
                int count = sudoku.domainCounter;               

                Sudoku newSudoku = sudoku.Clone();

                if (newSudoku.FirstEmptyVariable() == null)
                {
                    break;
                }
                // Generate successor
                newSudoku.domainCounter = 0;
                Tuple<int, int> newVariable = newSudoku.FirstEmptyVariable();
                List<int> variableDomain = newSudoku.domains[newVariable.Item1, newVariable.Item2];
                newSudoku.grid[newVariable.Item1, newVariable.Item2] = variableDomain[count];
                stack.Push(newSudoku);

                sudoku.domainCounter++; // Domain counter goes up

                // Failed partial solution
                if (!newSudoku.IsPartialSolution(newVariable))
                {
                    stack.Pop();
                }

                // Backtrack step
                if (sudoku.domainCounter >= variableDomain.Count)
                {
                    sudoku = stack.Pop();
                }
            }

            return sudoku;
        }

        private static int[,] ReadSudoku()
        {
            try
            {
                // Get the length of the first row N, the sudoku size is N * N 
                char[] firstRowArray = Console.ReadLine().Trim().ToCharArray();

                int sudokuLength = firstRowArray.Length;
                int[,] sudoku = new int[sudokuLength, sudokuLength];
                string[] allRows = new string[sudokuLength];

                // Add the first row to the sudoku array
                for (int i = 0; i < firstRowArray.Length; i++)
                {
                    sudoku[0, i] = int.Parse(firstRowArray[i].ToString());
                }

                // Add the rest of the rows to the sudoku array
                for (int i = 1; i < sudokuLength; i++)
                {
                    string s = Console.ReadLine().Trim();
                    char[] rowCharArray = s.ToCharArray();

                    for (int j = 0; j < rowCharArray.Length; j++)
                    {
                        sudoku[i, j] = int.Parse(rowCharArray[j].ToString());
                    }
                }
                return sudoku;
            }
            catch (Exception e)
            {
                // Incorrect input
                Console.WriteLine(e.Message);
                //Thread.Sleep(3000); // Wait for user to read error
                Environment.Exit(0); // Exit console
                return null;
            }
        }
    }


}
