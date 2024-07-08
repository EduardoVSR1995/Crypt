# Aplicação de Compactação com Criptografia e Descriptografia de Dados em Linha de Comando

Criada para compactar e criptografar dados de forma massiva e recursiva em um único arquivo `.txt`, facilitando a transmissão de grande volume de arquivos com segurança e de forma menos onerosa. Esta é uma aplicação de linha de comando.

## Utilização

1. Baixe o `Crypt.exe`.
2. No local onde foi feito o download, execute o seguinte comando:

```sh
Crypt.exe [dec ou enc] [caminho para o que será criptografado ou onde será a saída dos dados] [arquivo .txt de saída ou de dados criptografados] [sua senha para criptografia ou descriptografia]
```

### Exemplo criptografia
------
```sh
Crypt.exe enc C:\caminho\do\diretorio\entrada C:\caminho\para\saida\arquivo.txt suaSenhaAqui
```

### Exemplo criptografia
------
```sh
Crypt.exe dec C:\caminho\do\diretorio\saida C:\caminho\para\entrada\arquivo.txt suaSenhaAqui
```


