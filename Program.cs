using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CriptoProgram
{
    private const string CaminhoArquivoChaveAES = "aes_key.dat";
    private const string CaminhoArquivoIVAES = "aes_iv.dat";
    private const string CaminhoArquivoChave3DES = "3des_key.dat";
    private const string CaminhoArquivoChaveDES = "des_key.dat";

    public static void Main()
    {
        string textoPlano = "Oi, eu faço sistema de informação";

        // Definindo chaves e IV para AES, 3DES e DES
        byte[] chaveDES = ObterChaveDES();
        byte[] chave3DES = GerarChave3DES();
        byte[] chaveAES = GerarChaveAES();
        byte[] ivAES = GerarIVAES();

        // Salvar chaves e IV
        SalvarChave(CaminhoArquivoChaveDES, chaveDES);
        SalvarChave(CaminhoArquivoChave3DES, chave3DES);
        SalvarChave(CaminhoArquivoChaveAES, chaveAES);
        SalvarChave(CaminhoArquivoIVAES, ivAES);

        // Criptografar o texto com DES, depois com 3DES, e finalmente com AES
        byte[] desCriptografado = EncryptDES(textoPlano, chaveDES);
        byte[] tripleDesCriptografado = Encrypt3DES(desCriptografado, chave3DES);
        byte[] aesCriptografado = EncryptAES(tripleDesCriptografado, chaveAES, ivAES);

        // Exibir o texto criptografado com quebra de linha
        Console.WriteLine("Texto Criptografado:");
        Console.WriteLine(FormatHexadecimal(aesCriptografado));

        // Carregar chaves e IV
        byte[] chaveDESCarregada = CarregarChave(CaminhoArquivoChaveDES);
        byte[] chave3DESCarregada = CarregarChave(CaminhoArquivoChave3DES);
        byte[] chaveAESCarregada = CarregarChave(CaminhoArquivoChaveAES);
        byte[] ivAESCarregado = CarregarChave(CaminhoArquivoIVAES);

        // Descriptografar o texto com AES, depois com 3DES, e finalmente com DES
        byte[] aesDescriptografado = DecryptAES(aesCriptografado, chaveAESCarregada, ivAESCarregado);
        byte[] tripleDesDescriptografado = Decrypt3DES(aesDescriptografado, chave3DESCarregada);
        byte[] desDescriptografado = DecryptDES(tripleDesDescriptografado, chaveDESCarregada);

        string textoDescriptografado = Encoding.UTF8.GetString(desDescriptografado);
        Console.WriteLine("Texto Descriptografado: " + textoDescriptografado);
    }

    private static byte[] ObterChaveDES()
    {
        return Encoding.ASCII.GetBytes("D3SKEY!!"); // 8 bytes para DES
    }

    private static byte[] GerarChave3DES()
    {
        using (var tripleDes = TripleDES.Create())
        {
            byte[] chave = new byte[24]; // 24 bytes para TripleDES
            RandomNumberGenerator.Fill(chave);
            return chave;
        }
    }

    private static byte[] GerarChaveAES()
    {
        using (var aes = Aes.Create())
        {
            aes.Key = new byte[32]; // AES-256 tem uma chave de 32 bytes
            RandomNumberGenerator.Fill(aes.Key);
            return aes.Key;
        }
    }

    private static byte[] GerarIVAES()
    {
        using (var aes = Aes.Create())
        {
            aes.IV = new byte[16]; // IV para AES tem 16 bytes
            RandomNumberGenerator.Fill(aes.IV);
            return aes.IV;
        }
    }

    private static void SalvarChave(string caminhoArquivo, byte[] chave)
    {
        File.WriteAllBytes(caminhoArquivo, chave);
    }

    private static byte[] CarregarChave(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
        {
            throw new FileNotFoundException($"Arquivo de chave não encontrado: {caminhoArquivo}");
        }

        return File.ReadAllBytes(caminhoArquivo);
    }

    private static byte[] EncryptDES(string textoPlano, byte[] chave)
    {
        using (var des = DES.Create())
        {
            des.Key = chave;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            var encryptor = des.CreateEncryptor();
            var inputBuffer = Encoding.UTF8.GetBytes(textoPlano);

            return encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
        }
    }

    private static byte[] DecryptDES(byte[] ciphertext, byte[] chave)
    {
        using (var des = DES.Create())
        {
            des.Key = chave;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            var decryptor = des.CreateDecryptor();
            return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        }
    }

    private static byte[] Encrypt3DES(byte[] plaintext, byte[] chave)
    {
        using (var tripleDes = TripleDES.Create())
        {
            tripleDes.Key = chave;
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;

            var encryptor = tripleDes.CreateEncryptor();
            return encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
        }
    }

    private static byte[] Decrypt3DES(byte[] ciphertext, byte[] chave)
    {
        using (var tripleDes = TripleDES.Create())
        {
            tripleDes.Key = chave;
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;

            var decryptor = tripleDes.CreateDecryptor();
            return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        }
    }

    private static byte[] EncryptAES(byte[] plaintext, byte[] chave, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = chave;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
        }
    }

    private static byte[] DecryptAES(byte[] ciphertext, byte[] chave, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = chave;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        }
    }

    private static string FormatHexadecimal(byte[] bytes, int bytesPerLine = 16)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            if (i > 0 && i % bytesPerLine == 0)
            {
                sb.AppendLine(); // Adiciona uma quebra de linha após cada linha de bytesPerLine
            }
            sb.Append(bytes[i].ToString("X2")); // Converte cada byte em hexadecimal
            if (i < bytes.Length - 1)
            {
                sb.Append(" "); // Adiciona um espaço entre os bytes
            }
        }
        return sb.ToString();
    }
}
