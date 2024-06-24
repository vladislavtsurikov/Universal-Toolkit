using System;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class ClassModel : BaseElement
    {
        public ClassModel(string name = null)
        {
            base.CustomDataType = Constants.Class;
            base.Name = name;
            Constructors.Add(new Constructor(Name) { IsVisible = false, BracesInNewLine = false });
        }
        
        public bool HasPropertiesSpacing { get; set; } = true;

        public new BuiltInDataType? BuiltInDataType { get; }

        public new string CustomDataType => Constants.Class;

        public new string Name => base.Name;

        public string BaseClass { get; set; }

        public List<string> Interfaces { get; set; } = new List<string>();

        public virtual List<Constructor> Constructors { get; set; } = new List<Constructor>();

        public virtual Constructor DefaultConstructor
        {
            get => Constructors[0];
            set => Constructors[0] = value;
        }
        
        public List<EnumModel> Enums { get; set; } = new List<EnumModel>();
        public virtual List<Field> Fields { get; set; } = new List<Field>();
        public virtual List<Property> Properties { get; set; } = new List<Property>();

        public virtual List<Method> Methods { get; set; } = new List<Method>();

        public virtual List<ClassModel> NestedClasses { get; set; } = new List<ClassModel>();
        // Nested indent have to be set for each Nested element and subelement separately, or after generation manualy to select nested code and indent it with tab
        // Setting it automaticaly and propagating could be done if the parent sets the child's parent reference (to itself) when the child is added/assigned to a parent. Parent setter is internal.
        //   http://softwareengineering.stackexchange.com/questions/261453/what-is-the-best-way-to-initialize-a-childs-reference-to-its-parent

        public override string ToString()
        {
            string result = base.ToString();
            result += (BaseClass != null || Interfaces?.Count > 0) ? $" : " : "";
            result += BaseClass != null ? BaseClass : "";
            result += (BaseClass != null && Interfaces?.Count > 0) ? $", " : "";
            result += Interfaces?.Count > 0 ? string.Join(", ", Interfaces) : "";
            result += Constants.NewLine + Indent + "{";
            
            result += String.Join(Constants.NewLine, Enums);
            result += String.Join("", Fields);

            var visibleConstructors = Constructors.Where(a => a.IsVisible);
            bool hasFieldsBeforeConstructor = visibleConstructors.Any() && Fields.Any();
            result += hasFieldsBeforeConstructor ? Constants.NewLine : "";
            result += String.Join(Constants.NewLine, visibleConstructors);
            bool hasMembersAfterConstructor = (visibleConstructors.Any() || Fields.Any()) && (Properties.Any() || Methods.Any());
            result += hasMembersAfterConstructor ? Constants.NewLine : "";

            result += String.Join(HasPropertiesSpacing ? Constants.NewLine : "", Properties);

            result += hasMembersAfterConstructor ? Constants.NewLine : "";
            result += String.Join(Constants.NewLine, Methods);

            for (int i = 0; i < NestedClasses.Count; i++)
            {
                NestedClasses[i].IncreaseIndentLevel();
                
                result += String.Join(Constants.NewLine, NestedClasses[i]);
                
                if(i != NestedClasses.Count - 1)
                    result += Constants.NewLine;
            }

            result += Constants.NewLine + Indent + "}";
            return result;
        }

        private void IncreaseIndentLevel()
        {
            IndentLevel++;
                
            foreach (var element in Fields)
            {
                element.IndentLevel = IndentLevel + 1;
            }
            
            foreach (var element in Properties)
            {
                element.IndentLevel = IndentLevel + 1;
            }
            
            foreach (var element in Methods)
            {
                element.IndentLevel = IndentLevel + 1;
            }
            
            foreach (var element in Enums) 
            {
                element.IndentLevel = IndentLevel + 1;
                
                foreach (var enumValue in element.EnumValues)
                {
                    enumValue.IndentLevel = element.IndentLevel + 1;
                }
            }
            
            foreach (var element in Constructors)
            {
                element.IndentLevel = IndentLevel + 1;
            }
        }
    }
}
