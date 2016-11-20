using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public abstract class DatabaseModelItemXml : ConnectionItemXml, IAutoCategorize
    {
        

        public abstract ICollection<string> GetAutoCategories();

        public abstract string TypeName { get; }

        public virtual Dictionary<string, string> GetRegexMatch()
        {
            var dico = new Dictionary<string, string>();
            if (this is IModelSingleItemXml)
                dico.Add("sut:caption", ((IModelSingleItemXml)this).Caption);
            dico.Add("sut:typeName", TypeName);
            return dico;
        }
    }
}
