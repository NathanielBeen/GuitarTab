using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class UserViewModel : IViewModel<UserModel>
    {
        public UserModel Base { get; }
        public VMType ViewType { get { return VMType.BASE; } }

        public int Id
        {
            get { return Base.Id; }
        }

        public string Name
        {
            get { return Base.Name; }
        }

        public int Type
        {
            get { return Base.Type; }
        }

        public UserViewModel(UserModel model)
        {
            Base = model;
        }
    }

    public class EditUserViewModel : BaseInputViewModel, IEditModel<UserModel>
    {
        public UserModel Base { get; }
        public VMType ViewType { get { return VMType.BASE_EDIT; } }

        public int Id { get { return Base.Id; } }

        public string Name { get { return Base.Name; } }

        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                string error = TypeError;
                setIntProperty(ref type, value, 0, 1, ref error);
                TypeError = error;
            }
        }

        private string type_error;
        public string TypeError
        {
            get { return type_error; }
            set { SetProperty(ref type_error, value); }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public event EventHandler<UserModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<UserModel>> ConfirmEdit;

        public EditUserViewModel(UserModel model)
        {
            Base = model;

            Type = model.Type.ToString();
            TypeError = "";
        }

        public void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
        }

        public void handleReset()
        {
            Type = Base.Type.ToString();
            TypeError = "";
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (TypeError != String.Empty) { return; }

            var updater = new UserUpdater(Int32.Parse(Type));
            var args = new UpdateEventArgs<UserModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }
    }
}
