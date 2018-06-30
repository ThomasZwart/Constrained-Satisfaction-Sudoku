using System;
using System.Collections.Generic;

namespace SudukoLocalSearch
{
    class Sudoku
    {
        public int[,] grid;
        public HashSet<int>[,] domains;
        public HashSet<Tuple<int, int>> fixedNumbers;
        public HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> constraintSet;

        private static readonly Random getRandom = new Random();

        public Sudoku(int[,] _sudoku)
        {
            grid = _sudoku;
            SetFixedNumbers();
            InstantiateDomains();
            InstantiateConstrainSet();
        }

        private void InstantiateConstrainSet()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (!fixedNumbers.Contains(new Tuple<int, int>(i, j)))
                    {
                        for (int row = 0; row < Length; row++)
                        {
                            for (int column = 0; column < Length; column++)
                            {
                                if (i == row)
                                {

                                }
                                if (j == column)
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        private void InstantiateDomains()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (!fixedNumbers.Contains(new Tuple<int, int>(i, j)))
                    {
                        domains[i, j] = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    }
                }
            }
        }

        public Sudoku Clone() // Clones the current sudoku
        {
            // The grid cloned
            Sudoku clone = new Sudoku(grid.Clone() as int[,])
            {
                fixedNumbers = new HashSet<Tuple<int, int>>(fixedNumbers)
            };
            return clone;
        }

        public int this[int index1, int index2] // Used to access the grid variable easily
        {
            get { return grid[index1, index2]; }

            set { grid[index1, index2] = value; }
        }

        public void SetFixedNumbers() // Sets all numbers that aren't blank as fixed numbers in the sudoku based on their indices                                                   
        {
            fixedNumbers = new HashSet<Tuple<int, int>>();
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        fixedNumbers.Add(new Tuple<int, int>(i, j)); // Tuple of (row, column) coordinates in the grid
                    }
                }
            }
        }


        public int Length // The length of the sudoku
        {
            get { return grid.GetLength(0); }
        }


        public void WriteSudoku()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    Console.Write(grid[i, j]);
                }
                Console.Write("\n");
            }
        }
    }
}
