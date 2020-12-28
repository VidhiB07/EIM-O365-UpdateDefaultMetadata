using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EIM_O365_MetaDataTermSet.Data {
    public class FieldConfigCollection : List<FieldConfig> {
        /// <summary>
        /// Get by field config by field internal name
        /// </summary>
        /// <param name="internalName">field internal name as string</param>
        /// <returns>field config as FieldConfig</returns>
        public FieldConfig GetByInternalName(string internalName) {
            return this.FirstOrDefault(i => i.InternalName == internalName);
        }
        /// <summary>
        /// Get TermSet id by Internal Name
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns></returns>
        public string GetTermSetIdByInternalName(string internalName) {
            var item = this.GetByInternalName(internalName);
            return item?.TermSetId;
        }
        /// <summary>
        /// Returns either Enterprise or non-enterprise metadata fields
        /// </summary>
        /// <param name="isEnterpriseFields">Define if field should be enterprise field or not to be returned as bool</param>
        /// <returns></returns>
        public FieldConfigCollection GetItemsByIsEnterpriseMetadata(bool isEnterpriseFields) {
            FieldConfigCollection coll = new FieldConfigCollection();
            coll.AddRange(this.FindAll(i => i.IsEnterpriseMetadataField == isEnterpriseFields));
            return coll;
        }

    }
}
