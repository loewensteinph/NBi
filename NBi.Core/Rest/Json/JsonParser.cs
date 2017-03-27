using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Rest.Json
{
    class JsonParser : IParser
    {
        public DataSet Parse(string content)
        {
            var token = JToken.Parse(content);
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            WalkNode(token, dataSet.Tables[0], null);
            return dataSet;
        }

        private void WalkNode(JToken node, object parent, string columnName)
        {
            switch (node.Type)
            {
                case JTokenType.Property:
                    WalkProperty(node, parent, columnName);
                    break;
                case JTokenType.Object:
                    WalkObject(node, parent, columnName);
                    break;
                case JTokenType.Array:
                    WalkArray(node, parent, columnName);
                    break;
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Date:
                case JTokenType.Boolean:
                    WalkValue(node, parent, columnName, node.Type);
                    break;
            }
        }

        private void WalkProperty(JToken node, object parent, string columnName)
        {
            var property = node as JProperty;
            var row = (parent as DataRow);
            if (!row.Table.Columns.Contains(property.Name))
                row.Table.Columns.Add(property.Name, typeof(object));
            foreach (var child in property.Children())
            {
                WalkNode(child, row, property.Name);
            }
        }

        private void WalkValue(JToken node, object parent, string columnName, JTokenType jsonType)
        {
            if (parent is DataRow)
            {
                var row = (parent as DataRow);
                row[columnName] = GetValue(jsonType, node);
            }
            else if (parent is List<object>)
            {
                var list = (parent as List<object>);
                list.Add(GetValue(jsonType, node));
            }
        }

        private object GetValue(JTokenType type, JToken node)
        {
            switch (type)
            {
                case JTokenType.Integer:
                    return node.Value<Int32>();
                case JTokenType.Float:
                    return node.Value<float>();
                case JTokenType.String:
                    return node.Value<String>();
                case JTokenType.Boolean:
                    return node.Value<Boolean>();
                case JTokenType.Date:
                    return node.Value<DateTime>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WalkObject(JToken node, object parent, string columnName)
        {
            DataTable table;
            if (parent is DataRow)
            {
                table = new DataTable(columnName);
                (parent as DataRow)[columnName] = table;
            }
            else
            {
                table = (parent as DataTable);
            }

            var row = table.NewRow();
            table.Rows.Add(row);
            foreach (JToken child in node.Children())
            {
                WalkNode(child, row, null);
            }
        }

        private void WalkArray(JToken node, object parent, string columnName)
        {
            object dt = new DataTable(columnName);
            if (parent is DataSet)
            {
                (parent as DataSet).Tables.Add(dt as DataTable);
            }

            else if (parent is DataTable)
            {
                dt = parent;
            }

            else if (parent is DataRow)
            {
                if (node.Children().Any(x => x.Type == JTokenType.Object))
                    (parent as DataRow)[columnName] = dt;
                else
                {
                    dt = new List<object>();
                    (parent as DataRow)[columnName] = dt;
                }
            }

            foreach (JToken child in node.Children())
            {
                WalkNode(child, dt, columnName);
            }
        }
    }
}

