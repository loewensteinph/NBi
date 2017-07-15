using System;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core.FileManipulation;
using NBi.Xml.Settings;
using System.IO;
using NBi.Core.FolderManipulation;
using NBi.Core.Query;
using System.Collections.Generic;

namespace NBi.Xml.Decoration.Command
{
    public class FolderManipulationAbstractXml : DecorationCommandXml, IFolderManipulationCommand
    {
        [XmlAttribute("path")]
        public string Path { get; set; }
    }
}
