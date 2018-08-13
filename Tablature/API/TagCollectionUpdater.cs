using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class AdminTagCollection : BaseInputViewModel
    {
        public BaseAdminModelCollection<TagModel> Collection { get; }

        public StringInputField Name;
        public StringInputField Type;

        public ICommand ClearCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public AdminTagCollection(BaseAdminModelCollection<TagModel> collection)
        {
            Collection = collection;
            Name = new StringInputField("Name", 1, 64);
            Type = new StringInputField("Type", 1, 64);
            initCommands();
        }

        private void initCommands()
        {
            ClearCommand = new RelayCommand(clear);
            ConfirmCommand = new RelayCommand(handleUpdate);
            AddCommand = new RelayCommand(handleAdd);
            DeleteCommand = new RelayCommand(handleDelete);
        }

        private void handleDelete()
        {
            Collection.handleMultipleDeleteRequest();
        }

        private void handleUpdate()
        {
            if (Type.Error != String.Empty) { return; }
            IUpdater<TagModel> updater = TagUpdater.createTypeUpdater(Type.Value);
            Collection.handleMultipleUpdateRequest(updater);
            clear();
        }

        private void handleAdd()
        {
            if (Type.Error != String.Empty || Name.Error != String.Empty) { return; }
            TagModel model = new TagModel(0, Name.Value, Type.Value);
            Collection.handleAddRequest(model);
            clear();
        }

        private void clear()
        {
            Name.clearField();
            Type.clearField();
        }
    }
}
