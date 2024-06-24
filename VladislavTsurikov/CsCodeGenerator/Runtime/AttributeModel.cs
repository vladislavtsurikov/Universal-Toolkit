using System;
using System.Collections.Generic;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class AttributeModel
    {
        public AttributeModel(string name = null)
        {
            Name = name;
        }

        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public Parameter SingleParameter { set => Parameters.Add(value); }

        public override string ToString()
        {
            string parametersString = Parameters.Count > 0 ? Parameters.ToStringList() : "";
            string result = $"[{Name}{parametersString}]";
            return result;
        }
    }

    public static class AttributeModelExtensions
    {
        public static string ToStringList(this List<AttributeModel> attributes, string indent)
        {
            string result = attributes.Count > 0 ? Constants.NewLine + indent + String.Join(Constants.NewLine + indent, attributes) : "";
            return result;
        }
    }
}
