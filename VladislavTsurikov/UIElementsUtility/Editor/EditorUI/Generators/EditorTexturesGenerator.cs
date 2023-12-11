#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.CsCodeGenerator.Runtime.Enums;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Textures;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators
{
    internal static class EditorTexturesGenerator
    {
        private static string TargetFilePath => $"{EditorPath.Path}/EditorUI";
        
        internal static void Run()
        {
            List<EditorDataTextureGroup> groups = EditorDataTextureGroup.GetGroups();

            foreach (EditorDataTextureGroup group in groups)
            {
                group.Setup();
            }

            Run(groups); 
        }
        
        private static bool Run(List<EditorDataTextureGroup> groups)
        {
            var usingDirectives = new List<string>
            {
                "UnityEngine;",
                "VladislavTsurikov.UIElements.ScriptsEditor.EditorUI.ScriptableObjects.Textures;"
            };
            
            ClassModel classModel = new ClassModel("EditorTextures");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                var fields = new Field[]
                {
                    new Field(typeof(EditorDataTextureGroup), "s_textureGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(EditorDataTextureGroup), "textureGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_textureGroup != null? s_textureGroup: s_textureGroup = EditorDataTextureGroup.GetGroup(\"{group.GroupName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(Texture2D), "GetTexture2D")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        Parameters = new List<Parameter> { new Parameter("TextureName", "textureName"), },
                        BodyLines = new List<string>
                        {
                            "return textureGroup.GetTexture(textureName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("TextureName");

                foreach (var item in group.Textures)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.TextureName));
                    
                    Field field = new Field(typeof(Texture2D), $"s_{item.TextureName}")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(Texture2D), item.TextureName)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        DefaultValue = $"s_{item.TextureName} ? s_{item.TextureName} : s_{item.TextureName} = GetTexture2D(TextureName.{item.TextureName})"
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
            
            FileModel fileModel = new FileModel("EditorTextures");
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