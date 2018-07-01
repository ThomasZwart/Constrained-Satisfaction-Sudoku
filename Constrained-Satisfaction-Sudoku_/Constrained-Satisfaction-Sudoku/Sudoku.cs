using System;
using System.Collections.Generic;
using System.Linq;

namespace Constrained_Satisfaction_Sudoku
{
    class Sudoku
    {
        public int[,] grid;
        public int domainCounter;
        public List<int>[,] domains;
        public List<Tuple<Tuple<int, int>, int>> mcvList;
        public HashSet<Tuple<int, int>> fixedNumbers;
        public HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> constraintSet;
              
        public Sudoku(int[,] _sudoku)
        {
            grid = _sudoku;
            domainCounter = 0;
            SetFixedNumbers();
            InstantiateDomains();
            InstantiateConstraintSet();
            InitMCVList();
        }

        // For the clone, so there is no unneccesarily initialization every time a clone is made
        public Sudoku()
        {
            domainCounter = 0;
        }

        // Check to see if a sudoku is a partial solution with respect to a variable (for efficiency)
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
        
        // Clears the mcv list and fills it with the updated domains by forward checking
        public void UpdateMCVList()
        {
            // List consists of tuples of (location, number of domainelements)            
            mcvList.Clear();
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    //add all locations with the right number domain elements
                    mcvList.Add(new Tuple<Tuple<int, int>, int>(new Tuple<int, int>(i, j), domains[i, j].Count));
                }
            }
            // Sort using inherent sort method with overwrited CompareTo to deal with tuples.
            mcvList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
        }


        // Sort variables by domain size for the most constrained variable heuristic
        public void InitMCVList()
        {
            // List consists of tuples of (location, number of domainelements)            
            mcvList = new List<Tuple<Tuple<int, int>, int>>();
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    //add all locations with the right number domain elements
                    mcvList.Add(new Tuple<Tuple<int, int>, int>(new Tuple<int, int>(i, j), domains[i, j].Count));
                }
            }
            // Sort using inherent sort method with overwrited CompareTo to deal with tuples.
            mcvList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
        }

        // The amount of empty variables
        public int AmountEmptyVariables()
        {
            int counter = 0;
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        // Returns the first empty variable (without any heuristic)
        public Tuple<int, int> EmptyVariable(int number)
        {
            int counter = 0;
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        counter++;
                        if (counter > number)
                            return new Tuple<int, int>(i, j);
                    }
                }
            }
            return null;
        }

        // Check to see if the sudoku is solved
        public bool IsSolution()
        {
            if (AmountEmptyVariables() == 0)
            {
                foreach (Tuple<Tuple<int, int>, Tuple<int, int>> constraint in constraintSet)
                {
                    // Once a contraint is violated the sudoku isn't solved
                    if (grid[constraint.Item1.Item1, constraint.Item1.Item2] == grid[constraint.Item2.Item1, constraint.Item2.Item2])
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        // To instantiate the constraint sets and also some domain reduction
        private void InstantiateConstraintSet()
        {
            constraintSet = new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>();
            int blockLength = (int)Math.Sqrt(Length);

            // For every block
            for (int blockRow = 0; blockRow < blockLength; blockRow++)
            {
                for (int blockColumn = 0; blockColumn < blockLength; blockColumn++)
                {
                    int blockIndexRow = blockRow * blockLength;
                    int blockIndexColumn = blockColumn * blockLength;

                    // All variables in the block
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
                                        // Domain reduction
                                        if (grid[y,x] != 0)
                                        {
                                            domains[row, column].Remove(grid[y, x]);
                                        }
                                        // Set block constraints
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

                            // Set horizontal constrains
                            if (i != column)
                                constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(row, i)));
                        }

                        for (int i = 0; i < Length; i++)
                        {
                            // Domain reduction
                            if (grid[i, column] != 0)
                                domains[row, column].Remove(grid[i, column]);

                            // Set vertical constraints
                            if (i != row)
                                constraintSet.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(row, column), new Tuple<int, int>(i, column)));
                        }
                    }
                }
            }
        }

        // Instantiate the domains for all variables
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
                        domains[i, j] = new List<int> {grid[i, j]};
                    }
                }
            }
        }

        // Clones the current sudoku
        public Sudoku Clone() 
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

        // Used to access the grid variable easily
        public int this[int index1, int index2] 
        {
            get { return grid[index1, index2]; }
            set { grid[index1, index2] = value; }
        }

        // Sets all numbers that aren't blank as fixed numbers in the sudoku based on their indices        
        public void SetFixedNumbers()                                            
        {
            fixedNumbers = new HashSet<Tuple<int, int>>();
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        // Tuple of (row, column) coordinates in the grid
                        fixedNumbers.Add(new Tuple<int, int>(i, j)); 
                    }
                }
            }
        }

        // The length of the sudoku
        public int Length 
        {
            get { return grid.GetLength(0); }
        }

        // Method to write the sudoku out in console
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
