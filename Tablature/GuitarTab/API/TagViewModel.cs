using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class TagViewModel : IViewModel<TagModel>
    {
        public TagModel Base { get; }
        public VMType ViewType { get { return VMType.BASE; } }

        public int Id
        {
            get { return Base.Id; }
        }

        public string Name
        {
            get { return Base.Name; }
        }

        public string Type
        {
            get { return Base.Type; }
        }

        public TagViewModel(TagModel model)
        {
            Base = model;
        }
    }

    public class EditTagViewModel : BaseInputViewModel, IEditModel<TagModel>
    {
        public TagModel Base { get; }

        public StringInputField Name { get; private set; }
        public StringInputField Type { get; private set; }

        public VMType ViewType { get { return VMType.BASE_EDIT; } }

        public int Id { get { return Base.Id; } }

        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public event EventHandler<TagModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<TagModel>> ConfirmEdit;

        public EditTagViewModel(TagModel model)
        {
            Base = model;

            initFields(model);
            initCommands();
        }

        private void initFields(TagModel model)
        {
            Name = new StringInputField("Name", 1, 32);
            Type = new StringInputField("Type", 1, 32);

            Name.Value = model.Name;
            Type.Value = model.Type;
        }

        private void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
        }

        public void handleReset()
        {
            Name.Value = Base.Name;
            Type.Value = Base.Type;
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (Name.hasErrors() || Type.hasErrors()) { return; }

            var updater = TagUpdater.createNameTypeUpdater(Name.Value, Type.Value);
            var args = new UpdateEventArgs<TagModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }
    }

    public class SimpleTagViewModel : BaseViewModel
    {
        public string Name { get; }
        public string Type { get; }

        public NotificationField<bool> Selected { get; private set; }

        public ICommand SelectCommand { get; set; }

        public SimpleTagViewModel(string name, string type)
        {
            Name = name;
            Type = type;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Selected = new NotificationField<bool>();
            Selected.Value = false;
        }

        private void initCommands()
        {
            SelectCommand = new RelayCommand(handleSelected);
        }

        public void handleSelected()
        {
            Selected.Value = !Selected.Value;
        }
    }
}
