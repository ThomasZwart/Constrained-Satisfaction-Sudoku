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

                //switch for selecting algorithm. in each case, keep time, select algorithm, extract data, stop time and write answers.
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
                            Console.WriteLine();
                            newSudoku.WriteSudoku();
                            break;
                        }
                    case 3:
                        {
                            Console.Write("not implemented");

                            long x = DateTime.Now.Ticks;
                            Tuple<Sudoku, int> answer = ChronologicalBacktrackingWithHeuristic(sudoku);
                            Sudoku newSudoku = answer.Item1;
                            int expansions = answer.Item2;
                            Console.WriteLine("Time Elapsed (in seconds): " + TimeSpan.FromTicks(DateTime.Now.Ticks - x).TotalSeconds);
                            Console.WriteLine("Expanded: " + expansions + " vertices");
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

        //forward checking algorithm
        static private Tuple<Sudoku, int> ForwardChecking(Sudoku sudoku)
        {
            int expansionCount = 0;
            Stack<Sudoku> stack = new Stack<Sudoku>();
            stack.Push(sudoku);
            HashSet<Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku>> moderations = new HashSet<Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku>>();

            while (stack.Count != 0)
            {
                int count = sudoku.domainCounter;

                Sudoku newSudoku = sudoku.Clone();

                //if sudoku completely filled
                if (newSudoku.EmptyVariable(0) == null)
                {
                    break;
                }

                // Generate successor
                Tuple<int, int> newVariable = newSudoku.EmptyVariable(0);
                List<int> variableDomain = newSudoku.domains[newVariable.Item1, newVariable.Item2];
                newSudoku[newVariable.Item1, newVariable.Item2] = variableDomain[count];
                stack.Push(newSudoku);

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
                            moderations.Add(new Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku>(new Tuple<List<int>, Tuple<int, int>>(newSudoku.domains[constraint.Item1.Item1, constraint.Item1.Item2], newVariable), newSudoku));
                            //***

                            //remove the value of V_i from domains of V_j
                            newSudoku.domains[constraint.Item1.Item1, constraint.Item1.Item2].Remove(variableDomain[count]);
                        }
                    }

                    sudoku.domainCounter++; // Domain counter goes up
                    expansionCount++; //you have expanded a node (with next if loop)

                }
                // Failed partial solution
                if (!newSudoku.IsPartialSolution(newVariable))
                {
                    Sudoku badSudoku = stack.Pop();
                    foreach (Tuple<Tuple<List<int>, Tuple<int, int>>, Sudoku> moderation in moderations)
                    {
                        //write own equals
                        if (moderation.Item2.Equals(badSudoku))
                        {
                            //
                            //moderation.
                            //
                        }
                    }
                }

                // Backtrack step
                if (sudoku.domainCounter >= variableDomain.Count)
                {
                    sudoku = stack.Pop();
                }
            }

            return null;
        }


        static private Tuple<Sudoku, int> ChronologicalBacktrackingWithHeuristic(Sudoku sudoku)
        {
            int expansionCount = 0;
            Stack<Sudoku> stack = new Stack<Sudoku>();
            stack.Push(sudoku);
            Sudoku newSudoku = sudoku.Clone();

            while (!sudoku.IsSolution())
            {
                newSudoku = sudoku.Clone();

                // Generate and push successor
                Tuple<int, int> newVariable = newSudoku.mcvList[0].Item1;
                newSudoku.mcvList.Remove(newSudoku.mcvList[0]);

                List<int> domain = newSudoku.domains[newVariable.Item1, newVariable.Item2];
                newSudoku[newVariable.Item1, newVariable.Item2] = domain[sudoku.domainCounter];

                if (newSudoku.IsPartialSolution(newVariable))
                    stack.Push(newSudoku);

                // Domain counter goes up
                sudoku.domainCounter++;
                expansionCount++;

                // Backtrack step
                if (sudoku.domainCounter >= domain.Count)
                    sudoku = stack.Pop();
            }
            return new Tuple<Sudoku, int>(sudoku, expansionCount);
        }

        static private Tuple<Sudoku, int> ChronologicalBacktracking(Sudoku sudoku)
        {
            int expansionCount = 0;
            Stack<Sudoku> stack = new Stack<Sudoku>();
            stack.Push(sudoku);
            Sudoku newSudoku = sudoku.Clone();

            while (!sudoku.IsSolution())
            {
                newSudoku = sudoku.Clone();

                // Generate and push successor
                Tuple<int, int> newVariable = newSudoku.EmptyVariable(0);
                List<int> domain = newSudoku.domains[newVariable.Item1, newVariable.Item2];

                newSudoku[newVariable.Item1, newVariable.Item2] = domain[sudoku.domainCounter];
                
                if (newSudoku.IsPartialSolution(newVariable))
                    stack.Push(newSudoku);

                // Domain counter goes up
                sudoku.domainCounter++;
                expansionCount++;

                // Backtrack step
                if (sudoku.domainCounter >= domain.Count)
                    sudoku = stack.Pop();                             
            }
            return new Tuple<Sudoku, int>(sudoku, expansionCount);
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
