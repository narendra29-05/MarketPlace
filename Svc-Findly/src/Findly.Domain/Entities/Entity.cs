
namespace Findly.Domain.Entities;

public abstract class Entity
{
    public int      Id        { get; protected set; }
    public string?  CreatedBy { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public string?  UpdatedBy { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
}
