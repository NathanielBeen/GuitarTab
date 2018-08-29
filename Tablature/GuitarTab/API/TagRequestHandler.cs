using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class TagAdminRequestHandler : IHandleAdminRequests<TagModel>
    {
        public ModelMessageResult<TagModel> handleEdited(TagModel model, IUpdater<TagModel> updater)
        {
            var req = new TagUpdateRequest(updater.createRequestModel(model));
            MessageResult result = APIRequest.updateTag(req).GetAwaiter().GetResult();

            Result<TagModel> get_result = APIRequest.getTagById(req.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<TagModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleDeleted(TagModel model)
        {
            return APIRequest.removeTag(model.Id).GetAwaiter().GetResult();
        }

        public ModelMessageResult<TagModel> handleAdded(TagModel model)
        {
            var req = new TagFieldsRequest(model);
            IDMessageResult result = APIRequest.createTag(req).GetAwaiter().GetResult();

            Result<TagModel> get_result = APIRequest.getTagById(result.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<TagModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleMultipleDeleted(List<TagModel> models)
        {
            var request = new MultipleTagIdRequest(models);
            return APIRequest.removeMultipleTags(request).GetAwaiter().GetResult();
        }

        public MessageResult handleMultipleEdited(List<TagModel> models, IUpdater<TagModel> updater)
        {
            var req_model = updater.createRequestModel(models.First());
            var req = new MultipleTagUpdateRequest(models, req_model);
            return APIRequest.updateMultipleTags(req).GetAwaiter().GetResult();
        }
    }
}
