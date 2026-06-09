namespace LivrariaAPI.Models;

public class SenhaAdmin{
    public string Senha { get; set; } = string.Empty;

    public Livro Livro { get; set; } = new();
}