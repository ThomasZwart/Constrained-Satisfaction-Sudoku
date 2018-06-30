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
        public List<Tuple<Tuple<int, int>, int>> mcvList;
        
        public Sudoku(int[,] _sudoku)
        {
            grid = _sudoku;
            domainCounter = 0;
            SetFixedNumbers();
            InstantiateDomains();
            InstantiateConstraintSet();
            SortVariablesMCV();
        }

        // For the clone
        public Sudoku()
        {
            domainCounter = 0;
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

        public void SortVariablesMCV()
        {
            mcvList = new List<Tuple<Tuple<int, int>, int>>();
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    mcvList.Add(new Tuple<Tuple<int, int>, int>(new Tuple<int, int>(i, j), domains[i, j].Count));
                }
            }
            mcvList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
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
                        for (int column = blockIndexColumn; column < blockIndexColumn + blockLength; column++) // All numbers in a block
                        {
                            if (!fixedNumbers.Contains(new Tuple<int, int>(row, column)))
                            {
                                for (int y = blockIndexRow; y < blockIndexRow + blockLength; y++)
                                {
                                    for (int x = blockIndexColumn; x < blockIndexColumn + blockLength; x++)
                                    {
                                        // Domain reduction
                                        if (grid[y,x] != 0)
                                        {
                                            domains[row, column].Remove(grid[y, x]);
                                        }
                                        // Constraint set
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
                            // Domain reduction
                            if (grid[row, i] != 0)
                                domains[row, column].Remove(grid[row, i]);

                            if (i != column)
                                constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(row, i)));
                        }

                        for (int i = 0; i < Length; i++)
                        {
                            // Domain reduction
                            if (grid[i, column] != 0)
                                domains[row, column].Remove(grid[i, column]);

                            if (i != row)
                                constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(i, column)));
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
                        domains[i, j] = Enumerable.Range(1, Length).ToList();
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
            Sudoku clone = new Sudoku
            {
                fixedNumbers = new HashSet<Tuple<int, int>>(fixedNumbers),
                grid = grid.Clone() as int[,],
                constraintSet = new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>(constraintSet),
                domains = new List<int>[Length, Length],
                mcvList = new List<Tuple<Tuple<int, int>, int>>(mcvList)
            };
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    clone.domains[i, j] = new List<int>(domains[i, j]);
                }
            }
           
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
