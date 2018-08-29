using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class RatingAdminRequestHandler : IHandleAdminRequests<RatingModel>
    {
        public ModelMessageResult<RatingModel> handleEdited(RatingModel model, IUpdater<RatingModel> updater)
        {
            return new ModelMessageResult<RatingModel>(null, "action not supported", null);
        }

        public MessageResult handleDeleted(RatingModel model)
        {
            return APIRequest.adminRemoveRating(model.Id).GetAwaiter().GetResult();
        }

        public ModelMessageResult<RatingModel> handleAdded(RatingModel model)
        {
            return new ModelMessageResult<RatingModel>(null, "action not supported", null);
        }

        public MessageResult handleMultipleDeleted(List<RatingModel> models)
        {
            var request = new MultipleRatingIdRequest(models);
            return APIRequest.adminRemoveMultipleRatings(request).GetAwaiter().GetResult();
        }

        public MessageResult handleMultipleEdited(List<RatingModel> models, IUpdater<RatingModel> updater)
        {
            return new MessageResult() { Message = "action not supported" };
        }
    }

    public class RatingRequestHandler : IHandleRequests<RatingModel>
    {
        public ModelMessageResult<RatingModel> handleEdited(RatingModel model, IUpdater<RatingModel> updater)
        {
            var req = new RatingUpdateRequest(updater.createRequestModel(model));
            MessageResult result = APIRequest.updateRating(req).GetAwaiter().GetResult();

            Result<RatingModel> get_result = APIRequest.getRatingById(req.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<RatingModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleDeleted(RatingModel model)
        {
            return APIRequest.removeRating(model.Id).GetAwaiter().GetResult();
        }

        public ModelMessageResult<RatingModel> handleAdded(RatingModel model)
        {
            var req = new RatingFieldsRequest(model);
            IDMessageResult result = APIRequest.createRating(req).GetAwaiter().GetResult();

            Result<RatingModel> get_result = APIRequest.getRatingById(result.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<RatingModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }
    }
}
