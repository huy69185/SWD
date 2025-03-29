namespace ChildApi.Application.Messaging
{
    public interface IParentIdCache
    {
        Guid ParentId { get; set; }
    }
}