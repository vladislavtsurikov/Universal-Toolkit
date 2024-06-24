﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public abstract class BaseElement
    {
        public int IndentLevel = 1;

        public int IndentSize => IndentLevel * 4;

        public string Indent => new String(' ', IndentSize);

        public virtual string Comment { get; set; }

        public virtual bool HasAttributes => true;
        public virtual List<AttributeModel> Attributes { get; set; } = new List<AttributeModel>();

        public virtual AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        protected string AccessFormatted => Indent + AccessModifier.ToTextLower() + " ";

        public List<KeyWord> KeyWords { get; set; } = new List<KeyWord>();
        public KeyWord SingleKeyWord { set => KeyWords.Add(value); }
        protected string KeyWordsFormated => KeyWords?.Count > 0 ? String.Join(" ", KeyWords.Select(a => a.ToTextLower())) + " " : "";

        public virtual BuiltInDataType? BuiltInDataType { get; set; }
        public virtual string CustomDataType { get; set; }
        protected string DataTypeFormated => CustomDataType ?? BuiltInDataType?.ToTextLower();

        public virtual string Name { get; set; }

        public virtual string Signature => $"{AccessFormatted}{KeyWordsFormated}{DataTypeFormated} {Name}";

        public void AddAttribute(AttributeModel attributeModel) => Attributes?.Add(attributeModel);
        
        public BaseElement() { }

        public BaseElement(BuiltInDataType builtInDataType, string name)
        {
            BuiltInDataType = builtInDataType;
            Name = name;
        }

        public BaseElement(Type customDataType, string name)
        {
            CustomDataType = customDataType.ToString().Split('.').Last();
            Name = name;
        }
        
        public BaseElement(string customDataType, string name)
        {
            CustomDataType = customDataType;
            Name = name;
        }

        public override string ToString()
        {
            string result = Comment != null ? (Constants.NewLine + Indent + "// " + Comment) : "";
            result += HasAttributes ? Attributes.ToStringList(Indent) : "";
            result += Constants.NewLine + Signature;
            return result;
        }
    }
}
