namespace HotelManagementSystem.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; private set; }

    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

    protected Entity()
    { }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    override public bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return GetType() == other.GetType()
            && Id == other.Id;

    }

    override public int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}