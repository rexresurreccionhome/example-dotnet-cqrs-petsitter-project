using System.Reflection;
using Cqrs.Event;

namespace Cqrs.Domain;

public abstract class AggregateRoot
{
    private readonly List<BaseEvent> _uncommittedChanges = [];
    public Guid Id { get; set; }
    public int Version { get; set; } = 0;

    public IEnumerable<BaseEvent> GetUncommittedChanges()
    {
        return _uncommittedChanges;
    }

    public void MarkAsCommitted()
    {
        _uncommittedChanges.Clear();
    }

    private void ApplyChange(BaseEvent @event, bool uncommitted)
    {
        // https://medium.com/@trapdoorlabs/dynamically-invoking-c-methods-a4cd1e846676
        string applyMethodName = "Apply";
        MethodInfo? method = this.GetType().GetMethod(applyMethodName, new Type[] { @event.GetType() });
        if (method is null)
        {
            throw new ArgumentNullException(applyMethodName, $"Apply method not found in the aggregator for {@event.GetType().Name}");
        }
        
        method.Invoke(this, new object[] { @event });
        if (uncommitted)
        {
            _uncommittedChanges.Add(@event);
        }
    }

    protected void NewUncomittedEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach(BaseEvent @event in events) {
            ApplyChange(@event, false);
        }
    }
}
