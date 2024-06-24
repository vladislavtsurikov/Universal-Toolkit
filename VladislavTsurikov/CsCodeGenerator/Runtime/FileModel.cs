using System;
using System.Collections.Generic;
using System.Text;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class FileModel
    {
        private List<string> UsingDirectives { get; set; } = new List<string>();
        
        public bool WarningGeneratedFile = true;

        public List<string> PreprocessorDirectives { get; set; } = new List<string>();

        public string Namespace { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; } = Constants.CsExtension;

        public string FullName => Name + "." + Extension;

        public List<EnumModel> Enums { get; set; } = new List<EnumModel>();

        public List<ClassModel> Classes { get; set; } = new List<ClassModel>();
        
        public FileModel(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            string result = WarningGeneratedFile ? GetWarningGeneratedFileText() : "";
            
            foreach (var preprocessorDirective in PreprocessorDirectives)
            {
                result += "#if " + preprocessorDirective + Constants.NewLine;
            }
            
            string usingText = UsingDirectives.Count > 0 ? Constants.Using + " " : "";
            result += usingText + String.Join(Constants.NewLine + usingText, UsingDirectives);
            result += Constants.NewLineDouble + Constants.Namespace + " " + Namespace;
            result += Constants.NewLine + "{";
            result += String.Join(Constants.NewLine, Enums);
            result += (Enums.Count > 0 && Classes.Count > 0) ? Constants.NewLine : "";
            result += String.Join(Constants.NewLine, Classes);
            result += Constants.NewLine + "}";
            result += Constants.NewLine;
            
            foreach (var preprocessorDirective in PreprocessorDirectives)
            {
                result += "#endif" + Constants.NewLine;
            }
            
            return result;
        }

        public void LoadUsingDirectives(List<string> usingDirectives)
        {
            foreach (var usingDirective in usingDirectives)
            {
                UsingDirectives.Add(usingDirective + ";");
            }
        }
        
        public void LoadUsingDirectives(params Type[] types)
        {
            foreach (var usingDirective in NamespaceUtility.GetUsingDirectives(types))
            {
                UsingDirectives.Add(usingDirective + ";");
            }
        }
        
        public void SetNamespaceFromFolder(string path, params string[] ignoredFolders)
        {
            Namespace = NamespaceUtility.GetNamespaceFromPath(path, "VladislavTsurikov", ignoredFolders);
        }

        private string GetWarningGeneratedFileText()
        {
            var namesStringBuilder = new StringBuilder();
            
            namesStringBuilder.AppendLine("//.........................");
            namesStringBuilder.AppendLine("//.....Generated File......");
            namesStringBuilder.AppendLine("//.........................");
            namesStringBuilder.AppendLine("//.......Do not edit.......");
            namesStringBuilder.AppendLine("//.........................");

            return namesStringBuilder + Constants.NewLine;
        }
    }
}
