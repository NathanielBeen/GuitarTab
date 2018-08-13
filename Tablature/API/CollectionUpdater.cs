using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public interface ICollectionUpdater
    {
        ICommand ClearCommand { get; set; }
        ICommand DeleteCommand { get; set; }
        ICommand AddCommand { get; set; }
        ICommand UpdateCommand { get; set; }

        void handleClear();
        void handleDelete();
        void handleAdd();
        void handleUpdate();
    }

    /* Users: Add
     * Tags: Add, Delete, Update
     * Songs: Add?, Delete, Update
     * Ratings: Delete
     */
    public class BaseCollectionUpdater<T> : ICollectionUpdater
    {
        protected BaseAdminModelCollection<T> collection;

        public ICommand ClearCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        public BaseCollectionUpdater(BaseAdminModelCollection<T> coll)
        {
            collection = coll;
        }

        //these will all be overridden, as different tables will have different options
        public void handleClear() { }

        public void handleAdd()
        {
            if (checkForErrors()) { return; }
            var add = createAdd();
            collection.handleAddRequest(add);
        }

        public void handleDelete() { collection.handleMultipleDeleteRequest(); }

        public void handleUpdate()
        {
            if (checkForErrors()) { return; }
            var update = createUpdate();
            collection.handleMultipleUpdateRequest(update);
        }

        protected bool checkForErrors() { return false; }

        protected IUpdater<T> createUpdate()
        {
            return null;
        }

        protected T createAdd()
        {
            return default(T);
        }
    }
}
