using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class UserAdminRequestHandler : IHandleAdminRequests<UserModel>
    {
        public ModelMessageResult<UserModel> handleEdited(UserModel model, IUpdater<UserModel> updater)
        {
            var req = new UserUpdateRequest(updater.createRequestModel(model));
            MessageResult result = APIRequest.changeUserType(req).GetAwaiter().GetResult();

            Result<UserModel> get_result = APIRequest.getUserById(req.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<UserModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleDeleted(UserModel model)
        {
            var req = new UserUpdateRequest(model);
            return APIRequest.adminRemoveOtherAccount(req).GetAwaiter().GetResult();
        }

        public ModelMessageResult<UserModel> handleAdded(UserModel model)
        {
            var request = new UserFieldsRequest(model);

            IDMessageResult result;
            if (request.Type == 0) { result = APIRequest.createUser(request).GetAwaiter().GetResult(); }
            else { result = APIRequest.createAdmin(request).GetAwaiter().GetResult(); }

            Result<UserModel> get_result = APIRequest.getUserById(result.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<UserModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleMultipleDeleted(List<UserModel> models) { return new MessageResult() { Message = "action not supported" }; }

        public MessageResult handleMultipleEdited(List<UserModel> models, IUpdater<UserModel> updater) { return new MessageResult() { Message = "action not supported" }; }
    }
}
