using System.Collections.Generic;

namespace Fences
{
    public class Fence
    {
        private List<FenceEntry> _entries = new List<FenceEntry>();

        public void AddEntry(FenceEntry entry)
        {
            _entries.Add(entry);
        }

        public List<FenceEntry> GetEntries()
        {
            return _entries;
        }

        public SteelMassStat ComputeSteelMass()
        {
            SteelMassStat result = new SteelMassStat();
            foreach (FenceEntry entry in _entries)
            {
                result.IncreaseAllMasses(entry.ComputeSteelMass());
            }
            return result;
        }
    }
}