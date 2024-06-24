#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Styles;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Styles
{
    public class StylesGenerator : DataGroupAPIGenerator<StyleGroup, StyleInfo>
    {
        protected override void Generate(List<StyleGroup> groups)
        {
            ClassModel classModel = new ClassModel("GetStyle");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (StyleGroup group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                string groupName = "StyleGroup";
                
                var fields = new Field[]
                {
                    new Field(typeof(StyleGroup), "s_styleGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(StyleGroup), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_styleGroup != null? s_styleGroup: s_styleGroup = " +
                                     $"{nameof(DataGroupUtility)}.GetGroup<{nameof(StyleGroup)}, {nameof(StyleInfo)}>(\"{group.GroupName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(StyleSheet), "GetStyleSheet")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new Parameter("StyleName", "styleName") },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetStyleSheet(styleName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("StyleName");

                foreach (var style in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(style.UssReference.name));
                    string privateFieldItemName = $"s_{style.UssReference.name.ToLowerFirstChar()}";
                    
                    Field field = new Field(typeof(StyleSheet), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(StyleSheet), style.UssReference.name)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = $"{privateFieldItemName} ? {privateFieldItemName} : {privateFieldItemName} = GetStyleSheet(StyleName.{style.UssReference.name})"
                    };
                    
                    fields.Add(field);
                    properties.Add(property);
                }

                nestedClass.Fields = fields;
                nestedClass.Properties = properties;
                nestedClass.Methods = methods;
                nestedClass.Enums.Add(enumModel);
                
                nestedClasses.Add(nestedClass);
            }
            
            classModel.NestedClasses.AddRange(nestedClasses);
            
            FileModel fileModel = new FileModel(classModel.Name);
            fileModel.LoadUsingDirectives(
                typeof(StyleGroup),
                typeof(StyleSheet),
                typeof(DataGroupUtility));
            fileModel.SetNamespaceFromFolder(TargetFilePath, "Assets", "Runtime", "API");
            fileModel.Classes.Add(classModel);
            
            CsGenerator csGenerator = new CsGenerator();
            csGenerator.Files.Add(fileModel);
            csGenerator.Path = TargetFilePath;
            csGenerator.CreateFiles();
        }
    }
}
#endif