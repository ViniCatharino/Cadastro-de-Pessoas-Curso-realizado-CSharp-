using CadastroPessoas.Models;

namespace CadastroPessoas.Repositories
{
    public interface IPessoaRepository
    {
        void Adicionar(Pessoa pessoa);
        List<Pessoa> Consultar(string? nome);

        Pessoa? ObterPorId(int id);
        void Atualizar(Pessoa pessoa);

        void Excluir(Pessoa pessoa);
    }
}