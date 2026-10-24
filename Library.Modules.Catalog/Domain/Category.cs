namespace Library.Modules.Catalog.Domain;

public class Category
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } = null!;
  public string? Description { get; private set; }
  public bool IsActive { get; private set; } = true;

  private Category() {}
  
  public static Category Create(string name, string? description, bool isActive)
  {
    if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");

    return new Category { Id = Guid.NewGuid(), Name = name, Description = description, IsActive = isActive };
  }
  
  public void Update(string name, string? description, bool isActive)
  {
    if (string.IsNullOrWhiteSpace(name)) 
      throw new ArgumentException("Name is required");
    
    Name = name;
    Description = description;
    IsActive = isActive;
  }
}