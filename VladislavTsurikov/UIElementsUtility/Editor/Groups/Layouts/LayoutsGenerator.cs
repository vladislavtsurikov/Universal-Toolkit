#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Layouts
{
    public class LayoutsGenerator : DataGroupAPIGenerator<LayoutGroup, LayoutInfo>
    {
        protected override void Generate(List<LayoutGroup> groups)
        {
            ClassModel classModel = new ClassModel("GetLayout");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new List<ClassModel>();
            foreach (var group in groups)
            {
                ClassModel nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;
                
                string groupName = "LayoutGroup";
                
                var fields = new Field[]
                {
                    new Field(typeof(LayoutGroup), "s_layoutGroup")
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static, 
                    },
                }.ToList();

                var properties = new Property[]
                {
                    new Property(typeof(LayoutGroup), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_layoutGroup != null ? s_layoutGroup: s_layoutGroup = " +
                                       $"{nameof(DataGroupUtility)}.GetGroup<{nameof(LayoutGroup)}, {nameof(LayoutInfo)}>(\"{group.GroupName}\")"
                    },
                }.ToList();
                
                var methods = new Method[]
                {
                    new Method(typeof(VisualTreeAsset), "GetVisualTreeAsset")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new Parameter("LayoutName", "layoutName") },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetVisualTreeAsset(layoutName.ToString());"
                        }
                    },
                }.ToList();

                EnumModel enumModel = new EnumModel("LayoutName");

                foreach (var item in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.UxmlReference.name));
                    
                    string privateFieldItemName = $"s_{item.UxmlReference.name.ToLowerFirstChar()}";
                    
                    Field field = new Field(typeof(VisualTreeAsset), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                    };

                    Property property = new Property(typeof(VisualTreeAsset), item.UxmlReference.name)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = $"{privateFieldItemName} ? " +
                                     $"{privateFieldItemName} : " +
                                     $"{privateFieldItemName} = GetVisualTreeAsset(LayoutName.{item.UxmlReference.name})"
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
                typeof(DataGroupUtility),
                typeof(VisualTreeAsset),
                typeof(LayoutGroup));
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