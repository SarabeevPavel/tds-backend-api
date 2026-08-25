using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<FolderObject> Folders => Set<FolderObject>();
    public DbSet<FileObject> Files => Set<FileObject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>().OwnsOne(t => t.CreatedBy);
        modelBuilder.Entity<FileObject>().ToTable("Files").OwnsOne(f => f.CreatedBy);
        modelBuilder.Entity<FolderObject>().ToTable("Folders").OwnsOne(f => f.CreatedBy);
    }
}
