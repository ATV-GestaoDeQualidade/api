using MinhaApi.Dominio.DTOs.Produtos;
using MinhaApi.Dominio.Services;
using Microsoft.AspNetCore.Mvc;

namespace MinhaApi.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public sealed class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProdutoResponse>> Cadastrar(
        [FromBody] CreateProdutoRequest request,
        CancellationToken cancellationToken)
    {
        var produto = await _produtoService.CadastrarAsync(
            request.Nome,
            request.Categoria,
            request.CodigoBarras,
            request.Preco,
            request.NumeroSerie,
            request.Estoque,
            cancellationToken);

        var response = ToResponse(produto);
        return CreatedAtAction(nameof(BuscarPorId), new { id = produto.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProdutoResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var produtos = await _produtoService.ListarTodosAsync(cancellationToken);
        return Ok(produtos.Select(ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> BuscarPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var produto = await _produtoService.BuscarPorIdAsync(id, cancellationToken);
        return produto is null ? NotFound() : Ok(ToResponse(produto));
    }

    private static ProdutoResponse ToResponse(MinhaApi.Dominio.Entities.Produto produto) =>
        new(
            produto.Id,
            produto.Nome,
            produto.Categoria,
            produto.CodigoBarras,
            produto.Preco,
            produto.NumeroSerie,
            produto.Estoque);
}
