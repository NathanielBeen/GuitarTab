using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class AdminRatingCollection : BaseInputViewModel
    {
        public BaseAdminModelCollection<RatingModel> Collection { get; }

        public ICommand DeleteCommand { get; set; }

        public AdminRatingCollection(BaseAdminModelCollection<RatingModel> collection)
        {
            Collection = collection;
            DeleteCommand = new RelayCommand(handleDelete);
        }

        private void handleDelete()
        {
            Collection.handleMultipleDeleteRequest();
        }
    }
}
