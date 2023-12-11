#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;
using EditorDataStyleGroup = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Styles.EditorDataStyleGroup;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators
{
    internal static class EditorStylesGenerator
    {
        private static string TargetFilePath => $"{EditorPath.Path}/EditorUI";

        internal static void Run()
        {
            List<EditorDataStyleGroup> groups = EditorDataStyleGroup.GetGroups();

            foreach (EditorDataStyleGroup group in groups)
            {
                group.Setup();
            }

            Run(groups); 
        }
        
        private static bool Run(List<EditorDataStyleGroup> groups)
        {
            var usingDirectives = new List<string>
            {
                "UnityEngine.UIElements;",
                "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI.ScriptableObjects.Styles;"
            };
            
            ClassModel classModel = new ClassModel("EditorStyles");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (EditorDataStyleGroup group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                var fields = new Field[]
                {
                    new Field(typeof(EditorDataStyleGroup), "s_styleGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorDataStyleGroup), "styleGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_styleGroup != null? s_styleGroup: s_styleGroup = EditorDataStyleGroup.GetGroup(\"{group.GroupName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(StyleSheet), "GetStyleSheet")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter> { new Parameter("StyleName", "styleName") },
                        BodyLines = new List<string>
                        {
                            "return styleGroup.GetStyleSheet(styleName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("StyleName");

                foreach (var style in group.Styles)
                {
                    enumModel.EnumValues.Add(new EnumValue(style.UssReference.name));
                    
                    Field field = new Field(typeof(StyleSheet), $"s_{style.UssReference.name}")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(StyleSheet), style.UssReference.name)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_{style.UssReference.name} ? s_{style.UssReference.name} : s_{style.UssReference.name} = GetStyleSheet(StyleName.{style.UssReference.name})"
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
            
            FileModel fileModel = new FileModel("EditorStyles");
            fileModel.PreprocessorDirectives.Add("UNITY_EDITOR");
            fileModel.LoadUsingDirectives(usingDirectives);
            fileModel.Namespace = "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI";
            fileModel.Classes.Add(classModel);
            
            CsGenerator csGenerator = new CsGenerator();
            csGenerator.Files.Add(fileModel);
            csGenerator.Path = TargetFilePath;
            csGenerator.CreateFiles();

            return true;
        }
    }
}
#endif