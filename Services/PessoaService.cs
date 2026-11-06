using CadastroPessoas.Models;
using CadastroPessoas.Repositories;

namespace CadastroPessoas.Services
{
    public class PessoaService
    {
        private readonly IPessoaRepository _repository;

        public PessoaService(IPessoaRepository repository)
        {
            _repository = repository;
        }

        public void Adicionar(string nome, string? email)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome é obrigatório!");

            var pessoa = new Pessoa
            {
                Nome = nome.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim()
            };

            _repository.Adicionar(pessoa);
        }
        public List<Pessoa> Consultar(string? nome)
        {
            return _repository.Consultar(nome?.Trim());
        }

        public Pessoa? ObterPorId(int id) => _repository.ObterPorId(id);

        public void Alterar(Pessoa pessoa)
        {
            if (string.IsNullOrWhiteSpace(pessoa.Nome))
                throw new ArgumentException("O nome é obrigatório!");

            pessoa.Nome  = pessoa.Nome.Trim();
            pessoa.Email = string.IsNullOrWhiteSpace(pessoa.Email) ? null : pessoa.Email.Trim();

            _repository.Atualizar(pessoa);
        }

        public void Excluir(Pessoa pessoa)
        {
            _repository.Excluir(pessoa);
        }
    }
}