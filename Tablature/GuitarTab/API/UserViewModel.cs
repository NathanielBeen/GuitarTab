using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
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

        public StringInputField Type { get; private set; }

        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public event EventHandler<UserModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<UserModel>> ConfirmEdit;

        public EditUserViewModel(UserModel model)
        {
            Base = model;

            initFields(model);
            initCommands();
        }

        private void initFields(UserModel model)
        {
            Type = new StringInputField("Type", 1, 32);
            Type.Value = model.Type.ToString();
        }

        private void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
        }

        public void handleReset()
        {
            Type.Value = Base.Type.ToString();
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (Type.hasErrors()) { return; }

            var updater = new UserUpdater(Int32.Parse(Type.Value));
            var args = new UpdateEventArgs<UserModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }
    }
}
