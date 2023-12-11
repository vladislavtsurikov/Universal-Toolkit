using System;
using System.Collections.Generic;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class EnumModel : BaseElement
    {
        public EnumModel(string name = null)
        {
            base.CustomDataType = Util.Enum;
            base.Name = name;
        }


        public new BuiltInDataType? BuiltInDataType { get; }

        public new string CustomDataType => base.CustomDataType;

        public new string Name => base.Name;

        public List<EnumValue> EnumValues { get; set; } = new List<EnumValue>();

        public override string ToString()
        {
            string result = base.ToString();
            result += Util.NewLine + Indent + "{";

            result += EnumValues.Count > 0 ? Util.NewLine : "";
            result += String.Join("," + Util.NewLine, EnumValues);
            result += Util.NewLine + Indent + "}";
            result += Util.NewLine;
            return result;
        }
    }

    public class EnumValue
    {
        public int IndentLevel = 2;
        public int IndentSize => IndentLevel * 4;
        public string Indent => new String(' ', IndentSize);

        public string Name { get; set; }

        public int? Value { get; set; }
        public string ValuFormated => Value != null ? " = " + Value : "";

        public override string ToString()
        {
            string result = Indent + Name + ValuFormated;
            return result;
        }
        
        public EnumValue(string name = null, int? value = null)
        {
            Name = name;
            Value = value;
        }
    }
}
