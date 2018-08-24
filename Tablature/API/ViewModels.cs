using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public enum VMType
    {
        BASE,
        BASE_EDIT,
        HIGHLIGHT,
        SELECT,
        EXPAND,
        EDIT,
        DELETE
    }

    public interface IViewModel<T>
    {
        T Base { get; }
        VMType ViewType { get; }
    }

    public interface IEditModel<T> : IViewModel<T>
    {
        ICommand CancelCommand { get; set; }
        ICommand ResetCommand { get; set; }
        ICommand ConfirmCommand { get; set; }

        void handleCancel();
        void handleReset();
        void handleConfirm();

        event EventHandler<T> CancelEdit;
        event EventHandler<UpdateEventArgs<T>> ConfirmEdit;
    }

    public class HighlightView<T> : BaseViewModel, IViewModel<T>
    {
        public IViewModel<T> Core { get; }
        public VMType ViewType { get { return VMType.HIGHLIGHT; } }

        private bool highlighted;
        public bool Highlighted
        {
            get { return highlighted; }
            set { SetProperty(ref highlighted, value); }
        }

        public ICommand HighlightCommand { get; set; }
        public T Base { get { return Core.Base; } }

        public HighlightView(IViewModel<T> core)
        {
            this.Core = core;
            HighlightCommand = new RelayCommand(handleItemSelected);
        }

        public void handleItemSelected()
        {
            Highlighted = !Highlighted;
        }
    }

    public class SelectView<T> : BaseViewModel, IViewModel<T>
    {
        public IViewModel<T> Core { get; }
        public VMType ViewType { get { return VMType.SELECT; } }

        public ICommand SelectCommand { get; set; }
        public T Base { get { return Core.Base; } }

        public SelectView(IViewModel<T> core)
        {
            this.Core = core;
            SelectCommand = new RelayCommand(handleSelected);
        }

        public void handleSelected() { Selected?.Invoke(this, Base); }

        public event EventHandler<T> Selected;
    }

    public class ExpandView<T> : BaseViewModel, IViewModel<T>
    {
        public IViewModel<T> Core { get; }
        public VMType ViewType { get { return VMType.EXPAND; } }

        private bool expanded;
        public bool Expanded
        {
            get { return expanded; }
            set { SetProperty(ref expanded, value); }
        }

        public ICommand ExpandCommand { get; set; }
        public T Base { get { return Core.Base; } }

        public ExpandView(IViewModel<T> core)
        {
            this.Core = core;
            ExpandCommand = new RelayCommand(handleExpand);
        }

        public void handleExpand()
        {
            Expanded = !Expanded;
        }
    }

    public class EditView<T> : IViewModel<T>
    {
        public IViewModel<T> Core { get; }
        public VMType ViewType { get { return VMType.EDIT; } }

        public ICommand EditCommand { get; set; }
        public T Base { get { return Core.Base; } }

        public EditView(IViewModel<T> core)
        {
            this.Core = core;
            EditCommand = new RelayCommand(handleEdit);
        }

        public void handleEdit() { Edited?.Invoke(this, Base); }

        public event EventHandler<T> Edited;
    }

    public class DeleteView<T> : IViewModel<T>
    {
        public IViewModel<T> Core { get; }
        public VMType ViewType { get { return VMType.DELETE; } }

        public ICommand DeleteCommand { get; set; }
        public T Base { get { return Core.Base; } }

        public DeleteView(IViewModel<T> core)
        {
            this.Core = core;
        }

        public void handleDeleted() { Deleted?.Invoke(this, Base); }

        public event EventHandler<T> Deleted;
    }
}
