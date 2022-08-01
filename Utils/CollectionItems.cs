using LibraryWebMVC.Models;

namespace LibraryWebMVC.Utils
{
    public class CollectionItems
    {
        public Author Author { get; set; }
        public Book Book { get; set; }

        public override string ToString()
        {
            return Author.ToString() + ';' + Book.ToString();
        }

    }
}
