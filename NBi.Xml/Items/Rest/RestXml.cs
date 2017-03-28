using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core;
using NBi.Core.Report;
using NBi.Xml.Settings;
using NBi.Xml.Constraints;

namespace NBi.Xml.Items.Rest
{
    public class RestXml : ExecutableXml, IReferenceFriendly
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlElement("location")]
        public LocationXml Location { get; set; }
        
        [XmlElement("Credentials")]
        public CredentialsXml Credentials { get; set; }

        [XmlElement("parameter")]
        public List<RestParameterXml> Parameters { get; set; }

        public RestXml()
        {
            Location = LocationXml.Blank;
            Credentials = CredentialsXml.Blank;
            Parameters = new List<RestParameterXml>();
        }
        

        public void AssignReferences(IEnumerable<ReferenceXml> references)
        {
            //if (Location == LocationXml.Default)
            //    Location = Default.RestApi.Location;
            //if (Credentials == CredentialsXml.Default)
            //    Credentials = Default.RestApi.Credentials;

            //if (!string.IsNullOrEmpty(Location.BaseAddress) && Location.BaseAddress.StartsWith("@"))
            //    Location.BaseAddress = InitializeFromReferences(references, Location.BaseAddress, "base-address");
            //if (Location.ContentType.StartsWith("@"))
            //    Location.ContentType = InitializeFromReferences(references, Location.ContentType, "content-type");
            //if (Credentials.Reference.StartsWith("@"))
            //    Path = InitializeFromReferences(references, Path, "path");
        }

        protected string InitializeFromReferences(IEnumerable<ReferenceXml> references, string refName, string attribute)
        {
            //if (refName.StartsWith("@"))
            //    refName = refName.Substring(1);

            //var refChoice = GetReference(references, refName);

            //if (refChoice.Report == null)
            //    throw new NullReferenceException(string.Format("A reference named '{0}' has been found, but no element 'report' has been defined", refName));

            //if (attribute=="source")
            //    return refChoice.Report.Source;
            //if (attribute=="path")
            //    return refChoice.Report.Path;
            throw new ArgumentOutOfRangeException();
        }

        protected ReferenceXml GetReference(IEnumerable<ReferenceXml> references, string value)
        {
            if (references == null || references.Count() == 0)
                throw new InvalidOperationException("No reference has been defined for this constraint");

            var refChoice = references.FirstOrDefault(r => r.Name == value);
            if (refChoice == null)
                throw new IndexOutOfRangeException(string.Format("No reference named '{0}' has been defined.", value));
            return refChoice;
        }
    }
}
