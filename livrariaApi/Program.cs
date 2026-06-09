using LivrariaAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

List<Livro> livros = new List<Livro>();
List<Carrinho> carrinhos = new List<Carrinho>();

if (app.Environment.IsDevelopment()){
    app.MapOpenApi();
}
app.UseHttpsRedirection();
/*=====================================*/
//LISTAR LIVROS(SEM E COM FILTRO DE GÊNERO)
app.MapGet("/livros", (string? genero) =>
{
    if (genero == null)
    {
        return Results.Ok(livros);
    }

    return Results.Ok(
        livros.Where(livro => livro.Genero.Equals(genero, StringComparison.OrdinalIgnoreCase))
    );
});
//BUSCAR LIVRO POR ID
app.MapGet("/livros/{id}", (int id) =>
{
    var livro = livros.FirstOrDefault(livro => livro.Id == id);

    if (livro == null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    return Results.Ok(livro);
});
/*=====================================*/
//CADASTRAR LIVRO(SENHA ADMIN)
app.MapPost("/livros/{senha}", (string senha, Livro livro) =>{

    if(senha != "123456")
    {
       return Results.BadRequest("Senha inválida.");
    }

    if (livro.Preco < 10)
    {
        return Results.BadRequest("Preço mínimo R$ 10,00");
    }

    livros.Add(livro);

    return Results.Ok("Livro cadastrado com sucesso!");
});
/*=====================================*/
//DELETAR LIVRO POR ID(SENHA ADMIN)
app.MapDelete("/livros/{id}/{senha}", (int id, string senha) =>{

    if(senha != "123456")
    {
       return Results.BadRequest("Senha inválida.");
    }    
    
    var livro = livros.FirstOrDefault(livro => livro.Id == id);

    if (livro == null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    livros.Remove(livro);
    
    return Results.Ok("Livro removido com sucesso.");
});
/*=====================================*/
//ATUALIZAR LIVRO POR ID(SENHA ADMIN)
app.MapPut("/livros/{id}/{senha}", (int id, string senha, Livro Atualizar) =>
{
    if (senha != "123456")
    {
        return Results.BadRequest("Senha inválida.");
    }

    var livro = livros.FirstOrDefault(livro => livro.Id == id);

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

    return Results.Ok("Livro atualizado com sucesso.");
});
/*=====================================*/
/*=====================================*/
//CRIAR CARRINHO DE CLIENTE
app.MapPost("/carrinho", (Carrinho carrinho) =>
{
    carrinhos.Add(carrinho);

    return Results.Ok("Carrinho criado com sucesso!");
});
/*=====================================*/
//ENVIAR LIVRO PARA O CARRINHO POR ID
app.MapPost("/carrinho/{idCarrinho}/livros/{idLivro}", (int idCarrinho, int idLivro) =>{
    var carrinho = carrinhos.FirstOrDefault(carrinho => carrinho.Id == idCarrinho);
    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }
    var livro = livros.FirstOrDefault(livro => livro.Id == idLivro);
    if(livro == null)
    {
        return Results.BadRequest("Livro não encontrado");
    }
    carrinho.Itens.Add(livro);
    carrinho.ValorTotal += livro.Preco;

    return Results.Ok("Livro enviado para o carrinho com sucesso!");
});
/*=====================================*/
//LISTAR TODOS OS LIVROS DO CARRINHO
app.MapGet("/carrinho/{id}", (int id)=>{
    var carrinho = carrinhos.FirstOrDefault(carrinho => carrinho.Id == id);
    if(carrinho == null)
    {
        return Results.BadRequest("Carrinho não encontrado");
    }
    return Results.Ok(carrinho.Itens);    
});
/*=====================================*/
//DELETAR LIVRO DA LISTA DO CARRINHO
app.MapDelete("/carrinho/{idCarrinho}/livros/{idLivro}",(int idCarrinho, int idLivro) =>{
    var carrinho = carrinhos.FirstOrDefault(carrinho => carrinho.Id == idCarrinho);
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

    return Results.Ok("Livro excluído com sucesso!");    
});
/*=====================================*/
//REALIZAR COMPRA(SOMA DE PREÇOS DA LISTA)
app.MapPost("/carrinho/{id}/compra/{confirmar}", (int id, string confirmar) =>{
    var carrinho = carrinhos.FirstOrDefault(carrinho => carrinho.Id == id);
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
    }
    return Results.Ok(RegistroCompra);
});
app.Run();