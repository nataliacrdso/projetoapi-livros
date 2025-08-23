namespace ApiRestEf.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Editora { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public int Ano { get; set; }
    }
}