using Microsoft.EntityFrameworkCore;
using Proje;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .SetIsOriginAllowed(_ => true); // Allow all origins
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Items.Any())
    {
        dbContext.Items.Add(new Item { Name = "Sample Item" });
        dbContext.SaveChanges();
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();

app.MapHub<ChatHub>("/chat");

app.Run();

namespace Proje
{
    public class AppDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;
        public int ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;
    }
}
