#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    public class EditorColorsGenerator : DataGroupAPIGenerator<EditorColorPalette, EditorColorInfo>
    {
        protected override void Generate(List<EditorColorPalette> groups)
        {
            ClassModel classModel = new ClassModel("GetEditorColor");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                string groupName = "СolorPalette";
                
                var fields = new Field[]
                {
                    new Field(typeof(EditorColorPalette), "s_colorPalette")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorColorPalette), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = $"s_colorPalette != null? s_colorPalette: s_colorPalette = {nameof(DataGroupUtility)}.GetGroup<{nameof(EditorColorPalette)}, {nameof(EditorColorInfo)}>(\"{group.GroupName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(Color), "GetColor")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new Parameter("ColorName", "colorName"), },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetColor(colorName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("ColorName");

                foreach (var item in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.ColorName));
                    string privateFieldItemName = $"s_{item.ColorName.ToLowerFirstChar()}";
                    
                    Field field = new Field("Color?", privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(Color), item.ColorName)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = $"(Color) ({privateFieldItemName} ?? ({privateFieldItemName} = GetColor(ColorName.{item.ColorName})))"
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
            fileModel.PreprocessorDirectives.Add("UNITY_EDITOR");
            fileModel.LoadUsingDirectives(
                typeof(Color), 
                typeof(DataGroupUtility), 
                typeof(EditorColorPalette));
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