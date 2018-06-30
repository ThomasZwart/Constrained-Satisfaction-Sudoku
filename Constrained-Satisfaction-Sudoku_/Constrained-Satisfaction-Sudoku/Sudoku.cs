using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constrained_Satisfaction_Sudoku
{
    class Sudoku
    {
        public int[,] grid;
        public List<int>[,] domains;
        public HashSet<Tuple<int, int>> fixedNumbers;
        public HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> constraintSet;
        public int domainCounter;

        public Sudoku(int[,] _sudoku)
        {
            grid = _sudoku;
            domainCounter = 0;
            SetFixedNumbers();
            InstantiateDomains();
            InstantiateConstraintSet();
            DomainReduction();
        }

        public void DomainReduction()
        {
            for (int row = 0; row < Length; row++)
            {
                for (int column = 0; column < Length; column++)
                {
                    
                }
            }
        }

        public bool IsPartialSolution(Tuple<int, int> variable)
        {
            foreach (Tuple<Tuple<int, int>, Tuple<int, int>> constraint in constraintSet)
            {
                if (constraint.Item1.Equals(variable))
                {
                    if (grid[variable.Item1, variable.Item2] == grid[constraint.Item2.Item1, constraint.Item2.Item2])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Tuple<int, int> FirstEmptyVariable()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        return new Tuple<int, int>(i, j);
                    }
                }
            }
            return null;
        }

        private void InstantiateConstraintSet()
        {
            constraintSet = new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>();
            int blockLength = (int)Math.Sqrt(Length);
            for (int blockRow = 0; blockRow < blockLength; blockRow++)
            {
                for (int blockColumn = 0; blockColumn < blockLength; blockColumn++)
                {
                    int blockIndexRow = blockRow * blockLength;
                    int blockIndexColumn = blockColumn * blockLength;

                    for (int row = blockIndexRow; row < blockIndexRow + blockLength; row++)
                    {
                        for (int column = blockIndexColumn; column < blockIndexColumn + blockLength; column++)
                        {
                            if (!fixedNumbers.Contains(new Tuple<int, int>(row, column)))
                            {
                                for (int y = blockIndexRow; y < blockIndexRow + blockLength; y++)
                                {
                                    for (int x = blockIndexColumn; x < blockIndexColumn + blockLength; x++)
                                    {
                                        if (!(row == y && x == column))
                                        {
                                            constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(y, x)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int row = 0; row < Length; row++)
            {
                for (int column = 0; column < Length; column++)
                {
                    if (!fixedNumbers.Contains(new Tuple<int, int>(row, column)))
                    {
                        for (int i = 0; i < Length; i++)
                        {
                            if (i != column)
                            {
                                constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(row, i)));
                            }
                        }

                        for (int i = 0; i < Length; i++)
                        {
                            if (i != row)
                            {
                                constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(i, column)));
                            }
                        }
                    }
                }
            }
        }

        private void InstantiateDomains()
        {
            domains = new List<int>[Length, Length];
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (!fixedNumbers.Contains(new Tuple<int, int>(i, j)))
                    {
                        domains[i, j] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9};
                    }
                    else
                    {
                        domains[i, j] = new List<int> { grid[i, j]};
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
            clone.domainCounter = domainCounter;
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    clone.domains[i, j] = new List<int>(domains[i, j]);
                }
            }
            clone.constraintSet = new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>(constraintSet);

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
