# Livraria API

## Descrição

O nosso projeto se consiste em um sistema de verificação, cadastro e compra de uma pequena livraria, no qual o usuário pode interagir com a interface e utilizar das operações para simular de uma forma mais simplificada a busca e compra de livros por título, autor e gênero.

## Regra de negócio

Nenhum livro pode ser cadastrado ou listado com preço menor que R$ 10,00.

Integrantes:
- Caio Lamers
- Heverton Ricardo

URL base:
```text
http://localhost:5214
```

Senha de administrador:
```text
123456
```

## Funcionalidades

- Cadastrar livros
- Listar livros
- Buscar livro por ID
- Atualizar livro
- Remover livro
- Criar carrinho
- Listar carrinhos
- Buscar livros de um carrinho
- Atualizar carrinho
- Remover carrinho
- Adicionar livro no carrinho
- Remover livro do carrinho
- Realizar compra

## URLs

```text
GET    /livros
GET    /livros?genero=Romance
GET    /livros/{id}
POST   /livros/{senha}
PUT    /livros/{id}/{senha}
DELETE /livros/{id}/{senha}
```

```text
GET    /carrinho
GET    /carrinho/{id}
POST   /carrinho
PUT    /carrinho/{id}
DELETE /carrinho/{id}
POST   /carrinho/{idCarrinho}/livros/{idLivro}
DELETE /carrinho/{idCarrinho}/livros/{idLivro}
POST   /carrinho/{id}/compra/{confirmar}
```

Exemplo da compra:
```text
POST /carrinho/1/compra/sim
POST /carrinho/1/compra/nao
```

## JSONs prontos

Cadastrar livro:
```json
{
  "titulo": "Dom Casmurro",
  "autor": "Machado de Assis",
  "genero": "Romance",
  "preco": 35.90
}
```

Atualizar livro:
```json
{
  "titulo": "Dom Casmurro",
  "autor": "Machado de Assis",
  "genero": "Romance",
  "preco": 39.90
}
```

Criar carrinho:
```json
{
  "id": 1,
  "cliente": "Caio"
}
```

Atualizar carrinho:
```json
{
  "id": 1,
  "cliente": "Heverton"
}
```

## Exemplos com URL completa

Cadastrar livro:
```text
POST http://localhost:5214/livros/123456
```

Criar carrinho:
```text
POST http://localhost:5214/carrinho
```

Adicionar livro 1 no carrinho 1:
```text
POST http://localhost:5214/carrinho/1/livros/1
```

Realizar compra:
```text
POST http://localhost:5214/carrinho/1/compra/sim
```
