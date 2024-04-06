namespace Lilith.Server.Entities;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool Disabled { get; set; } = false;
}
