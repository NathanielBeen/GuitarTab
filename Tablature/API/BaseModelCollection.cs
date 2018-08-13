using GuitarTab;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class BaseModelCollection<T> : BaseViewModel
    {
        protected BaseViewModelFactory<T> factory;
        protected Action<T> action;

        public ObservableCollection<IViewModel<T>> Collection { get; set; }

        public BaseModelCollection(BaseViewModelFactory<T> fac, Action<T> select_action)
        {
            Collection = new ObservableCollection<IViewModel<T>>();
            factory = fac;
            action = select_action;

            factory.setSelectDelegate(handleSelected);
        }

        public void populateModels(List<T> models)
        {
            foreach (T model in models) { Collection.Add(factory.createStandardVM(model)); }
        }

        protected void handleSelected(T model) { action?.Invoke(model); }

        public void addViewVM(T model)
        {
            var standard_model = factory.createStandardVM(model);
            if (standard_model != null)
            {
                Collection.Add(standard_model);
            }
        }

        public void removeVM(T model)
        {
            Collection.Remove(Collection.Where(m => m.Base.Equals(model)).First());
        }
    }

    public class BaseAdminModelCollection<T> : BaseViewModel
    {
        private IHandleAdminRequests<T> request_handler;
        private BaseAdminViewModelFactory<T> factory;

        private Error error;
        public Error Error
        {
            get { return error; }
            set { SetProperty(ref error, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ObservableCollection<IViewModel<T>> Collection { get; set; }
       
        public BaseAdminModelCollection(IHandleAdminRequests<T> req, BaseAdminViewModelFactory<T> fac)
        {
            Collection = new ObservableCollection<IViewModel<T>>();
            request_handler = req;
            factory = fac;

            factory.setEventHandlers(handleBeginEdit, handleCancelEdit, handleDeleteRequest, handleUpdateRequest);
        }

        public void populateModels(List<T> models)
        {
            foreach (T model in models) { Collection.Add(factory.createStandardVM(model)); }
        }

        private void setErrorAndMessage(MessageResult result)
        {
            Error = result.Error;
            Message = result.Message;
        }

        private List<T> getHighlightedVMs()
        {
            return (from model in Collection
                    where (model as HighlightView<T>)?.Highlighted ?? false
                    select model.Base).ToList();
        }

        private void replaceWithEditVM(T old_model, T new_model)
        {
            var edit_model = factory.createEditVM(new_model);
            if (edit_model != null)
            {
                int index = Collection.IndexOf(Collection.Where(m => m.Base.Equals(old_model)).First());
                Collection.RemoveAt(index);
                Collection.Insert(index, edit_model);
            }
        }

        private void replaceWithViewVm(T old_model, T new_model)
        {
            var standard_model = factory.createStandardVM(new_model);
            if (standard_model != null)
            {
                int index = Collection.IndexOf(Collection.Where(m => m.Base.Equals(old_model)).First());
                Collection.RemoveAt(index);
                Collection.Insert(index, standard_model);
            }
        }

        private void addViewVM(T model)
        {
            var standard_model = factory.createStandardVM(model);
            if (standard_model != null)
            {
                Collection.Add(standard_model);
            }
        }

        private void removeVM(T model)
        {
            Collection.Remove(Collection.Where(m => m.Base.Equals(model)).First());
        }

        private void removeMultipleVM(List<T> models)
        {
            var to_remove = Collection.Where(m => models.Contains(m.Base));
            foreach (var item in to_remove)
            {
                Collection.Remove(item);
            }
        }

        private void handleUpdateRequest(T model, IUpdater<T> updater)
        {
            ModelMessageResult<T> response = request_handler.handleEdited(model, updater);

            if (response.Error == null)
            {
                replaceWithViewVm(model, response.NewModel);
            }
            setErrorAndMessage(response);
        }

        private void handleBeginEdit(T model)
        {
            replaceWithEditVM(model, model);
        }

        private void handleCancelEdit(T model)
        {
            replaceWithViewVm(model, model);
        }

        private void handleDeleteRequest(T model)
        {
            MessageResult response = request_handler.handleDeleted(model);

            if (response.Error == null)
            {
                removeVM(model);
            }
            setErrorAndMessage(response);
        }

        public void handleAddRequest(T model)
        {
            ModelMessageResult<T> response = request_handler.handleAdded(model);

            if (response.Error == null)
            {
                addViewVM(response.NewModel);
            }
            setErrorAndMessage(response);
        }

        public void handleMultipleDeleteRequest()
        {
            var model_list = getHighlightedVMs();
            MessageResult response = request_handler.handleMultipleDeleted(model_list);

            if (response.Error == null)
            {
                removeMultipleVM(model_list);
            }
            setErrorAndMessage(response);
        }

        public void handleMultipleUpdateRequest(IUpdater<T> updater)
        {
            var model_list = getHighlightedVMs();
            MessageResult response = request_handler.handleMultipleEdited(model_list, updater);

            if (response.Error == null)
            {
                foreach (T model in model_list)
                {
                    replaceWithViewVm(model, updater.updateModel(model));
                }
            }
            setErrorAndMessage(response);
        }
    }

    public delegate void CollectionEventDelegate<T>(T model);
    public delegate void CollectionUpdateEventDelegate<T>(T model, IUpdater<T> updater);
}
