New-Item -ItemType Directory -Force -Path runtime | Out-Null

$operacao = "CADASTRAR".PadRight(10)
$codigo = "".PadRight(6)
$nome = "Maria Oliveira".PadRight(30)
$email = "maria.oliveira@cooperativa.com".PadRight(60)
$telefone = "".PadRight(11)

$linha = $operacao + $codigo + $nome + $email + $telefone

Set-Content -Path runtime\entrada.txt -Value $linha -Encoding ascii