using CefSharp;

namespace InternetArcade
{
    internal class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "cef";
        public const string SchemeNameTest = "test";

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new SchemeHandler(InternetArcade.Instance);
        }
    }
}
