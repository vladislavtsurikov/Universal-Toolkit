#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Fonts;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators
{
    internal static class EditorFontsGenerator
    {
        private static string TargetFilePath => $"{EditorPath.Path}/EditorUI";

        internal static void Run()
        {
            List<EditorDataFontFamily> groups = EditorDataFontFamily.GetGroups();

            foreach (EditorDataFontFamily group in groups)
            {
                group.Setup();
            }

            Run(groups);
        }

        private static bool Run(List<EditorDataFontFamily> groups)
        {
            var usingDirectives = new List<string>
            {
                "UnityEngine;",
                "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI.ScriptableObjects.Fonts;"
            };

            ClassModel classModel = new ClassModel("EditorFonts");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.FontName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                var fields = new Field[]
                {
                    new Field(typeof(EditorDataFontFamily), "s_fontFamily")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorDataFontFamily), "fontFamily")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue =
                            $"s_fontFamily != null? s_fontFamily: s_fontFamily = EditorDataFontFamily.GetGroup(\"{group.FontName}\")"
                    },
                }.ToList();

                var methods = new Method[]
                {
                    new Method(typeof(Font), "GetFont")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter> { new Parameter("FontWeight", "weight") },
                        BodyLines = new List<string>
                        {
                            "return fontFamily.GetFont((int)weight);"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("FontWeight");

                foreach (var info in group.Fonts)
                {
                    enumModel.EnumValues.Add(new EnumValue(info.Weight.ToString(), (int)info.Weight));

                    Field field = new Field(typeof(Font), $"s_{info.Weight.ToString()}")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(Font), info.Weight.ToString())
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue =
                            $"s_{info.Weight.ToString()} ? s_{info.Weight.ToString()} : s_{info.Weight.ToString()} = GetFont(FontWeight.{info.Weight.ToString()})"
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

            FileModel fileModel = new FileModel("EditorFonts");
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