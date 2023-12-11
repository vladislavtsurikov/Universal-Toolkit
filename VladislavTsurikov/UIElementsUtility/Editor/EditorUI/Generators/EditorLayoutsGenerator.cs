#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;
using EditorDataLayoutGroup = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Layouts.EditorDataLayoutGroup;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators
{
    internal static class EditorLayoutsGenerator
    { 
        private static string TargetFilePath => $"{EditorPath.Path}/EditorUI";
        
        internal static void Run()
        {
            List<EditorDataLayoutGroup> groups = EditorDataLayoutGroup.GetGroups();

            foreach (EditorDataLayoutGroup group in groups)
            {
                group.Setup();
            }

            Run(groups); 
        }

        private static bool Run(List<EditorDataLayoutGroup> groups)
        {
            var usingDirectives = new List<string>
            {
                "UnityEngine.UIElements;",
                "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI.ScriptableObjects.Layouts;"
            };
            
            ClassModel classModel = new ClassModel("EditorLayouts");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                var fields = new Field[]
                {
                    new Field(typeof(EditorDataLayoutGroup), "s_layoutGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorDataLayoutGroup), "layoutGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_layoutGroup != null? s_layoutGroup: s_layoutGroup = EditorDataLayoutGroup.GetGroup(\"{group.GroupName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(VisualTreeAsset), "GetVisualTreeAsset")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter> { new Parameter("LayoutName", "layoutName") },
                        BodyLines = new List<string>
                        {
                            "return layoutGroup.GetVisualTreeAsset(layoutName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("LayoutName");

                foreach (var layoutInfo in group.Layouts)
                {
                    enumModel.EnumValues.Add(new EnumValue(layoutInfo.UxmlReference.name));
                    
                    Field field = new Field(typeof(VisualTreeAsset), $"s_{layoutInfo.UxmlReference.name}")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(VisualTreeAsset), layoutInfo.UxmlReference.name)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_{layoutInfo.UxmlReference.name} ? s_{layoutInfo.UxmlReference.name} : s_{layoutInfo.UxmlReference.name} = GetVisualTreeAsset(LayoutName.{layoutInfo.UxmlReference.name})"
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
            
            FileModel fileModel = new FileModel("EditorLayouts");
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