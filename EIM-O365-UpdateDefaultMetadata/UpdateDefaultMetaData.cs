using EIM_O365_MetaDataTermSet;
//using Microsoft.Graph;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_UpdateDefaultMetaData
{
    public class UpdateDefaultMetaData
    {
        public static List<Log> UpdateSite(ClientContext ctx, DefaultFields defaultFields)
        {
            const string TaxonomyFieldType = "TaxonomyFieldType";
            const string TaxonomyFieldTypeMulti = "TaxonomyFieldTypeMulti";
            Web web = ctx.Web;
            try
            {
                Log.ClearLog();
                ctx.Load(web);

                FieldCollection fields = web.Fields;
                ctx.Load(fields, tcol => tcol.Where(t => t.TypeAsString == TaxonomyFieldType || t.TypeAsString == TaxonomyFieldTypeMulti));
                ctx.ExecuteQuery();

                /// Update Columns

                foreach (var column in defaultFields)
                {
                    Field field = GetField(column.InternalName, fields);
                    if (field == null)
                    {
                        Log.Error(web.Url, "", column.InternalName, "Invalid Field Name");
                    }
                    else
                    {
                        TaxonomyField taxonomyField = ctx.CastTo<TaxonomyField>(field);
                        if (taxonomyField.AllowMultipleValues)
                        {
                            TaxonomyFieldValueCollection taxonomyFieldValues = new TaxonomyFieldValueCollection(ctx, "", field);
                            taxonomyFieldValues.PopulateFromLabelGuidPairs(column.TermLabel + "|" + column.Guid);

                            var validatedValues = taxonomyField.GetValidatedString(taxonomyFieldValues);
                            ctx.ExecuteQuery();
                            taxonomyField.DefaultValue = validatedValues.Value;
                        }
                        else
                        {
                            TaxonomyFieldValue taxonomyFieldValue = new TaxonomyFieldValue();
                            taxonomyFieldValue.WssId = -1;
                            taxonomyFieldValue.Label = column.TermLabel;
                            taxonomyFieldValue.TermGuid = column.Guid;
                            var validatedValue = taxonomyField.GetValidatedString(taxonomyFieldValue);
                            ctx.ExecuteQuery();
                            taxonomyField.DefaultValue = validatedValue.Value;
                        }

                        taxonomyField.UpdateAndPushChanges(true);
                        ctx.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(web.Url, "", "", ex.Message);
            }
            return Log.GetLog();

        }
        static Field GetField(string internalName, FieldCollection fields)
        {
            foreach (Field field in fields)
            {
                if (internalName == field.InternalName)
                {
                    return field;
                }
            }
            return null;
        }


        public static List<Log> Update(ClientContext ctx, DefaultFields defaultFields)
        {
            //return Update(ctx, defaultFields, true);
        //}
        //static public List<Log> Update(ClientContext ctx, DefaultFields defaultFields, bool pushToLibrary)
        //{
            Web web = ctx.Web;
            try
            {
                Log.ClearLog();
                WebCollection webs = web.Webs;
                ListCollection lists = web.Lists;
                FieldCollection fields = web.Fields;
                ctx.Load(web);
                ctx.Load(webs);
                ctx.Load(lists, doclist => doclist.Where(doc => doc.BaseTemplate == 101));
                //ctx.Load(fields, tcol => tcol.Include(t => t.InternalName, t => t.DefaultValue), tcol => tcol.Where(t => t.TypeAsString == "TaxonomyFieldType" || t.TypeAsString == "TaxonomyFieldTypeMulti"));
                ctx.Load(fields,  tcol => tcol.Where(t => t.TypeAsString == "TaxonomyFieldType" || t.TypeAsString == "TaxonomyFieldTypeMulti"));
                ctx.ExecuteQuery();
                defaultFields = UpdateSite(ctx, web, fields, defaultFields, false);
                foreach (List list in lists)
                {
                    UpdateList(ctx, web, list.Title, defaultFields);
                }


                foreach (Web subweb in webs)
                {
                    ListCollection subwebLists = subweb.Lists;
                    ctx.Load(subwebLists, doclist => doclist.Where(dl => dl.BaseTemplate == 101));
                    ctx.ExecuteQuery();
                    foreach (List list in subwebLists)
                    {
                        //Console.WriteLine(subweb.Title + " " + list.Title);
                        UpdateList(ctx, subweb, list.Title, defaultFields);

                    }
                }
                // Push update of status field, as users isn't able to set column default value on library without it, due to some strange behavior from Microsoft
                PushStatusFieldUpdate(ctx, web, fields);
            }
            catch (Exception ex)
            {
                Log.Error(ctx.Web.Url, "", "", ex.Message);

            }
            return Log.GetLog();

        }
        static DefaultFields UpdateSite(ClientContext ctx, Web web,FieldCollection fields, DefaultFields defaultFields, bool newLibrary)
        {
            // If newLibrary just get default values from web and fill the DefaultFields
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ctx);
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();

            foreach (var df in defaultFields)
            {
                try
                {
                    Field field = fields.GetFieldByInternalName(df.InternalName);
                    if (field != null)
                    {
                        
                        if (!String.IsNullOrEmpty(field.DefaultValue))
                        {
                            string[] guid = field.DefaultValue.Split('|');
                            if (guid.Length > 0)
                            {
                                df.OriginGuid = guid[1];
                                if(newLibrary)
                                {
                                    df.Guid = guid[1];
                                }
                            }
                        }
                        string termSetGuid = TermSetName.GetTermSetGuidByInternalName(df.InternalName);
                        TermSet termSet = termStore.GetTermSet(new Guid(termSetGuid));
                        Term term = termSet.GetTerm(new Guid(df.Guid));
                        ctx.Load(term);

                        if (newLibrary)
                        {
                            ctx.ExecuteQuery();
                            df.TermItem = term;
                            Log.Info(web.Url, "", df.InternalName, df.Guid);
                        }
                        else
                        {
                            bool multiValue;
                            TaxonomyField taxField = ctx.CastTo<TaxonomyField>(field);
                            try
                            {
                                multiValue = taxField.AllowMultipleValues;
                            }
                            catch 
                            {
                                multiValue = false;
                            }
                            if (multiValue)
                            {
                                string defaultValue = "-1;#" + df.TermLabel + "|" + df.Guid;
                                //TaxonomyFieldValueCollection defaultValue = new TaxonomyFieldValueCollection()
                                //defaultValue.WssId = -1;
                                //defaultValue.Label = df.TermLabel;
                                //defaultValue.TermGuid = df.Guid;

                                //var validatedValue = taxField.GetValidatedString(defaultValue);
                                //ctx.ExecuteQuery();
                                taxField.DefaultValue = defaultValue;

                                taxField.Update();
                                ctx.ExecuteQuery();
                                df.TermItem = term;
                                Log.Info(web.Url, "", df.InternalName, defaultValue);

                            }
                            else
                            {
                                TaxonomyFieldValue defaultValue = new TaxonomyFieldValue();
                                defaultValue.WssId = -1;
                                defaultValue.Label = df.TermLabel;
                                defaultValue.TermGuid = df.Guid;

                                var validatedValue = taxField.GetValidatedString(defaultValue);
                                ctx.ExecuteQuery();
                                taxField.DefaultValue = validatedValue.Value;

                                taxField.Update();
                                ctx.ExecuteQuery();
                                df.TermItem = term;
                                Log.Info(web.Url, "", df.InternalName, validatedValue.Value);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(web.Url, "", df.InternalName, ex.Message);
                }
            }
           
            return defaultFields;

        }
        static void PushStatusFieldUpdate(ClientContext ctx, Web web, FieldCollection fields) {
            // Push update of status field, as users isn't able to set column default value on library without it, due to some strange behavior from Microsoft

            try {
                Field statusField = fields.GetFieldByInternalName("EIMStatus");
                if (statusField == null) return; //Check if status field has been found

                TaxonomyField taxStatusField = ctx.CastTo<TaxonomyField>(statusField);
                taxStatusField.UpdateAndPushChanges(true);
                ctx.ExecuteQuery();
            }
            catch (Exception ex) {

                Log.Error(web.Url, "", "EIMStatus", ex.Message);
            }
        }
        static void UpdateList(ClientContext ctx, Web web, string listTitle, DefaultFields defaultFields)
        {
            List list = web.GetListByTitle(listTitle);
            FieldCollection fields = list.Fields;
            ctx.Load(fields, tcol => tcol.Include(t => t.InternalName), tcol => tcol.Where(t => t.TypeAsString == "TaxonomyFieldType" || t.TypeAsString == "TaxonomyFieldTypeMulti"));
            ctx.ExecuteQuery();

            List<IDefaultColumnValue> defaultValues = new List<IDefaultColumnValue>();

            var currentDefaultValues = list.GetDefaultColumnValues();

            string[] guid;
            string[] current;
            foreach (var newDefaultValue in defaultFields)
            {
                try
                {
                    Field found = fields.GetFieldByInternalName(newDefaultValue.InternalName);
                    if (found != null)
                    {
                        bool update = true;

                        if (currentDefaultValues != null)
                        {
                            foreach (var currentDefaultValue in currentDefaultValues)
                            {
                                current = currentDefaultValue.Values.ToArray();
                                if (newDefaultValue.InternalName == current[1])
                                {
                                    guid = current[2].Split('|');
                                    //{[Value, -1;#Internal|3361fef0-33ac-457d-8a1d-df19735ffcb1]}
                                    if (guid.Length > 0)
                                    {
                                        if (newDefaultValue.OriginGuid != guid[1])
                                        {
                                            update = false;
                                        }
                                    }
                                }
                            }
                        }
                        if (update)
                        {
                            var defaultColumnValue = new DefaultColumnTermValue();
                            defaultColumnValue.FieldInternalName = newDefaultValue.InternalName;

                            defaultColumnValue.FolderRelativePath = "/";
                            defaultColumnValue.Terms.Add(newDefaultValue.TermItem);
                            defaultValues.Add(defaultColumnValue);
                            Log.Info(web.Url, list.Title, newDefaultValue.InternalName,"");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(web.Url, list.Title, newDefaultValue.InternalName, ex.Message);
                }
            }
            if (defaultValues.Count > 0)
            {
                try
                {
                    list.SetDefaultColumnValues(defaultValues);
                }
                catch(Exception ex)
                {
                    Log.Error(web.Url, list.Title, "", ex.Message);
                }
            }
        }
        public static List<Log> UpdateNewLibrary(ClientContext ctx, string listTitle)
        {
            Web web = ctx.Web;
            try
            {
                Log.ClearLog();
                List list = web.Lists.GetByTitle(listTitle);
                FieldCollection fields = list.Fields;
                ctx.Load(web);
                ctx.Load(list);
                ctx.Load(fields, tcol => tcol.Include(t => t.InternalName, t => t.DefaultValue), tcol => tcol.Where(t => t.TypeAsString == "TaxonomyFieldType" || t.TypeAsString == "TaxonomyFieldTypeMulti"));
                ctx.ExecuteQuery();
                DefaultFields defaultFields = new DefaultFields();
                DefaultField defaultField;
                foreach(Field  field in fields)
                {
                    defaultField = new DefaultField();
                    defaultField.InternalName = field.InternalName;
                    defaultFields.Add(defaultField);
                }
                defaultFields = UpdateSite(ctx, web, fields, defaultFields, true);
                // Remove fields with empty defaultvalues
                DefaultFields dfs = new DefaultFields();
                DefaultField df;
                
                foreach (DefaultField field in defaultFields)
                {
                    if (!String.IsNullOrEmpty(field.Guid))
                    {
                        df = new DefaultField();
                        df.Guid = field.Guid;
                        df.InternalName = field.InternalName;
                        df.OriginGuid = field.OriginGuid;
                        df.TermItem = field.TermItem;
                        dfs.Add(df);
                    }
                }
                UpdateList(ctx, web, list.Title, dfs);

            }
            catch (Exception ex)
            {
                Log.Error(ctx.Web.Url, "", "", ex.Message);

            }
            return Log.GetLog();

        }


    }
}
