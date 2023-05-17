using System.ComponentModel.DataAnnotations;

namespace PustokStart.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string FullName { get; set; }
        public List<Book> Books { get; set; }
    }
}
