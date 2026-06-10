using LivrariaAPI.Data;
using LivrariaAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<LivrariaContext>(options =>
    options.UseSqlite("Data Source=livraria.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.MapOpenApi();
}
app.UseHttpsRedirection();
/*=====================================*/
//LISTAR LIVROS(SEM E COM FILTRO DE GÊNERO)
app.MapGet("/livros", async (string? genero, LivrariaContext banco) =>
{
    if (genero == null)
    {
        return Results.Ok(await banco.Livros.ToListAsync());
    }

    var livrosFiltrados = await banco.Livros
        .Where(livro => livro.Genero.ToLower() == genero.ToLower())
        .ToListAsync();

    return Results.Ok(livrosFiltrados);
});
//BUSCAR LIVRO POR ID
app.MapGet("/livros/{id}", async (int id, LivrariaContext banco) =>
{
    var livro = await banco.Livros.FindAsync(id);

    if (livro == null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    return Results.Ok(livro);
});
/*=====================================*/
//CADASTRAR LIVRO(SENHA ADMIN)
app.MapPost("/livros/{senha}", async (string senha, Livro livro, LivrariaContext banco) =>{

    if(senha != "123456")
    {
       return Results.BadRequest("Senha inválida.");
    }

    if (livro.Preco < 10)
    {
        return Results.BadRequest("Preço mínimo R$ 10,00");
    }

    banco.Livros.Add(livro);
    await banco.SaveChangesAsync();

    return Results.Ok("Livro cadastrado com sucesso!");
});
/*=====================================*/
//DELETAR LIVRO POR ID(SENHA ADMIN)
app.MapDelete("/livros/{id}/{senha}", async (int id, string senha, LivrariaContext banco) =>{

    if(senha != "123456")
    {
       return Results.BadRequest("Senha inválida.");
    }    
    
    var livro = await banco.Livros.FindAsync(id);

    if (livro == null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    banco.Livros.Remove(livro);
    await banco.SaveChangesAsync();
    
    return Results.Ok("Livro removido com sucesso.");
});
/*=====================================*/
//ATUALIZAR LIVRO POR ID(SENHA ADMIN)
app.MapPut("/livros/{id}/{senha}", async (int id, string senha, Livro Atualizar, LivrariaContext banco) =>
{
    if (senha != "123456")
    {
        return Results.BadRequest("Senha inválida.");
    }

    var livro = await banco.Livros.FindAsync(id);

    if (livro == null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    if (Atualizar.Preco < 10)
    {
        return Results.BadRequest("Preço mínimo R$ 10,00");        
    }

    livro.Titulo = Atualizar.Titulo;
    livro.Autor = Atualizar.Autor;
    livro.Genero = Atualizar.Genero;
    livro.Preco = Atualizar.Preco;

    await banco.SaveChangesAsync();

    return Results.Ok("Livro atualizado com sucesso.");
});
/*=====================================*/
/*=====================================*/
//LISTAR TODOS OS CARRINHOS
app.MapGet("/carrinho", async (LivrariaContext banco) =>
{
    var carrinhos = await banco.Carrinhos
        .Include(carrinho => carrinho.Itens)
        .ToListAsync();

    return Results.Ok(carrinhos);
});
/*=====================================*/
//CRIAR CARRINHO DE CLIENTE
app.MapPost("/carrinho", async (Carrinho carrinho, LivrariaContext banco) =>
{
    carrinho.ValorTotal = 0;
    carrinho.Itens = new List<Livro>();

    banco.Carrinhos.Add(carrinho);
    await banco.SaveChangesAsync();

    return Results.Ok("Carrinho criado com sucesso!");
});
/*=====================================*/
//ATUALIZAR CARRINHO
app.MapPut("/carrinho/{id}", async (int id, Carrinho Atualizar, LivrariaContext banco) =>
{
    var carrinho = await banco.Carrinhos.FindAsync(id);

    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }

    carrinho.Cliente = Atualizar.Cliente;
    await banco.SaveChangesAsync();

    return Results.Ok("Carrinho atualizado com sucesso!");
});
/*=====================================*/
//ENVIAR LIVRO PARA O CARRINHO POR ID
app.MapPost("/carrinho/{idCarrinho}/livros/{idLivro}", async (int idCarrinho, int idLivro, LivrariaContext banco) =>{
    var carrinho = await banco.Carrinhos
        .Include(carrinho => carrinho.Itens)
        .FirstOrDefaultAsync(carrinho => carrinho.Id == idCarrinho);

    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }

    var livro = await banco.Livros.FindAsync(idLivro);

    if(livro == null)
    {
        return Results.BadRequest("Livro não encontrado");
    }
    carrinho.Itens.Add(livro);
    carrinho.ValorTotal += livro.Preco;

    await banco.SaveChangesAsync();

    return Results.Ok("Livro enviado para o carrinho com sucesso!");
});
/*=====================================*/
//LISTAR TODOS OS LIVROS DO CARRINHO
app.MapGet("/carrinho/{id}", async (int id, LivrariaContext banco)=>{
    var carrinho = await banco.Carrinhos
        .Include(carrinho => carrinho.Itens)
        .FirstOrDefaultAsync(carrinho => carrinho.Id == id);

    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }
    return Results.Ok(carrinho.Itens);    
});
/*=====================================*/
//DELETAR LIVRO DA LISTA DO CARRINHO
app.MapDelete("/carrinho/{idCarrinho}/livros/{idLivro}", async (int idCarrinho, int idLivro, LivrariaContext banco) =>{
    var carrinho = await banco.Carrinhos
        .Include(carrinho => carrinho.Itens)
        .FirstOrDefaultAsync(carrinho => carrinho.Id == idCarrinho);

    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }
    var livro = carrinho.Itens.FirstOrDefault(livro => livro.Id == idLivro);
    if(livro == null)
    {
        return Results.BadRequest("Livro não encontrado");
    }
    carrinho.Itens.Remove(livro);
    carrinho.ValorTotal -= livro.Preco;

    await banco.SaveChangesAsync();

    return Results.Ok("Livro excluído com sucesso!");    
});
/*=====================================*/
//DELETAR CARRINHO
app.MapDelete("/carrinho/{id}", async (int id, LivrariaContext banco) =>{
    var carrinho = await banco.Carrinhos.FindAsync(id);

    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }

    banco.Carrinhos.Remove(carrinho);
    await banco.SaveChangesAsync();

    return Results.Ok("Carrinho removido com sucesso!");
});
/*=====================================*/
//REALIZAR COMPRA(SOMA DE PREÇOS DA LISTA)
app.MapPost("/carrinho/{id}/compra/{confirmar}", async (int id, string confirmar, LivrariaContext banco) =>{
    var carrinho = await banco.Carrinhos
        .Include(carrinho => carrinho.Itens)
        .FirstOrDefaultAsync(carrinho => carrinho.Id == id);

    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }

    if(carrinho.Itens.Count == 0)
    {
        return Results.BadRequest("Seu carrinho esta vazio!");
    }

    if(confirmar != "sim" && confirmar != "nao")
    {
    return Results.BadRequest("Informe 'sim' ou 'nao'.");
    }

    string resultado = "Voltamos às compras!";

    if(confirmar == "sim")
    {
        resultado = "Carrinho esvaziado, volte sempre!";
    }

    var RegistroCompra = new{
        Comprador = carrinho.Cliente,
        QntLivros = carrinho.Itens.Count,
        Total = carrinho.ValorTotal,
        Resultado  = resultado
    };

    if(confirmar == "sim")
    {
    carrinho.Itens.Clear();
    carrinho.ValorTotal = 0; 
    await banco.SaveChangesAsync();
    }
    return Results.Ok(RegistroCompra);
});
app.Run();
