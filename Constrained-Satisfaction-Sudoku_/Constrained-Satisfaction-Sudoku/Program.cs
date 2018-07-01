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
            while(true)
            {
                int[,] input = ReadSudoku();
                Sudoku sudoku = new Sudoku(input);
                Console.WriteLine();
                Console.WriteLine("Select algorithm: ");
                Console.WriteLine("   1: Chronological Backtracking");
                Console.WriteLine("   2: Chronological Backtracking with MCV heuristic");
                Console.WriteLine("   3: Forward Checking");
                Console.WriteLine("   4: Forward Checking with MCV heuristic");
                Console.WriteLine();

                // Switch for selecting algorithm. in each case, keep time, select algorithm, extract data, stop time and write answers.
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        {
                            long x = DateTime.Now.Ticks;
                            Tuple<Sudoku, int> answer = ChronologicalBacktracking(sudoku);
                            Sudoku newSudoku = answer.Item1;
                            int expansions = answer.Item2;
                            Console.WriteLine("Time Elapsed (in seconds): " + TimeSpan.FromTicks(DateTime.Now.Ticks - x).TotalSeconds);
                            Console.WriteLine("Expanded: " + expansions + " vertices");
                            Console.WriteLine("Is Solution? " + newSudoku.IsSolution().ToString());
                            Console.WriteLine();
                            newSudoku.WriteSudoku();
                            break;
                        }

                    case 2:
                        {
                            long x = DateTime.Now.Ticks;
                            Tuple<Sudoku, int> answer = ChronologicalBacktrackingWithHeuristic(sudoku);
                            Sudoku newSudoku = answer.Item1;
                            int expansions = answer.Item2;
                            Console.WriteLine("Time Elapsed (in seconds): " + TimeSpan.FromTicks(DateTime.Now.Ticks - x).TotalSeconds);
                            Console.WriteLine("Expanded: " + expansions + " vertices");
                            Console.WriteLine("Is Solution? " + newSudoku.IsSolution().ToString());
                            Console.WriteLine();
                            newSudoku.WriteSudoku();
                            break;
                        }
                    case 3:
                        {
                            Console.Write("not implemented");

                            long x = DateTime.Now.Ticks;
                            Tuple<Sudoku, int> answer = ForwardChecking(sudoku);
                            Sudoku newSudoku = answer.Item1;
                            int expansions = answer.Item2;
                            Console.WriteLine("Time Elapsed (in seconds): " + TimeSpan.FromTicks(DateTime.Now.Ticks - x).TotalSeconds);
                            Console.WriteLine("Expanded: " + expansions + " vertices");
                            Console.WriteLine("Is Solution? " + newSudoku.IsSolution().ToString());
                            Console.WriteLine();
                            newSudoku.WriteSudoku();
                            break;
                        }

                    case 4:
                        {
                            Console.WriteLine("not implemented");
                            break;
                        }                   
                }
                Console.WriteLine();
            }
        }

        // Forward checking algorithm
        static private Tuple<Sudoku, int> ForwardChecking(Sudoku sudoku)
        {
            // Expansion counter
            int expansionCount = 0;
            Stack<Sudoku> stack = new Stack<Sudoku>();
            // Initial push
            stack.Push(sudoku);
            // Set to keep track of changes
            HashSet<Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku>> moderations = new HashSet<Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku>>();
            Sudoku newSudoku = sudoku.Clone();

            while (!sudoku.IsSolution())
            {
                newSudoku = sudoku.Clone();

                // Get new variable, first empty one
                Tuple<int, int> newVariable = newSudoku.EmptyVariable(0);
                List<int> domain = newSudoku.domains[newVariable.Item1, newVariable.Item2];

                newSudoku[newVariable.Item1, newVariable.Item2] = domain[sudoku.domainCounter];

                // Check if the new sudoku is a partial solution, if so, push to stack
                if (newSudoku.IsPartialSolution(newVariable))
                    stack.Push(newSudoku);

                // Domain and expansion counter
                sudoku.domainCounter++;
                expansionCount++;

                //update constraints where this sudoku is a part of. possible, but how to undo them?
                //if we keep an hashset with Tuple<TUple<Domainlist, location>, Sudoku>
                //so we keep a tuple of old domain for each location that is changed by the new variable in the sudoku.
                //for instance. we try value 2 for Variable. it has constraint with Location. Location has domain{1,2,3}. we store this combination in tpule with Variable. <<OldDomain, Location>, Variable> and update domain to {1,3}.
                //if further on, we have to pop Sudoku with changed Variable from stack, because value 2 doesn't work, we pop it from stack (and retrieve it), we search hashSet for the sudoku and for each value in the hashset we update the OldDomains to the location (so we are back at original before updating domains).

                //find all constraints that should be updated and update them
                //check all constraints in constraintset.
                //if V_i has value, get all constraints C_ji where V_j not instantiated, and make them consistent.

                //!!!do we have double constraints? --> do we have to check for C_ij to? or does update on just C_ji updates all necessary domains??/
                foreach (Tuple<Tuple<int, int>, Tuple<int, int>> constraint in newSudoku.constraintSet)
                {
                    if (constraint.Item2.Equals(newVariable))
                    {
                        //if domain of V_j is not 1 (not instantiated)
                        if (newSudoku.domains[constraint.Item1.Item1, constraint.Item1.Item2].Count != 1)
                        {
                            //add to hashset of moderated domains with stored <<OldDomain, locationofDomain>, Sudoku with variable (retrievable from stack)>

                            //***
                            //moderations.Add(new Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku>(new Tuple<List<int>, Tuple<int, int>>(newSudoku.domains[constraint.Item1.Item1, constraint.Item1.Item2], newVariable), newSudoku));
                            //***

                            //remove the value of V_i from domains of V_j
                            newSudoku.domains[constraint.Item1.Item1, constraint.Item1.Item2].Remove(domain[newSudoku.domainCounter]);

                            //if new domain of a location is empty, it should step back.
                            //problem with double backtrack step (if also sudoku.domaincounter >= domain.count)
                            if (newSudoku.domains[constraint.Item1.Item1, constraint.Item1.Item2].Count == 0)
                            {
                                sudoku = stack.Pop();
                            }
                        }
                    }
                }

                // Backtrack step
                if (sudoku.domainCounter >= domain.Count)
                    sudoku = stack.Pop();
            }
            return new Tuple<Sudoku, int>(sudoku, expansionCount);
        }

        // Chronological backtracking algorithm with most-contrained-variable heuristic
        static private Tuple<Sudoku, int> ChronologicalBacktrackingWithHeuristic(Sudoku sudoku)
        {
            // Counting the amount of expansions
            int expansionCount = 0;
            Stack<Sudoku> stack = new Stack<Sudoku>();
            // Initial push
            stack.Push(sudoku);
            Sudoku newSudoku = sudoku.Clone();

            // Loop untill solution is found
            while (!sudoku.IsSolution())
            {
                newSudoku = sudoku.Clone();

                // Get new variable, first one in the mcv list
                Tuple<int, int> newVariable = newSudoku.mcvList[0].Item1;
                // Remove so that variables get looped through
                newSudoku.mcvList.Remove(newSudoku.mcvList[0]);
                List<int> domain = newSudoku.domains[newVariable.Item1, newVariable.Item2];
                newSudoku[newVariable.Item1, newVariable.Item2] = domain[sudoku.domainCounter];

                // Check if the new sudoku is a partial solution, if so, push to stack
                if (newSudoku.IsPartialSolution(newVariable))
                    stack.Push(newSudoku);

                // Domain and expansion counter
                sudoku.domainCounter++;
                expansionCount++;

                // Backtrack step
                if (sudoku.domainCounter >= domain.Count)
                    sudoku = stack.Pop();
            }
            return new Tuple<Sudoku, int>(sudoku, expansionCount);
        }

        // Chronological backtracking algorithm
        static private Tuple<Sudoku, int> ChronologicalBacktracking(Sudoku sudoku)
        {
            // Counting the amount of expansions
            int expansionCount = 0;
            Stack<Sudoku> stack = new Stack<Sudoku>();
            // Initial push
            stack.Push(sudoku);
            Sudoku newSudoku = sudoku.Clone();

            while (!sudoku.IsSolution())
            {
                newSudoku = sudoku.Clone();

                // Get new variable, the first empty one
                Tuple<int, int> newVariable = newSudoku.EmptyVariable(0);
                List<int> domain = newSudoku.domains[newVariable.Item1, newVariable.Item2];

                newSudoku[newVariable.Item1, newVariable.Item2] = domain[sudoku.domainCounter];

                // Check if the new sudoku is a partial solution, if so, push to stack
                if (newSudoku.IsPartialSolution(newVariable))
                    stack.Push(newSudoku);

                // Domain and expansion counter
                sudoku.domainCounter++;
                expansionCount++;

                // Backtrack step
                if (sudoku.domainCounter >= domain.Count)
                    sudoku = stack.Pop();                             
            }
            return new Tuple<Sudoku, int>(sudoku, expansionCount);
        }

        // Read the sudoku from the console
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
                Environment.Exit(0); // Exit console
                return null;
            }
        }
    }


}
