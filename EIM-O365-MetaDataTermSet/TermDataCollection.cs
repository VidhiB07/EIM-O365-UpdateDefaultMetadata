using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_MetaDataTermSet
{
    public class TermDataCollection
    {
        public string WebTitle { get; set; }
        public string ProcessAreaValue { get; set; }
        public string ProcessAreaGuid { get; set; }
        public List<TermItem> ProcessArea { get; set; }
        public string ProcessValue { get; set; }
        public string ProcessGuid { get; set; }
        public List<TermItem> Process { get; set; }
        public string InformationAssetValue { get; set; }
        public string InformationAssetGuid { get; set; }
        public List<TermItem> InformationAsset { get; set; }
        public string BusinessAreaValue { get; set; }
        public string BusinessAreaGuid { get; set; }
        public List<TermItem> BusinessArea { get; set; }
        public string CountryValue { get; set; }
        public string CountryGuid { get; set; }
        public List<TermItem> Country { get; set; }
        public string LegalEntityValue { get; set; }
        public string LegalEntityGuid { get; set; }
        public List<TermItem> LegalEntity { get; set; }
        public string SecurityClassificationValue { get; set; }
        public string SecurityClassificationGuid { get; set; }
        public List<TermItem> SecurityClassification { get; set; }
    }
}
