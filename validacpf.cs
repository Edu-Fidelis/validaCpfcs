using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

public static class ValidaCpfFunction
{
    // Função de validação de CPF
    [FunctionName("ValidaCpf")]
    public static string Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] string cpf,
        ILogger log)
    {
        log.LogInformation($"Validação do CPF iniciada: {cpf}");

        // Chama a função de validação de CPF
        bool isValid = ValidarCpf(cpf);
        return isValid ? "CPF válido" : "CPF inválido";
    }

    // Função que valida o CPF
    private static bool ValidarCpf(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "");

        // Verifica se o CPF tem o tamanho correto e se todos os caracteres são números
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;

        // Multiplicadores para o cálculo dos dígitos verificadores
        int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        // Calcula o primeiro dígito verificador
        var primeiroDigito = CalcularDigito(cpf.Substring(0, 9), multiplicadores1);
        // Calcula o segundo dígito verificador
        var segundoDigito = CalcularDigito(cpf.Substring(0, 9) + primeiroDigito, multiplicadores2);

        // Verifica se o CPF é válido
        return cpf.EndsWith(primeiroDigito.ToString() + segundoDigito.ToString());
    }

    // Função para calcular o dígito verificador
    private static int CalcularDigito(string numero, int[] multiplicadores)
    {
        int soma = 0;
        for (int i = 0; i < numero.Length; i++)
        {
            soma += int.Parse(numero[i].ToString()) * multiplicadores[i];
        }
        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
