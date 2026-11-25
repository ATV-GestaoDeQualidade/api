using MinhaApi.Dominio.DTOs.Vendas;
using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Services;
using Microsoft.AspNetCore.Mvc;

namespace MinhaApi.Api.Controllers;

[ApiController]
[Route("api/vendas")]
public sealed class VendaController : ControllerBase
{
    private readonly IVendaService _vendaService;
    private readonly IClienteService _clienteService;
    private readonly IProdutoService _produtoService;

    public VendaController(
        IVendaService vendaService,
        IClienteService clienteService,
        IProdutoService produtoService)
    {
        _vendaService = vendaService;
        _clienteService = clienteService;
        _produtoService = produtoService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VendaResponse>> Registrar(
        [FromBody] CreateVendaRequest request,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.BuscarPorIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente não encontrado.");

        if (request.Itens is null)
            throw new ArgumentException("A lista de itens da venda é obrigatória.", nameof(request.Itens));

        var itens = new List<(Produto produto, int quantidade)>();
        foreach (var item in request.Itens)
        {
            var produto = await _produtoService.BuscarPorIdAsync(item.ProdutoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Produto '{item.ProdutoId}' não encontrado.");

            itens.Add((produto, item.Quantidade));
        }

        var venda = await _vendaService.RegistrarVendaAsync(
            cliente,
            itens,
            request.FormaPagamento,
            request.Desconto,
            cancellationToken);

        return CreatedAtAction(nameof(BuscarPorId), new { id = venda.Id }, ToResponse(venda));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<VendaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<VendaResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var vendas = await _vendaService.ListarTodasAsync(cancellationToken);
        return Ok(vendas.Select(ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaResponse>> BuscarPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var venda = await _vendaService.BuscarPorIdAsync(id, cancellationToken);
        return venda is null ? NotFound() : Ok(ToResponse(venda));
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<VendaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<VendaResponse>>> ListarPorCliente(
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        if (await _clienteService.BuscarPorIdAsync(clienteId, cancellationToken) is null)
            return NotFound();

        var vendas = await _vendaService.ListarPorClienteAsync(clienteId, cancellationToken);
        return Ok(vendas.Select(ToResponse).ToArray());
    }

    private static VendaResponse ToResponse(MinhaApi.Dominio.Entities.Venda venda) =>
        new(
            venda.Id,
            venda.Cliente.Id,
            venda.Cliente.Nome,
            new DateTimeOffset(venda.DataVenda, TimeSpan.Zero),
            venda.FormaPagamento,
            venda.Desconto,
            venda.Itens.Select(item => new ItemVendaResponse(
                item.Produto.Id,
                item.Produto.Nome,
                item.Quantidade,
                item.PrecoUnitario,
                item.Subtotal)).ToArray(),
            venda.Subtotal,
            venda.ValorTotal);
}
