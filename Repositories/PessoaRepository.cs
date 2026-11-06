using CadastroPessoas.Models;
using Microsoft.Data.SqlClient;
using System.Data;
    
namespace CadastroPessoas.Repositories
{
    public class PessoaRepository : IPessoaRepository
    {

        private readonly SqlConnectionFactory _connectionFactory;

        public PessoaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Adicionar(Pessoa pessoa)
        {
            using var conexao = _connectionFactory.CreateConnection();
            conexao.Open();

            string sql = "INSERT INTO dbo.Pessoas(Nome,Email,AlteradoEm)"+
                         "VALUES(@Nome,@Email,@AlteradoEm)               ";

            using var qryAdicionar = new SqlCommand(sql, conexao);
            qryAdicionar.Parameters.AddWithValue("@Nome"        ,  pessoa.Nome);
            qryAdicionar.Parameters.AddWithValue("@Email"       , (object?)pessoa.Email ?? DBNull.Value);
            qryAdicionar.Parameters.AddWithValue("@AlteradoEm"  , DateTime.Now);

            qryAdicionar.ExecuteNonQuery();
        }

        public List<Pessoa> Consultar(string? nome)
        {
            var lista = new List<Pessoa>();

            using var conexao = _connectionFactory.CreateConnection();
            conexao.Open();

            var sql = "SELECT ID                          " +
                      "      ,Nome                        " +
                      "      ,Email                       " +
                      "      ,CriadoEm                    " +
                      "      ,AlteradoEm                  " +
                      "FROM dbo.Pessoas WITH (NOLOCK)     " +
                      "WHERE @Nome = ''                   " +
                      "  OR  Pessoas.Nome LIKE '%' + @Nome + '%' ";

            using var qryConsultar = new SqlCommand( sql, conexao);
            qryConsultar.Parameters.Add("@Nome", SqlDbType.VarChar, 100).Value = nome ?? "";

            using var reader = qryConsultar.ExecuteReader();

            while(reader.Read())
            {
                lista.Add(new Pessoa
                {
                    Id         = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nome       = reader.GetString(reader.GetOrdinal("Nome")),
                    Email      = reader.IsDBNull(reader.GetOrdinal("Email"))
                                 ? null
                                 : reader.GetString(reader.GetOrdinal("Email")),
                    CriadoEm   = reader.GetDateTime(reader.GetOrdinal("CriadoEm")),
                    AlteradoEm = reader.GetDateTime(reader.GetOrdinal("AlteradoEm"))
                });
            }

            return lista;

        }
    
        public Pessoa? ObterPorId(int id)
        {
            using var conexao = _connectionFactory.CreateConnection();
            conexao.Open();

            var sql = "SELECT ID                          " +
                      "      ,Nome                        " +
                      "      ,Email                       " +
                      "FROM dbo.Pessoas WITH (NOLOCK)     " +
                      "WHERE ID = @Id                     ";
            
            using var qryConsultarId = new SqlCommand(sql, conexao);
            qryConsultarId.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var reader = qryConsultarId.ExecuteReader();

            if (!reader.Read()) return null;

            return new Pessoa
            {
                Id         = reader.GetInt32(reader.GetOrdinal("Id")),
                Nome       = reader.GetString(reader.GetOrdinal("Nome")),
                Email      = reader.IsDBNull(reader.GetOrdinal("Email")) 
                             ? null
                             : reader.GetString(reader.GetOrdinal("Email")),
                AlteradoEm = DateTime.Now                
            };
        }
    
        public void Atualizar(Pessoa pessoa)
        {
            using var conexao = _connectionFactory.CreateConnection();
            conexao.Open();

            string sql = "UPDATE dbo.Pessoas SET Nome       = @Nome       " +
                         "                      ,Email      = @Email      " +
                         "                      ,AlteradoEm = @AlteradoEm " +
                         "WHERE Id = @Id                                  " ;
            
            using var qryAtualizar = new SqlCommand(sql,conexao);
            qryAtualizar.Parameters.Add("@Id"        , SqlDbType.Int).Value      = pessoa.Id;
            qryAtualizar.Parameters.Add("@Nome"      , SqlDbType.VarChar).Value  = pessoa.Nome;
            qryAtualizar.Parameters.Add("@Email", SqlDbType.VarChar).Value =
                                       (object?)pessoa.Email ?? DBNull.Value;
            qryAtualizar.Parameters.Add("@AlteradoEm", SqlDbType.DateTime).Value = pessoa.AlteradoEm;

            qryAtualizar.ExecuteNonQuery();
        }

        public void Excluir(Pessoa pessoa)
        {
            using var conexao = _connectionFactory.CreateConnection();
            conexao.Open(); 

            string sql = "DELETE FROM dbo.Pessoas " +
                         "WHERE Id = @Id          " ;

            using var qryExcluir = new SqlCommand(sql, conexao);
            qryExcluir.Parameters.Add("@Id", SqlDbType.Int).Value = pessoa.Id;

            qryExcluir.ExecuteNonQuery();   
        }
    }
}