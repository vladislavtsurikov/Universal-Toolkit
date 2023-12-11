using System;
using System.Collections.Generic;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class InterfaceModel : BaseElement
    {
        public InterfaceModel(string name = null)
        {
            base.CustomDataType = Util.Interface;
            base.Name = name;
        }
        
        public new BuiltInDataType? BuiltInDataType { get; }

        public new string CustomDataType => base.CustomDataType;

        public new string Name => base.Name;

        public virtual List<Property> Properties { get; set; } = new List<Property>();

        public virtual List<Method> Methods { get; set; } = new List<Method>();

        public override string ToString()
        {
            string result = base.ToString();
            result += Util.NewLine + Indent + "{";

            result += String.Join("", Properties);
            bool hasPropertiesAndMethods = Properties.Count > 0 && Methods.Count > 0;
            result += hasPropertiesAndMethods ? Util.NewLine : "";
            result += String.Join(Util.NewLine, Methods);

            result += Util.NewLine + Indent + "}";
            result = result.Replace(AccessModifier.Public.ToTextLower() + " ", "");
            result = result.Replace("\r\n        {\r\n        }", ";");
            return result;
        }
    }
}
