using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_MetaDataTermSet
{
    public class TermSetName
    {
        public enum Name
        {
            [EnumStringAttribute("dd8ea1ba-7897-47bf-ba2a-f94bff34ce6d")] Basin,
            [EnumStringAttribute("b3aea3ce-3c5a-41c5-8af2-8da777f85a59")] Block,
            [EnumStringAttribute("a8ca9a86-9113-48ea-8063-579000373f6c")] BusinessArea,
            [EnumStringAttribute("c14b3bad-ae35-4ced-a532-48f1a3681732")] BusinessArrangementArea,
            [EnumStringAttribute("a6e771ef-7935-4928-8fac-8734f039e96d")] Continent,
            [EnumStringAttribute("5bcd3742-12ff-44ba-a297-326d29cfc029")] Counterparty,
            [EnumStringAttribute("0250f7c1-058f-435e-97fc-c7f58584592f")] Country,
            [EnumStringAttribute("aa7075b8-d08f-492d-8b3b-46b38bab4a0d")] DecisionGate,
            [EnumStringAttribute("d1866989-1eec-46c3-b7d1-bd44d1645697")] Discipline,
            [EnumStringAttribute("bb03b0af-6c71-485a-8d19-649941a9afaa")] Field,
            [EnumStringAttribute("b76f03a6-1db7-44cf-ab25-1870b16029af")] InformationAsset,
            [EnumStringAttribute("547ebc0c-73a3-4a88-b498-ea2a950fe21d")] LegalEntity,
            [EnumStringAttribute("6c8238d0-84a8-4183-b7b1-be0b06d14b7b")] License,
            [EnumStringAttribute("1865ccaa-a637-45c8-af65-07991be48bf5")] MarketingProduct,
            [EnumStringAttribute("f36a540b-7bb7-4142-ae76-bc96ba0f7f01")] OrganisationUnit,
            [EnumStringAttribute("6a72daf7-6d10-4de7-9de1-ed36ace60846")] Plant,
            [EnumStringAttribute("760386cc-a14d-4138-b860-5a508bf62739")] PlantType,
            [EnumStringAttribute("041c847a-4248-484c-8e89-6aba1a2f3a53")] ProcessArea,
            [EnumStringAttribute("3b80e1d2-5900-412d-b185-fa8652847d19")] Process,
            [EnumStringAttribute("05691e65-85f3-4033-9ed1-4d2fcd15000f")] Region,
            [EnumStringAttribute("6586e8a5-4521-47b6-a267-a502c9a77ea2")] SecurityClassification,
            [EnumStringAttribute("d33f5a32-5927-419a-919b-e9f3bcd9191e")] SeismicSurvey,
            [EnumStringAttribute("68f706c4-2129-47d0-b770-1bab961b61be")] Source,
            [EnumStringAttribute("ee819452-dde8-4ad2-a8b9-030bdfafa61b")] Status,
            [EnumStringAttribute("92fe4f39-937b-49c2-a5a7-fd1767a6d8f7")] Well,
            [EnumStringAttribute("0701e62b-4363-436b-ba46-8f5ac21d05ba")] Wellbore
        }
        public static string GetTermSetGuidByInternalName(string internalName)
        {
            Name name = Name.Basin;
            switch (internalName)
            {
                case "EIMBasin":
                    name = TermSetName.Name.Basin;
                    break;
                case "EIMBlock":
                    name = TermSetName.Name.Block;
                    break;
                case "EIMBusinessArea":
                    name = TermSetName.Name.BusinessArea;
                    break;
                case "EIMBusinessArrangementArea":
                    name = TermSetName.Name.BusinessArrangementArea;
                    break;
                case "EIMContinent":
                    name = TermSetName.Name.Continent;
                    break;
                case "EIMCounterparty":
                    name = TermSetName.Name.Counterparty;
                    break;
                case "EIMCountry":
                    name = TermSetName.Name.Country;
                    break;
                case "EIMDecisionGate":
                    name = TermSetName.Name.DecisionGate;
                    break;
                case "EIMDiscipline":
                    name = TermSetName.Name.Discipline;
                    break;
                case "EIMField":
                    name = TermSetName.Name.Field;
                    break;
                case "EIMInformationAsset":
                    name = TermSetName.Name.InformationAsset;
                    break;
                case "EIMLegalEntity":
                    name = TermSetName.Name.LegalEntity;
                    break;
                case "EIMLicense":
                    name = TermSetName.Name.License;
                    break;
                case "EIMMarketingProduct":
                    name = TermSetName.Name.MarketingProduct;
                    break;
                case "EIMOrganisationUnit":
                    name = TermSetName.Name.OrganisationUnit;
                    break;
                case "EIMPlant":
                    name = TermSetName.Name.Plant;
                    break;
                case "EIMPlantType":
                    name = TermSetName.Name.PlantType;
                    break;
                case "EIMProcess":
                    name = TermSetName.Name.Process;
                    break;
                case "EIMProcessArea":
                    name = TermSetName.Name.ProcessArea;
                    break;
                case "EIMRegion":
                    name = TermSetName.Name.Region;
                    break;
                case "EIMSecurityClassification":
                    name = TermSetName.Name.SecurityClassification;
                    break;
                case "EIMSeismicSurvey":
                    name = TermSetName.Name.SeismicSurvey;
                    break;
                case "EIMSource":
                    name = TermSetName.Name.Source;
                    break;
                case "EIMStatus":
                    name = TermSetName.Name.Status;
                    break;
                case "EIMWell":
                    name = TermSetName.Name.Well;
                    break;
                case "EIMWellbore":
                    name = TermSetName.Name.Wellbore;
                    break;
                default:
                    return null;

            }

            return name.GetStringValue();
        }

    }
    public class EnumStringAttribute : Attribute
    {
        public EnumStringAttribute(string stringValue)
        {
            this.stringValue = stringValue;
        }
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    public static class ExtenstionClass
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            // Get the stringvalue attributes  
            EnumStringAttribute[] attribs = fieldInfo.GetCustomAttributes(
                 typeof(EnumStringAttribute), false) as EnumStringAttribute[];
            // Return the first if there was a match.  
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }
    }
}
