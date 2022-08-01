
using System.ComponentModel;

namespace LibraryWebMVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        [DisplayName("Book name")]
        public string Name { get; set; }

        [DisplayName("Book year")]
        public int Year { get; set; }
        
        public int AuthorId { get; set; }
        
        public override string ToString()
        {
            return Name + ';' + Year;
        }

    }


}
