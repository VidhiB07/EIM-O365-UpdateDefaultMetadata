using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_UpdateDefaultMetaData
{
    public class DefaultField
    {
        public string InternalName { get; set; }
        public string TermLabel { get; set; }
        public string OriginGuid { get; set; }
        public string Guid { get; set; }
        public Term TermItem { get; set; }

    }
    public class DefaultFields : List<DefaultField> { }
}

