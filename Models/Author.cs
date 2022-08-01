


using System.ComponentModel;

namespace LibraryWebMVC.Models
{
    public class Author
    {
        public int Id { get; set; }

        [DisplayName("Firstname")]
        public string FirstName { get; set; }

        [DisplayName("Lastname")]
        public string LastName { get; set; }

        [DisplayName("Surname")]
        public string SurName { get; set; } = "";

        [DisplayName("Birthdate")]
        public DateTime BirthDate { get; set; }

        public override string ToString()
        {
            return FirstName + ';' + LastName + ';' + SurName + ';' + BirthDate;
        }

    }


}
