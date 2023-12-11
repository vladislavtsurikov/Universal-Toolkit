
namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console
{
    public sealed class PrototypeLog
    {
        public string Header;
        public string Text;
        public bool Error;

        public PrototypeLog(string header, string text, bool error = true)
        {
            Header = header;
            Text = text;
            Error = error;
        }
    }
}