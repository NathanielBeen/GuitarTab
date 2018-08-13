﻿using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class AdminUserCollection : BaseInputViewModel
    {
        public BaseAdminModelCollection<UserModel> Collection { get; }

        public StringInputField Name;
        public StringInputField Password;

        private bool is_admin;
        public bool IsAdmin
        {
            get { return is_admin; }
            set { SetProperty(ref is_admin, value); }
        }

        public ICommand AddCommand { get; set; }

        public AdminUserCollection(BaseAdminModelCollection<UserModel> collection)
        {
            Collection = collection;
            AddCommand = new RelayCommand(handleAdded);
        }

        private void handleAdded()
        {
            if (Name.Error != String.Empty || Password.Error != String.Empty) { return; }

            int type = (IsAdmin) ? 1 : 0;
            var model = new UserModel(0, Name.Value, Password.Value, type);
        }
    }
}
