using System;
using SlidingTiles;

namespace DebugWalkingDistance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing Walking Distance Heuristic...");
            
            var heuristic = new WalkingDistanceHeuristic();
            
            // Test case 1: Goal state (should return 0)
            var goalState = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 0
            });
            
            var goalResult = heuristic.Calculate(goalState);
            Console.WriteLine($"Goal state walking distance: {goalResult}");
            
            // Test case 2: One move away (swap 8 and 0)
            var oneMoveState = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 0, 8
            });
            
            var oneMoveResult = heuristic.Calculate(oneMoveState);
            Console.WriteLine($"One move away walking distance: {oneMoveResult}");
            
            // Test case 3: Scrambled state
            var scrambledState = new PuzzleState(3, 3, new int[]
            {
                2, 1, 3,
                4, 5, 6,
                7, 8, 0
            });
            
            var scrambledResult = heuristic.Calculate(scrambledState);
            Console.WriteLine($"Scrambled state walking distance: {scrambledResult}");
            
            // Test case 4: Complex scrambled state
            var complexState = new PuzzleState(3, 3, new int[]
            {
                8, 1, 3,
                4, 0, 2,
                7, 6, 5
            });
            
            var complexResult = heuristic.Calculate(complexState);
            Console.WriteLine($"Complex scrambled state walking distance: {complexResult}");
            
            // Test case 5: All tiles out of place
            var allOutOfPlaceState = new PuzzleState(3, 3, new int[]
            {
                8, 7, 6,
                5, 4, 3,
                2, 1, 0
            });
            
            var allOutOfPlaceResult = heuristic.Calculate(allOutOfPlaceState);
            Console.WriteLine($"All tiles out of place walking distance: {allOutOfPlaceResult}");
            
            Console.WriteLine("\nTest completed!");
        }
    }
}
