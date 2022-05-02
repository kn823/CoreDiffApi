namespace CoreDiffApi.Models
{
    /// <summary>
    /// Diffs stores the bit value(s) and position(s) where left encode & right encode differ
    /// </summary>
    public class DiffOutputDiffs : DiffOutput
    {
        public DiffOutputDiffs()
        {
            Diffs = new Dictionary<int, string>();
        }
        public Dictionary<int, string> Diffs;
    }
}
