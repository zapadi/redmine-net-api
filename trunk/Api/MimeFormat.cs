namespace Redmine.Net.Api
{
    public enum MimeFormat
    {
        xml
        //#if RUNNING_ON_35_OR_ABOVE
        , json
        //#endif
    }
}