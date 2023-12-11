#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators
{
    internal static class EditorColorsGenerator
    {
        private static string TargetFilePath => $"{EditorPath.Path}/EditorUI";
        
        internal static void Run()
        {
            List<EditorDataColorPalette> groups = EditorDataColorPalette.GetGroups();

            foreach (EditorDataColorPalette group in groups)
            {
                group.Setup();
            }

            EditorDataColorPalette.RegenerateDefaultColorPalette();
            Run(groups); 
        }
        
        private static bool Run(List<EditorDataColorPalette> groups)
        {
            var usingDirectives = new List<string>
            {
                "System;",
                "UnityEngine;",
                "VladislavTsurikov.Extensions.Scripts;",
                "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI.ScriptableObjects.Colors;"
            };
            
            ClassModel classModel = new ClassModel("EditorColors");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.paletteName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                var fields = new Field[]
                {
                    new Field(typeof(EditorDataColorPalette), "s_colorPalette")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorDataColorPalette), "colorPalette")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_colorPalette != null? s_colorPalette: s_colorPalette = EditorDataColorPalette.GetGroup(\"{group.paletteName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(UnityEngine.Color), "GetColor")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter> { new Parameter("ColorName", "colorName"), },
                        BodyLines = new List<string>
                        {
                            "return colorPalette.GetColor(colorName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("ColorName");

                foreach (var item in group.colors)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.ColorName));
                    
                    Field field = new Field("Color?", $"s_{item.ColorName}")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(UnityEngine.Color), item.ColorName)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"(Color) (s_{item.ColorName} ?? (s_{item.ColorName} = GetColor(ColorName.{item.ColorName})))"
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
            
            FileModel fileModel = new FileModel("EditorColors");
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