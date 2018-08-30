using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class AdminUserCollection : BaseInputViewModel
    {
        public BaseAdminModelCollection<UserModel> Collection { get; }

        public StringInputField Name { get; private set; }
        public StringInputField Password { get; private set; }
        public NotificationField<bool> IsAdmin { get; private set; }

        public ICommand AddCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public AdminUserCollection(BaseAdminModelCollection<UserModel> collection)
        {
            Collection = collection;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Name = new StringInputField("Name", 4, 32);
            Pasword = new StringInputField("Password", 6, 32);
            IsAdmin = new NotificationField<bool>();
        }

        private void initCommands()
        {
            AddCommand = new RelayCommand(handleAdded);
            ClearCommand = new RelayCommand(clear);
        }

        private void handleAdded()
        {
            if (Name.hasErrors() || Password.hasErrors()) { return; }

            int type = (IsAdmin.Value) ? 1 : 0;
            var model = new UserModel(0, Name.Value, Password.Value, type);
        }

        private void clear()
        {
            Name.clearField();
            Password.clearField();
        }
    }
}
