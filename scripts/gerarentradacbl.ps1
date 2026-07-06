New-Item -ItemType Directory -Force -Path runtime | Out-Null

$operacao = "CONSULTAR".PadRight(10)
$codigo = "000001".PadRight(6)
$nome = "".PadRight(30)
$email = "".PadRight(60)
$telefone = "".PadRight(11)

$linha = $operacao + $codigo + $nome + $email + $telefone

Set-Content -Path runtime\entrada.txt -Value $linha -Encoding ascii