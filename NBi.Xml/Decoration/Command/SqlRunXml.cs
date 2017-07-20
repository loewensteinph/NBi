using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core.Etl;
using NBi.Xml.Items;
using System.IO;
using NBi.Core.Batch;
using NBi.Xml.Settings;
using System.ComponentModel;
using System.Data;
using System.Text;
using NBi.Core;
using NBi.Xml.SerializationOption;
using NBi.Core.Query;

namespace NBi.Xml.Decoration.Command
{
    public class SqlRunXml : DecorationCommandXml, IBatchRunCommand
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("path")]
        public string InternalPath { get; set; }

        [XmlIgnore]
        private string inlineQuery;

        [XmlIgnore]
        public CData InlineQueryWrite
        {
            get { return inlineQuery; }
            set { inlineQuery = value; }
        }

        [XmlText]
        public string InlineQuery
        {
            get { return inlineQuery; }
            set { inlineQuery = value; }
        }

        [XmlIgnore]
        public string FullPath
        {
            get
            {
                var fullPath = string.Empty;
                if (!Path.IsPathRooted(InternalPath) || string.IsNullOrEmpty(Settings.BasePath))
                    fullPath = InternalPath + Name;
                else
                    fullPath = Settings.BasePath + InternalPath + Name;

                return fullPath;
            }
        }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("connectionString")]
        public string SpecificConnectionString { get; set; }

        [XmlIgnore]
        public string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(SpecificConnectionString) && SpecificConnectionString.StartsWith("@"))
                    return Settings.GetReference(SpecificConnectionString.Remove(0, 1)).ConnectionString;
                if (!String.IsNullOrWhiteSpace(SpecificConnectionString))
                    return SpecificConnectionString;
                if (Settings != null && Settings.GetDefault(SettingsXml.DefaultScope.Decoration) != null)
                    return Settings.GetDefault(SettingsXml.DefaultScope.Decoration).ConnectionString;
                return string.Empty;
            }
        }

        IEnumerable<IQueryTemplateVariable> IBatchRunCommand.Variables
        { get { return GetVariables(); } set { throw new NotImplementedException(); } }

        public SqlRunXml()
        {
            Variables = new List<QueryTemplateVariableXml>();
            Version = "SqlServer2014";
        }

        public string GetQuery()
        {
            //if Sql is specified then return it
            if (InlineQuery != null && !string.IsNullOrEmpty(InlineQuery))
                return InlineQuery;

            if (string.IsNullOrEmpty(FullPath))
                throw new InvalidOperationException("Element query must contain a query or a file!");

            //Else check that file exists and read the file's content
            var file = string.Empty;
            if (Path.IsPathRooted(FullPath))
                file = FullPath;
            if (!System.IO.File.Exists(file))
                throw new ExternalDependencyNotFoundException(file);
            var query = System.IO.File.ReadAllText(file, Encoding.UTF8);
            return query;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual IDbCommand GetCommand()
        {
            var conn = new ConnectionFactory().Get(ConnectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = GetQuery();

            return cmd;
        }
    }
}
