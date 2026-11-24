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
    public ActionResult<ProdutoResponse> Cadastrar(
        [FromBody] CreateProdutoRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var produto = _produtoService.Cadastrar(
            request.Nome,
            request.Categoria,
            request.CodigoBarras,
            request.Preco,
            request.NumeroSerie,
            request.Estoque);

        var response = ToResponse(produto);
        return CreatedAtAction(nameof(BuscarPorId), new { id = produto.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProdutoResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<ProdutoResponse>> Listar(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(_produtoService.ListarTodos().Select(ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProdutoResponse> BuscarPorId(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var produto = _produtoService.BuscarPorId(id);
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
