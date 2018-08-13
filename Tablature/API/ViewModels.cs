using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public interface IViewModel<T>
    {
        T Base { get; }
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
        private IViewModel<T> core;

        private bool highlighted;
        public bool Highlighted
        {
            get { return highlighted; }
            set { SetProperty(ref highlighted, value); }
        }

        public ICommand HighlightCommand { get; set; }
        public T Base { get { return core.Base; } }

        public HighlightView(IViewModel<T> core)
        {
            this.core = core;
            HighlightCommand = new RelayCommand(handleItemSelected);
        }

        public void handleItemSelected()
        {
            Highlighted = !Highlighted;
        }
    }

    public class SelectView<T> : BaseViewModel, IViewModel<T>
    {
        private IViewModel<T> core;
        
        public ICommand SelectCommand { get; set; }
        public T Base { get { return core.Base; } }

        public SelectView(IViewModel<T> core)
        {
            this.core = core;
            SelectCommand = new RelayCommand(handleSelected);
        }

        public void handleSelected() { Selected?.Invoke(this, Base); }

        public event EventHandler<T> Selected;
    }

    public class ExpandView<T> : BaseViewModel, IViewModel<T>
    {
        private IViewModel<T> core;

        private bool expanded;
        public bool Expanded
        {
            get { return expanded; }
            set { SetProperty(ref expanded, value); }
        }

        public ICommand ExpandCommand { get; set; }
        public T Base { get { return core.Base; } }

        public ExpandView(IViewModel<T> core)
        {
            this.core = core;
            ExpandCommand = new RelayCommand(handleExpand);
        }

        public void handleExpand()
        {
            Expanded = !Expanded;
        }
    }

    public class EditView<T> : IViewModel<T>
    {
        private IViewModel<T> core;

        public ICommand EditCommand { get; set; }
        public T Base { get { return core.Base; } }

        public EditView(IViewModel<T> core)
        {
            this.core = core;
            EditCommand = new RelayCommand(handleEdit);
        }

        public void handleEdit() { Edited?.Invoke(this, Base); }

        public event EventHandler<T> Edited;
    }

    public class DeleteView<T> : IViewModel<T>
    {
        private IViewModel<T> core;

        public ICommand DeleteCommand { get; set; }
        public T Base { get { return core.Base; } }

        public DeleteView(IViewModel<T> core)
        {
            this.core = core;
        }

        public void handleDeleted() { Deleted?.Invoke(this, Base); }

        public event EventHandler<T> Deleted;
    }
}
