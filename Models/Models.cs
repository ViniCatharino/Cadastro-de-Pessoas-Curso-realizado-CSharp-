namespace CadastroPessoas.Models
{
    public class Pessoa
    {
        public int Id {get; set;}
        public string? Nome {get; set;}
        public string? Email {get; set;}
        public DateTime? CriadoEm {get; set;}
        public DateTime? AlteradoEm {get; set;}
    }
}