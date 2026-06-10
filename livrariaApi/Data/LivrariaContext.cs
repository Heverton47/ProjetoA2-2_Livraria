using LivrariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LivrariaAPI.Data;

public class LivrariaContext : DbContext
{
    public LivrariaContext(DbContextOptions<LivrariaContext> options) : base(options)
    {
    }

    public DbSet<Livro> Livros { get; set; }
    public DbSet<Carrinho> Carrinhos { get; set; }
}
