using LibraryWebMVC.Utils;

namespace LibraryWebMVC.ViewModels
{
    public class CollectionItemsViewModel
    {
        public IEnumerable<CollectionItems> CollectionItems { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }

    }
}
