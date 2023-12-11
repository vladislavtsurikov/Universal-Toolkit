#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators
{
    internal static class EditorSelectableColorsGenerator
    {
        private static string TargetFilePath => $"{EditorPath.Path}/EditorUI";
        
        internal static void Run()
        {
            List<EditorDataSelectableColorPalette> groups = EditorDataSelectableColorPalette.GetGroups();

            foreach (EditorDataSelectableColorPalette group in groups)
            {
                group.Setup();
            }
            
            EditorDataSelectableColorPalette.RegenerateDefaultSelectableColorPalette();
            Run(groups); 
        }
        
        private static bool Run(List<EditorDataSelectableColorPalette> groups)
        {
            var usingDirectives = new List<string>
            {
                "UnityEngine;",
                "UnityEngine.UIElements;",
                "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI.ScriptableObjects.Colors;",
                "VladislavTsurikov.UIElements.Scripts.Color;"
            };
            
            ClassModel classModel = new ClassModel("EditorSelectableColors");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.paletteName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                var fields = new Field[]
                {
                    new Field(typeof(EditorDataSelectableColorPalette), "s_selectableColorPalette")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorDataSelectableColorPalette), "selectableColorPalette")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_selectableColorPalette != null? s_selectableColorPalette: s_selectableColorPalette = EditorDataSelectableColorPalette.GetGroup(\"{group.paletteName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(UnityEngine.Color), "GetColor")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter>
                        {
                            new Parameter("ColorName", "colorName"),
                            new Parameter("SelectionState", "state")
                        },
                        BodyLines = new List<string>
                        {
                            "return selectableColorPalette.GetColor(colorName.ToString(), state);"
                        }
                    },
                    
                    new Method(typeof(EditorThemeColor), "GetThemeColor")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter>
                        {
                            new Parameter("ColorName", "colorName"),
                            new Parameter("SelectionState", "state")
                        },
                        BodyLines = new List<string>
                        {
                            "return selectableColorPalette.GetThemeColor(colorName.ToString(), state);"
                        }
                    },
                    
                    new Method(typeof(EditorSelectableColorInfo), "GetSelectableColorInfo")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter> { new Parameter("ColorName", "colorName"), },
                        BodyLines = new List<string>
                        {
                            "return selectableColorPalette.GetSelectableColorInfo(colorName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("ColorName");

                foreach (var item in group.selectableColors)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.ColorName));
                    
                    Field field = new Field(typeof(EditorSelectableColorInfo), $"s_{item.ColorName}")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(EditorSelectableColorInfo), item.ColorName)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_{item.ColorName} ?? (s_{item.ColorName} = GetSelectableColorInfo(ColorName.{item.ColorName}))"
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
            
            FileModel fileModel = new FileModel("EditorSelectableColors");
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