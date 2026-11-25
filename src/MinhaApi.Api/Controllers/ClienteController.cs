using MinhaApi.Dominio.DTOs.Clientes;
using MinhaApi.Dominio.Services;
using Microsoft.AspNetCore.Mvc;

namespace MinhaApi.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Cadastrar(
        [FromBody] CreateClienteRequest request,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.CadastrarAsync(
            request.Nome,
            request.CpfCnpj,
            request.Telefone,
            request.Email,
            request.Endereco,
            cancellationToken);

        return CreatedAtAction(nameof(BuscarPorId), new { id = cliente.Id }, ToResponse(cliente));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClienteResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ListarTodosAsync(cancellationToken);
        return Ok(clientes.Select(ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> BuscarPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.BuscarPorIdAsync(id, cancellationToken);
        return cliente is null ? NotFound() : Ok(ToResponse(cliente));
    }

    [HttpGet("buscar")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> BuscarPorDocumento(
        [FromQuery] string documento,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.BuscarPorDocumentoAsync(documento, cancellationToken);
        return cliente is null ? NotFound() : Ok(ToResponse(cliente));
    }

    private static ClienteResponse ToResponse(MinhaApi.Dominio.Entities.Cliente cliente) =>
        new(
            cliente.Id,
            cliente.Nome,
            cliente.CpfCnpj,
            cliente.Telefone,
            cliente.Email,
            cliente.Endereco);
}
