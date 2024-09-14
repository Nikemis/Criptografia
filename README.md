## Modos de Operação
Os modos de operação determinam como os blocos de dados são criptografados. O Electronic Codebook 
(ECB), embora simples e rápido, é menos seguro porque blocos idênticos de texto plano produzem 
blocos idênticos de texto cifrado. Outros modos, como Cipher Block Chaining (CBC), utilizam um vetor de 
inicialização (IV) e encadeiam blocos de texto cifrado com o próximo bloco de texto plano, melhorando a 
segurança.

## Implementação do Código
A implementação do código foi realizada na linguagem C# utilizando a biblioteca 
System.Security.Cryptography, que fornece suporte para operações criptográficas. O código exemplifica a 
criptografia e descriptografia usando DES, 3DES e AES, ilustrando a geração, armazenamento e 
recuperação de chaves e IVs.

## Estrutura do Código

1. Geração e Armazenamento de Chaves e IV:
• DES: Utiliza uma chave fixa de 8 bytes.
• 3DES: Gera uma chave de 24 bytes.
• AES: Gera uma chave de 32 bytes e um IV de 16 bytes.
• Chaves e o IV são salvos em arquivos para uso posterior.
2. Criptografia e Descriptografia:
• O texto é criptografado usando DES, seguido por 3DES e finalmente por AES.
• O processo inverso descriptografa os dados na ordem reversa, recuperando o texto original.

Os algoritmos são configurados com os seguintes parâmetros:
• DES e 3DES: Modo ECB e padding PKCS7.
• AES: Modo CBC e padding PKCS7.
