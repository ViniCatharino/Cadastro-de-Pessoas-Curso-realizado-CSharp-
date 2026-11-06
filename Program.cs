using System.Security;
using CadastroPessoas.Models;
using CadastroPessoas.Repositories;
using CadastroPessoas.Services;
using Microsoft.Data.SqlClient;

namespace CadastroPessoas
{
    internal class Program
    {
        static PessoaService _service = null!;
        const string formato = "{0,-6} {1,-25} {2,-30} {3,-20} {4,-20}";

        static void Main(string[] args)
        {
            string connectionString =
                @"Server=VINI-PC\SQLEXPRESS;Database=CadastroPessoas;Trusted_Connection=True;TrustServerCertificate=True;";

            var factory = new SqlConnectionFactory(connectionString);
            IPessoaRepository repository = new PessoaRepository(factory);
            _service = new PessoaService(repository);

            bool continuar = true;
            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("Bem-Vindo ao Cadastro de pessoas\n");
                Console.WriteLine("1. Adicionar");
                Console.WriteLine("2. Consultar");
                Console.WriteLine("3. Alterar");
                Console.WriteLine("4. Excluir");
                Console.WriteLine("0. Sair\n");
                Console.Write("Escolha uma opção: ");

                switch (Console.ReadLine())
                {
                    case "1": 
                      TelaAdicionar(); 
                      break;
                    case "2":
                      TelaConsultar();
                      break;
                    case "3":
                      TelaAlterar();
                      break;
                    case "4":
                      TelaExcluir();
                      break;
                    case "0": 
                      continuar = false; 
                      break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void TelaAdicionar()
        {
            Console.Clear();
            Console.WriteLine("=== Adicionar Pessoa ===");

            Console.Write("Nome: ");
            string nome = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";

            try
            {
                _service.Adicionar(nome, email);
                Console.WriteLine("\nPessoa adicionada com sucesso!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"\nErro no banco: {ex.Message}");
            }

            Console.ReadKey();
        }

        static void TelaConsultar()

        {
            string nome = "";
            while(nome != "0")
            {
                Console.Clear();
                Console.WriteLine("=== Consultar Pessoa ===");

                Console.WriteLine("Digite o nome da pessoa ou deixe vazio para trazer todos os clientes");
                Console.Write("ou Digite '0' para voltar:");
                nome = Console.ReadLine() ?? "";

                if (nome == "0") continue;
                
                try
                {
                    var pessoas = _service.Consultar(nome);
                    Console.WriteLine();

                    if (pessoas.Count == 0)
                    {
                        Console.WriteLine("Nenhuma pessoa encontrada.");
                        Console.ReadKey();
                        continue;       
                    }

                        Console.WriteLine();
                        Console.WriteLine(formato, "Código", "Nome", "Email", "Cadastrado em", "Última alteração");
                        Console.WriteLine(new string('-', 6 + 1 + 25 + 1 + 30 + 1 + 20 + 1 + 20));

                        foreach (var p in pessoas)
                        {
                            Console.WriteLine(formato,
                                p.Id,
                                p.Nome,
                                p.Email                                    ?? "-",
                                p.CriadoEm?.ToString("dd/MM/yyyy HH:mm")   ?? "-",
                                p.AlteradoEm?.ToString("dd/MM/yyyy HH:mm") ?? "-");
                        }

                        Console.WriteLine($"\nTotal: {pessoas.Count}");
                        Console.WriteLine("\nPressione qualquer tecla para voltar...");
                        Console.ReadKey();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\n{ex.Message}");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"\nErro no banco: {ex.Message}");
                }

                Console.ReadKey();    
            }

        }
    
        static void TelaAlterar()
        {
            Console.Clear();
            Console.WriteLine("=== Alterar Pessoa ===");
            Console.WriteLine();
            Console.Write("Digite o Código da pessoal que deseja Alterar:");
            string entrada = Console.ReadLine() ?? "";

            if (!int.TryParse(entrada,out int idPessoa) || idPessoa <= 0)
            {
                Console.WriteLine("Digite um código numérico válido.");
                Console.ReadKey();
                return;
            }

            Pessoa? pessoa = null;

            try
            {
                pessoa = _service.ObterPorId(idPessoa);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"\nErro no banco: {ex.Message}");
            }

            if (pessoa is null)
            {
                Console.WriteLine("Pessoa não Encontrada!");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("\nDados atuais:=====================================================");
            Console.WriteLine($"  Nome : {pessoa.Nome, -35}");
            Console.WriteLine($"  Email: {pessoa.Email ?? "-",-35}");
            Console.WriteLine("====================================================================");
            Console.WriteLine("\nDeixe em branco para manter o valor atual.\n");

            Console.Write($"Novo nome [{pessoa.Nome}]: ");
            string novoNome = Console.ReadLine() ?? "";

            Console.Write($"Novo email [{pessoa.Email ?? "-"}]: ");
            string novoEmail = Console.ReadLine() ?? "";

            // Só troca o que foi digitado
            if (!string.IsNullOrWhiteSpace(novoNome))
                pessoa.Nome = novoNome;

            if (!string.IsNullOrWhiteSpace(novoEmail))
                pessoa.Email = novoEmail;

            Console.Write("\nConfirmar alteração? (S/N): ");
            string confirma = Console.ReadLine() ?? "";

            if (!confirma.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Alteração cancelada.");
                Console.ReadKey();
                return;
            }

            try
            {
                _service.Alterar(pessoa);

                Console.WriteLine("Pessoa alterada com sucesso!");
                Console.ReadKey();
                return;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro no banco: {ex.Message}");
                Console.ReadKey();
                return;
            }
        }
    
        static void TelaExcluir()
        {
            Console.Clear();
            Console.WriteLine("=== Excluir Pessoa ===");
            Console.WriteLine();
            Console.Write("Digite o Código da pessoal que deseja Excluir:");
            string entrada = Console.ReadLine() ?? "";

            if (!int.TryParse(entrada,out int idPessoa) || idPessoa <= 0)
            {
                Console.WriteLine("Digite um código numérico válido.");
                Console.ReadKey();
                return;
            }

            Pessoa? pessoa = null;

            try
            {
                pessoa = _service.ObterPorId(idPessoa);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"\nErro no banco: {ex.Message}");
            }

            if (pessoa is null)
            {
                Console.WriteLine("Pessoa não Encontrada!");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("\nPessoa:=====================================================");
            Console.WriteLine($"  Nome : {pessoa.Nome, -35}");
            Console.WriteLine($"  Email: {pessoa.Email ?? "-",-35}");
            Console.WriteLine("====================================================================");

            Console.Write("\nConfirmar exclusão? (S/N): ");
            string confirma = Console.ReadLine() ?? "";

            if (!confirma.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Exclusão cancelada.");
                Console.ReadKey();
                return;
            }

            try
            {
                _service.Excluir(pessoa);

                Console.WriteLine("Pessoa Excluida com sucesso!");
                Console.ReadKey();
                return;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro no banco: {ex.Message}");
                Console.ReadKey();
                return;
            }
        }
    }
}