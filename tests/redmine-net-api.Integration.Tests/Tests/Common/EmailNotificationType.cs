namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;

public sealed record EmailNotificationType
{
    public static readonly EmailNotificationType OnlyMyEvents = new EmailNotificationType(1, "only_my_events");
    public static readonly EmailNotificationType OnlyAssigned = new EmailNotificationType(2, "only_assigned");
    public static readonly EmailNotificationType OnlyOwner = new EmailNotificationType(3, "only_owner");
    public static readonly EmailNotificationType None = new EmailNotificationType(0, "");

    public int Id { get; }
    public string Name { get; }

    private EmailNotificationType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static EmailNotificationType FromId(int id)
    {
        return id switch
        {
            1 => OnlyMyEvents,
            2 => OnlyAssigned,
            3 => OnlyOwner,
            _ => None
        };
    }
}