# Projeto Cooperativa Alfa

Projeto final de modernização de sistema legado para a **Cooperativa Financeira Alfa** como proposta de projeto final.

A aplicação integra uma tela moderna em .NET com um fluxo legado em COBOL, usando Helper C# e banco DB2.

## Objetivo

O objetivo do projeto é demonstrar uma modernização gradual, mantendo o COBOL participando do fluxo principal da aplicação.

O sistema permite:

- consultar cliente;
- cadastrar novo cliente;
- confirmar dados antes do cadastro;
- atualizar email e telefone;
- persistir os dados no DB2;

## Arquitetura

Fluxo principal da aplicação:

```text
Razor Pages -> API .NET -> COBOL -> Helper C# -> ODBC -> DB2
```

## Demonstração

### Tela Web

<img width="1200" height="675" alt="2026-07-08 05-21-52" src="https://github.com/user-attachments/assets/9792deae-8e31-4c20-a560-d6f8e24c687f" />

---

### Monitor COBOL

<img width="1200" height="675" alt="2026-07-08 05-32-29 (online-video-cutter com)" src="https://github.com/user-attachments/assets/a2aed9ef-d912-4562-bf05-803441d66c86" />

---

### Banco DB2

<img width="1582" height="450" alt="image" src="https://github.com/user-attachments/assets/16f0be1e-fed6-4d0e-8d88-10dbcb12be5c" />


![Banco DB2](docs/gifs/banco-db2.gif)

## Testando a API no navegador

Com a API rodando, é possível testar as rotas de consulta diretamente pelo navegador.

### Consulta de cliente existente

Acesse:

```text
http://localhost:5017/api/clientes/000001
```

Resultado esperado:

```text
Cliente encontrado.
Código de retorno: 0000
```

<img width="507" height="145" alt="image" src="https://github.com/user-attachments/assets/fb89096b-78dd-48c7-9948-77c374fb1700" />



### Consulta de cliente inexistente

Acesse:

```text
http://localhost:5017/api/clientes/999999
```

Resultado esperado:

```text
Cliente nao encontrado.
Código de retorno: 0404
```

<img width="662" height="132" alt="image" src="https://github.com/user-attachments/assets/a2960ec5-9357-4403-b51a-856c3c0d6847" />



## Tecnologias utilizadas

- .NET
- Razor Pages
- ASP.NET Core Web API
- C#
- COBOL
- GnuCOBOL
- DB2
- ODBC
- Docker
- PowerShell
- xUnit

## Estrutura do projeto

Estrutura principal do repositório:

```text
ProjetoCooperativaAlfa-Montreal/
├── db/
│   ├── consultas.sql
│   ├── dados-iniciais.sql
│   ├── limpar-base.sql
│   └── tabelas.sql
│
├── docs/
│   ├── Documento de Arquitetura .pdf
│   ├── Estrutura Compartilhada.pdf
│   ├── Plano de Testes.pdf
│   └── Relatorio IA.pdf
│
├── runtime/
│
├── scripts/
│   ├── gerarentradacbl.ps1
│   ├── rodarcobol.ps1
│   ├── testardb2.ps1
│   ├── testarfluxocompleto.ps1
│   └── testarhelper.ps1
│
├── shared/
│   └── cliente.json
│
├── src/
│   ├── Api/
│   │   ├── Controllers/
│   │   ├── Properties/
│   │   ├── Servicos/
│   │   ├── Cooperativa.Api.csproj
│   │   ├── Cooperativa.Api.http
│   │   ├── Program.cs
│   │   ├── appsettings.Development.json
│   │   └── appsettings.json
│   │
│   ├── Cobol/
│   │   ├── Copy/
│   │   └── PROJETOFINAL.cbl
│   │
│   ├── Contratos/
│   │   ├── CadastroCliente.cs
│   │   ├── Cliente.cs
│   │   ├── CodigoResposta.cs
│   │   ├── ContatoCliente.cs
│   │   ├── Cooperativa.Contratos.csproj
│   │   ├── OperacaoCliente.cs
│   │   └── RespostaCliente.cs
│   │
│   ├── Helper/
│   │   ├── ArquivoHelper.cs
│   │   ├── ClienteDb2Servico.cs
│   │   ├── ClienteMemoriaServico.cs
│   │   ├── ComandoHelper.cs
│   │   ├── Cooperativa.Helper.csproj
│   │   ├── IClienteServico.cs
│   │   ├── LeitorComando.cs
│   │   ├── Program.cs
│   │   └── ResultadoHelper.cs
│   │
│   └── Web/
│       ├── Pages/
│       ├── Properties/
│       ├── Servicos/
│       ├── wwwroot/
│       ├── Cooperativa.Web.csproj
│       ├── Program.cs
│       ├── appsettings.Development.json
│       └── appsettings.json
│
├── testes/
│   └── Xunit/
│       ├── ClientesApiServicoTests.cs
│       ├── ClientesControllerTests.cs
│       ├── ContratoClienteTests.cs
│       ├── Cooperativa.Testes.csproj
│       ├── FluxoCobolServicoTests.cs
│       ├── HelperFluxoTests.cs
│       ├── HelperTests.cs
│       └── ResultadoHelperTests.cs
│
├── .gitignore
├── Cooperativa.slnx
└── README.md
```

## Principais pastas

### `src/Api`

Contém a API .NET responsável por receber as requisições da tela, gerar a entrada para o COBOL, executar o programa COBOL e devolver o resultado para o frontend.

### `src/Web`

Contém a aplicação Razor Pages, que fornece a interface visual para consulta, cadastro e atualização de clientes.

### `src/Cobol`

Contém o programa COBOL principal e os copybooks usados na integração por arquivos.

### `src/Helper`

Contém o Helper C#, responsável por acessar o DB2 via ODBC e retornar os dados em formato fixo para o COBOL.

### `src/Contratos`

Contém as classes compartilhadas entre Web, API e Helper.

### `db`

Contém os scripts SQL para criação, limpeza, dados iniciais e consultas no DB2.

### `scripts`

Contém scripts PowerShell para gerar entrada, rodar COBOL, testar Helper, testar DB2 e validar o fluxo completo.

### `testes/Xunit`

Contém os testes automatizados do projeto.

### `docs`

Contém os documentos finais da entrega em PDF.


### Rotas principais da API

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/clientes/{codigo}` | Consulta cliente por código |
| POST | `/api/clientes` | Cadastra novo cliente |
| PUT | `/api/clientes/{codigo}/contato` | Atualiza email e telefone |


## Testes

Rodando os testes automatizados:

```powershell
dotnet test Cooperativa.slnx
```
Resultado:

<img width="659" height="184" alt="image" src="https://github.com/user-attachments/assets/52c8436a-755b-44ff-9395-3fde652c11e4" />


## Documentação

A documentação do projeto está na pasta `docs`:

```text
docs/
├── Documento de Arquitetura .pdf
├── Estrutura Compartilhada.pdf
├── Plano de Testes.pdf
└── Relatorio IA.pdf
```
