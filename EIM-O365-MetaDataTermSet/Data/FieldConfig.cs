using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_MetaDataTermSet.Data {
    public class FieldConfig {
        /// <summary>
        /// Field Internal Name
        /// </summary>
        public string InternalName { get; set; }
        /// <summary>
        /// Term set id assassinated with taxonomyfield if the field is a tax field
        /// </summary>
        public string TermSetId { get; set; }
        /// <summary>
        /// If for associated content type for this field
        /// </summary>
        public string ContentTypeIdAssociation { get; set; }

        /// <summary>
        /// Field XML definition to generate field
        /// </summary>
        public string FieldDefinition { get; set; }
        /// <summary>
        /// Is this field an Enterprise Metadata field
        /// </summary>
        public bool IsEnterpriseMetadataField { get; set; }

        
    }
}
