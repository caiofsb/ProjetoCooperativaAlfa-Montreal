using System.Data.Odbc;
using Cooperativa.Contratos;

namespace Cooperativa.Helper;

public sealed class ClienteDb2Servico : IClienteServico
{
    private readonly string connectionString;

    public ClienteDb2Servico(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public ResultadoHelper Consultar(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return ResultadoHelper.DadosInvalidos("Codigo obrigatorio.");
        }

        try
        {
            using var conexao = new OdbcConnection(connectionString);
            conexao.Open();

            using var comando = conexao.CreateCommand();
            comando.CommandText = """
                SELECT CLI_CODIGO,
                       CLI_NOME,
                       CLI_EMAIL,
                       CLI_TELEFONE
                  FROM CLIENTES
                 WHERE CLI_CODIGO = ?
                """;

            comando.Parameters.AddWithValue("codigo", codigo);

            using var leitor = comando.ExecuteReader();

            if (!leitor.Read())
            {
                return ResultadoHelper.NaoEncontrado();
            }

            var cliente = LerCliente(leitor);

            return ResultadoHelper.Ok(cliente, "Cliente encontrado.");
        }
        catch
        {
            return ResultadoHelper.ErroSistema("Erro ao consultar DB2.");
        }
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

        try
        {
            if (EmailJaExiste(email, null))
            {
                return ResultadoHelper.EmailDuplicado();
            }

            var codigo = GerarProximoCodigo();

            using var conexao = new OdbcConnection(connectionString);
            conexao.Open();

            using var comando = conexao.CreateCommand();
            comando.CommandText = """
                INSERT INTO CLIENTES
                    (CLI_CODIGO, CLI_NOME, CLI_EMAIL, CLI_TELEFONE)
                VALUES
                    (?, ?, ?, ?)
                """;

            comando.Parameters.AddWithValue("codigo", codigo);
            comando.Parameters.AddWithValue("nome", nome);
            comando.Parameters.AddWithValue("email", email);
            comando.Parameters.AddWithValue(
                "telefone",
                string.IsNullOrWhiteSpace(telefone) ? DBNull.Value : telefone
            );

            comando.ExecuteNonQuery();

            var cliente = new Cliente
            {
                Codigo = codigo,
                Nome = nome,
                Email = email,
                Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone
            };

            return ResultadoHelper.Ok(cliente, "Cliente cadastrado.");
        }
        catch (OdbcException erro) when (EhEmailDuplicado(erro))
        {
            return ResultadoHelper.EmailDuplicado();
        }
        catch
        {
            return ResultadoHelper.ErroSistema("Erro ao cadastrar DB2.");
        }
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

        try
        {
            var clienteAtual = Consultar(codigo);

            if (!clienteAtual.Sucesso)
            {
                return clienteAtual;
            }

            if (EmailJaExiste(email, codigo))
            {
                return ResultadoHelper.EmailDuplicado();
            }

            using var conexao = new OdbcConnection(connectionString);
            conexao.Open();

            using var comando = conexao.CreateCommand();
            comando.CommandText = """
                UPDATE CLIENTES
                   SET CLI_EMAIL = ?,
                       CLI_TELEFONE = ?,
                       DT_ATUALIZACAO = CURRENT TIMESTAMP
                 WHERE CLI_CODIGO = ?
                """;

            comando.Parameters.AddWithValue("email", email);
            comando.Parameters.AddWithValue(
                "telefone",
                string.IsNullOrWhiteSpace(telefone) ? DBNull.Value : telefone
            );
            comando.Parameters.AddWithValue("codigo", codigo);

            comando.ExecuteNonQuery();

            var cliente = new Cliente
            {
                Codigo = codigo,
                Nome = clienteAtual.Cliente?.Nome ?? string.Empty,
                Email = email,
                Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone
            };

            return ResultadoHelper.Ok(cliente, "Cliente atualizado.");
        }
        catch (OdbcException erro) when (EhEmailDuplicado(erro))
        {
            return ResultadoHelper.EmailDuplicado();
        }
        catch
        {
            return ResultadoHelper.ErroSistema("Erro ao atualizar DB2.");
        }
    }

    private bool EmailJaExiste(string email, string? codigoIgnorado)
    {
        using var conexao = new OdbcConnection(connectionString);
        conexao.Open();

        using var comando = conexao.CreateCommand();

        if (string.IsNullOrWhiteSpace(codigoIgnorado))
        {
            comando.CommandText = """
                SELECT COUNT(*)
                  FROM CLIENTES
                 WHERE UPPER(CLI_EMAIL) = UPPER(?)
                """;

            comando.Parameters.AddWithValue("email", email);
        }
        else
        {
            comando.CommandText = """
                SELECT COUNT(*)
                  FROM CLIENTES
                 WHERE UPPER(CLI_EMAIL) = UPPER(?)
                   AND CLI_CODIGO <> ?
                """;

            comando.Parameters.AddWithValue("email", email);
            comando.Parameters.AddWithValue("codigo", codigoIgnorado);
        }

        var total = Convert.ToInt32(comando.ExecuteScalar());

        return total > 0;
    }

    private string GerarProximoCodigo()
    {
        using var conexao = new OdbcConnection(connectionString);
        conexao.Open();

        using var comando = conexao.CreateCommand();
        comando.CommandText = """
            SELECT COALESCE(MAX(INTEGER(CLI_CODIGO)), 0) + 1
              FROM CLIENTES
            """;

        var proximo = Convert.ToInt32(comando.ExecuteScalar());

        return proximo.ToString("000000");
    }

    private static Cliente LerCliente(OdbcDataReader leitor)
    {
        return new Cliente
        {
            Codigo = leitor.GetString(0).Trim(),
            Nome = leitor.GetString(1).Trim(),
            Email = leitor.GetString(2).Trim(),
            Telefone = leitor.IsDBNull(3) ? null : leitor.GetString(3).Trim()
        };
    }

    private static bool EhEmailDuplicado(OdbcException erro)
    {
        foreach (OdbcError item in erro.Errors)
        {
            if (item.SQLState == "23505")
            {
                return true;
            }
        }

        return false;
    }
}