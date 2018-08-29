using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public interface IHandleRequests<T>
    {
        ModelMessageResult<T> handleEdited(T model, IUpdater<T> updater);
        MessageResult handleDeleted(T model);
        ModelMessageResult<T> handleAdded(T model);
    }

    public interface IHandleAdminRequests<T> : IHandleRequests<T>
    {
        MessageResult handleMultipleEdited(List<T> models, IUpdater<T> updater);
        MessageResult handleMultipleDeleted(List<T> models);
    }

    public interface IUpdater<T>
    {
        T createRequestModel(T model);
        T updateModel(T model);
    }
}
