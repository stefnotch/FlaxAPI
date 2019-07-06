using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaxEngine
{
    /// <summary>
    /// My custom graph
    /// Does *not* inherit from JsonAsset!
    /// </summary>
    public sealed class NumberGraph
    {
        public string GraphName { get; set; } = "Neko 42";
    }
}
