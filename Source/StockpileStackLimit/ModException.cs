using System;

namespace StockpileStackLimit
{
    public class ModException : Exception
    {
        public ModException(string s) : base($"[{Mod.Name}]{s}")
        {
        }
    }
}