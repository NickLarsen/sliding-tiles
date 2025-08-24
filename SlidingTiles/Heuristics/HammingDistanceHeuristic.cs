namespace SlidingTiles
{
    public class HammingDistanceHeuristic : IHeuristic
    {
        public string Name => "Hamming Distance";
        public string Abbreviation => "hd";

        public int Calculate(PuzzleState state)
        {
            int hammingDistance = 0;
            for (int i = 0; i < state.Cells.Length; i++)
            {
                if (state.Cells[i] != 0 && state.Cells[i] != i + 1)
                {
                    hammingDistance++;
                }
            }
            return hammingDistance;
        }
    }
}
