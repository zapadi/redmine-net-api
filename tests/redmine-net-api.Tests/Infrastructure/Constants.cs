namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

public static class Constants
{
    public const string DeserializeCollection = "DeserializeCollection";
    public const string JsonNewtonsoft = "JsonNewtonsoft";
    public const string JsonSystemText = "JsonSystemText";
    public const string Xml = "Xml";

    public static class ConditionalCompilationSymbol
    {
        public  const string DebugXml = "DEBUG_XML";
        public  const string DebugJson = "DEBUG_JSON";
    }
}