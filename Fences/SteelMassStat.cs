using System.Collections.Generic;

namespace Fences
{
    public class SteelMassStat
    {
        private Dictionary<SteelType, double> _type2mass = new Dictionary<SteelType, double>();

        public void IncreaseMass(SteelType type, double mass)
        {
            if (!_type2mass.ContainsKey(type))
            {
                _type2mass[type] = mass;
            }
            else
            {
                _type2mass[type] += mass;
            }
        }

        public void IncreaseAllMasses(SteelMassStat stat)
        {
            foreach (var typemass in stat._type2mass)
            {
                IncreaseMass(typemass.Key, typemass.Value);
            }
        }
    }
}