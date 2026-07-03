using Cooperativa.Contratos;

namespace Cooperativa.Helper;

public sealed class ClienteMemoriaServico : IClienteServico
{
    private readonly List<Cliente> clientes =
    [
        new Cliente
        {
            Codigo = "000001",
            Nome = "Ana Silva",
            Email = "ana.silva@cooperativa.com",
            Telefone = "11988887777"
        },
        new Cliente
        {
            Codigo = "000002",
            Nome = "Bruno Costa",
            Email = "bruno.costa@cooperativa.com",
            Telefone = "21977776666"
        },
        new Cliente
        {
            Codigo = "000003",
            Nome = "Carla Souza",
            Email = "carla.souza@cooperativa.com",
            Telefone = null
        }
    ];

    public ResultadoHelper Consultar(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return ResultadoHelper.DadosInvalidos("Codigo obrigatorio.");
        }

        var cliente = clientes.FirstOrDefault(c => c.Codigo == codigo);

        if (cliente is null)
        {
            return ResultadoHelper.NaoEncontrado();
        }

        return ResultadoHelper.Ok(cliente, "Cliente encontrado.");
    }

    public ResultadoHelper Cadastrar(string nome, string email, string? telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return ResultadoHelper.DadosInvalidos("Nome obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return ResultadoHelper.DadosInvalidos("Email obrigatorio.");
        }

        if (EmailJaExiste(email))
        {
            return ResultadoHelper.EmailDuplicado();
        }

        var proximoCodigo = (clientes.Count + 1).ToString("000000");

        var cliente = new Cliente
        {
            Codigo = proximoCodigo,
            Nome = nome,
            Email = email,
            Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone
        };

        clientes.Add(cliente);

        return ResultadoHelper.Ok(cliente, "Cliente cadastrado.");
    }

    public ResultadoHelper Atualizar(string codigo, string email, string? telefone)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return ResultadoHelper.DadosInvalidos("Codigo obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return ResultadoHelper.DadosInvalidos("Email obrigatorio.");
        }

        var cliente = clientes.FirstOrDefault(c => c.Codigo == codigo);

        if (cliente is null)
        {
            return ResultadoHelper.NaoEncontrado();
        }

        var emailUsadoPorOutroCliente = clientes.Any(c =>
            c.Codigo != codigo &&
            string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase)
        );

        if (emailUsadoPorOutroCliente)
        {
            return ResultadoHelper.EmailDuplicado();
        }

        cliente.Email = email;
        cliente.Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone;

        return ResultadoHelper.Ok(cliente, "Cliente atualizado.");
    }

    private bool EmailJaExiste(string email)
    {
        return clientes.Any(c =>
            string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase)
        );
    }
}