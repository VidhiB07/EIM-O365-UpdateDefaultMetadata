using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_MetaDataTermSet
{
    public class TermItem
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string Icon { get; set; }
        public bool Leaf { get; set; }
    }
}