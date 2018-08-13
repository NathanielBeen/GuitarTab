using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class TagViewModel : IViewModel<TagModel>
    {
        public TagModel Base { get; }

        public int Int
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

        public int Id { get { return Base.Id; } }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                string error = NameError;
                setStringProperty(ref name, value, ref error);
                NameError = error;
            }
        }

        private string name_error;
        public string NameError
        {
            get { return name_error; }
            set { SetProperty(ref name_error, value); }
        }

        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                string error = TypeError;
                setStringProperty(ref type, value, ref error);
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

        public event EventHandler<TagModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<TagModel>> ConfirmEdit;

        public EditTagViewModel(TagModel model)
        {
            Base = model;

            Name = Base.Name;
            NameError = "";
            Type = Base.Type;
            TypeError = "";

            initCommands();
        }

        public void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
        }

        public void handleReset()
        {
            Name = Base.Name;
            NameError = "";
            Type = Base.Type;
            TypeError = "";
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (NameError != String.Empty || TypeError != String.Empty) { return; }

            var updater = TagUpdater.createNameTypeUpdater(Name, Type);
            var args = new UpdateEventArgs<TagModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }
    }

    public class SimpleTagViewModel : BaseViewModel
    {
        public string Name { get; }
        public string Type { get; }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }

        public ICommand SelectCommand { get; set; }

        public SimpleTagViewModel(string name, string type)
        {
            Name = name;
            Type = type;
            Selected = false;

            SelectCommand = new RelayCommand(handleSelected);
        }

        public void handleSelected()
        {
            Selected = !Selected;
        }
    }
}
