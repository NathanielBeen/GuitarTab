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

        public StringInputField UpdateType;

        public ICommand ClearCommand { get; set; }
        public ICommand ClearUpdateCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public AdminTagCollection(BaseAdminModelCollection<TagModel> collection)
        {
            Collection = collection;
            Name = new StringInputField("Name", 1, 64);
            Type = new StringInputField("Type", 1, 64);
            UpdateType = new StringInputField("Type", 1, 64);
            initCommands();
        }

        private void initCommands()
        {
            ClearCommand = new RelayCommand(clear);
            ClearUpdateCommand = new RelayCommand(clearUpdate);
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
            if (UpdateType.hasErrors()) { return; }
            IUpdater<TagModel> updater = TagUpdater.createTypeUpdater(UpdateType.Value);
            Collection.handleMultipleUpdateRequest(updater);
            clear();
        }

        private void handleAdd()
        {
            if (Type.hasErrors() || Name.hasErrors()) { return; }
            TagModel model = new TagModel(0, Name.Value, Type.Value);
            Collection.handleAddRequest(model);
            clear();
        }

        private void clear()
        {
            Name.clearField();
            Type.clearField();
        }

        private void clearUpdate()
        {
            UpdateType.clearField();
        }
    }
}
