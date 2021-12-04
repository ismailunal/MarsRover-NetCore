using System;
using System.Collections.Generic;
using System.Linq;

namespace MarsRover_NetCore
{
    public class Program
    {
        ///Variable for grid size 
        public static Grid grid_size = new Grid();

        /// <summary>
        /// This defines the Grid class.
        /// </summary>
        public class Grid
        {
            /// <summary>
            /// Grid size in horizontal space, x
            /// </summary>
            public int M { get; set; }

            /// <summary>
            /// Grid size in vertical space, y
            /// </summary>
            public int N { get; set; }
        }

        /// <summary>
        /// This defines the orientation of North, East, South and West in clockwise order. Order is important!
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            /// North
            /// </summary>
            N = 0,
            /// <summary>
            /// East
            /// </summary>
            E = 1,
            /// <summary>
            /// South
            /// </summary>
            S = 2,
            /// <summary>
            /// West
            /// </summary>
            W = 3
        }

        /// <summary>
        /// This defines the possible movements that rover can achieve
        /// </summary>
        public enum Move
        {
            /// <summary>
            /// Forward one space
            /// </summary>
            F = 0,
            /// <summary>
            /// Rotate Left 90 degrees
            /// </summary>
            L = 1,
            /// <summary>
            /// Rotate Right 90 degrees
            /// </summary>
            R = 2
        }

        /// <summary>
        /// This defines the Rover class. 
        /// </summary>
        public class Rover
        {
            #region Properties
            /// <summary>
            /// X is horizontal, easterly direction
            /// </summary>            
            public int X { get; set; }

            /// <summary>
            /// Y is vertical, northerly direction
            /// </summary>            
            public int Y { get; set; }

            /// <summary>
            /// Orientation of the Rover
            /// </summary>
            public Orientation Orientation { get; set; }

            /// <summary>
            /// Pre-defined list of moves of the rover (valid moves are L,R,F)
            /// </summary>
            public List<Move> Moves { get; set; }

            /// <summary>
            /// Is true when the rover moved off the grid
            /// </summary>
            public bool IsLost { get; set; }
            #endregion

            #region Methods
            /// <summary>
            /// Initial method that rover should launch. 3 2 1 Lift Off!
            /// </summary>
            public void Launch()
            {
                foreach (Move move in Moves)
                {
                    int last_valid_move_x = this.X;
                    int last_valid_move_y = this.Y;

                    if (move == Move.F)
                        this.Forward();
                    else if (move == Move.R || move == Move.L)
                        this.Rotate(move);

                    if (this.X < 0 || this.Y < 0 || this.X > grid_size.M || this.Y > grid_size.N)
                    {
                        this.X = last_valid_move_x;
                        this.Y = last_valid_move_y;
                        this.IsLost = true;
                        break;
                    }
                }
            }

            /// <summary>
            /// Moves the Rover forward one space
            /// </summary>
            private void Forward()
            {
                if (this.Orientation == Orientation.N)
                {
                    this.Y += 1;
                }
                else if (this.Orientation == Orientation.S)
                {
                    this.Y -= 1;
                }
                else if (this.Orientation == Orientation.E)
                {
                    this.X += 1;
                }
                else if (this.Orientation == Orientation.W)
                {
                    this.X -= 1;
                }
            }

            /// <summary>
            /// Rotates the Rover 90 degrees
            /// </summary>
            /// <param name="rotation">Rotation direction either Left or Right</param>
            private void Rotate(Move rotation)
            {
                if (rotation == Move.R)
                {
                    this.Orientation = this.Orientation.Next();
                }
                else if (rotation == Move.L)
                {
                    this.Orientation = this.Orientation.Previous();
                }
            }
            #endregion
        }


        static void Main(string[] args)
        {
            ///Variables
            List<Rover> all_rovers = new List<Rover>();
            string grid_size_input;
            string initial_state_input;

            ///Input collection
            Console.WriteLine("Mars Rover");

            //Read the input : grid size with (m x n)
            grid_size_input = Console.ReadLine().Trim();

            if (Validate_grid_size(grid_size_input))
            {
                //checks if the grid size is properly set

                //retrieve user input for robot(s) initial position, orientation and sequence of moves
                string line;
                while ((line = Console.ReadLine()) != null && !string.IsNullOrEmpty(line))
                {
                    initial_state_input = line.Trim();
                    all_rovers.Add(ExtractSubstringAndPrepareRover(initial_state_input));
                }

                try
                {
                    //Loop through all valid rovers, launch and form the output
                    foreach (Rover r in all_rovers)
                    {
                        //Prepare the rover and lauch
                        r.Launch();
                        //print out the final state
                        Console.WriteLine($"({r.X}, {r.Y}, {r.Orientation}) " + ($"{(r.IsLost ? "LOST" : "")}"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Constants.ERROR_ROVER_CREATION + "\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Validates the grid size entered by the user (m x n)
        /// </summary>
        /// <param name="grid_size_input">input of grid size in string format</param>
        /// <returns>True when user input is correct, false if not</returns>
        private static bool Validate_grid_size(string grid_size_input)
        {
            try
            {
                List<int> grid_size = grid_size_input.Split(' ').Select(Int32.Parse).ToList();
                Program.grid_size.M = grid_size[0];
                Program.grid_size.N = grid_size[1];
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.ERROR_INVALID_GRID_SIZE + "\n" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Extracts the substring for rover's initial position and orientation
        /// </summary>
        /// <param name="text">full string of user input</param>
        /// <param name="start_char">Optional start character for the initial rover position, orientation</param>
        /// <param name="end_char">Optional end character for the initial rover position, orientation</param>
        /// <returns>The rover object</returns>
        private static Rover ExtractSubstringAndPrepareRover(string text, string start_char = "(", string end_char = ")")
        {
            try
            {
                text = text.Replace(" ", string.Empty);
                int start_index = text.IndexOf(start_char);
                int end_index = text.LastIndexOf(end_char);

                List<string> result = text.Substring(start_index + start_char.Length, end_index - start_index - start_char.Length).Split(",").ToList();

                ///Rover object creation
                Rover rover = new Rover();
                if (Int32.TryParse(result[0], out int result_x))
                    rover.X = result_x;
                if (Int32.TryParse(result[1], out int result_y))
                    rover.Y = result_y;
                if (Enum.TryParse(result[2], true, out Orientation result_orientation))
                    rover.Orientation = result_orientation;
                rover.Moves = new List<Move>();

                foreach (char c in text.Substring(end_index + 1).ToUpper())
                {
                    if (Enum.TryParse(c.ToString(), true, out Move result_move))
                        rover.Moves.Add(result_move);
                }

                return rover;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.ERROR_INVALID_ROVER_INPUT + "\n" + ex.Message);
                return new Rover();
            }
        }
    }

    /// <summary>
    /// Extention for Enum to get next and previous item
    /// </summary>
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            T[] enumsArr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(enumsArr, src) + 1;
            return (enumsArr.Length == j) ? enumsArr[0] : enumsArr[j];
        }
        public static T Previous<T>(this T src) where T : struct
        {
            T[] enumsArr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(enumsArr, src) - 1;
            return (j == -1) ? enumsArr[enumsArr.Length - 1] : enumsArr[j];
        }
    }

    /// <summary>
    /// Constants of the program
    /// </summary>
    public class Constants
    {
        //Error Messages
        public const string ERROR_INVALID_ROVER_INPUT = "Rover values are invalid, please use the format '(x, y, O) M' where x is horizontal position, y is vertical position, O is orientation(N,E,S,W) and M is the sequence of movements (F,L,R).";
        public const string ERROR_INVALID_GRID_SIZE = "Grid size is invalid, please use the model (m x n) where m and n are integer values.";
        public const string ERROR_ROVER_CREATION = "Error occured while creating the rover";
    }
}
