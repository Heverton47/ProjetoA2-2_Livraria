namespace LivrariaAPI.Models;

public class Carrinho
{
    public int Id { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public List<Livro> Itens { get; set; } = new();
}