using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class SongAdminRequestHandler : IHandleAdminRequests<SongModel>
    {
        public ModelMessageResult<SongModel> handleEdited(SongModel model, IUpdater<SongModel> updater)
        {
            var req = new SongUpdateRequest(updater.createRequestModel(model));
            MessageResult result = APIRequest.adminUpdateSong(req).GetAwaiter().GetResult();

            Result<SongModel> get_result = APIRequest.getSongById(req.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<SongModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleDeleted(SongModel model)
        {
            return APIRequest.adminRemoveSong(model.Id).GetAwaiter().GetResult();
        }

        public ModelMessageResult<SongModel> handleAdded(SongModel model)
        {
            return new ModelMessageResult<SongModel>(null, "action not supported", null);
        }

        public MessageResult handleMultipleDeleted(List<SongModel> models)
        {
            var request = new MultipleSongIdRequest(models);
            return APIRequest.adminRemoveMultipleSong(request).GetAwaiter().GetResult();
        }

        public MessageResult handleMultipleEdited(List<SongModel> models, IUpdater<SongModel> updater)
        {
            var req_model = updater.createRequestModel(models.First());
            var request = new MultipleSongUpdateRequest(models, req_model);
            return APIRequest.adminUpdateMultipleSongs(request).GetAwaiter().GetResult();
        }
    }

    public class SongRequestHandler : IHandleRequests<SongModel>
    {
        public ModelMessageResult<SongModel> handleEdited(SongModel model, IUpdater<SongModel> updater)
        {
            var req = new SongUpdateRequest(updater.createRequestModel(model));
            MessageResult result = APIRequest.updateSong(req).GetAwaiter().GetResult();

            Result<SongModel> get_result = APIRequest.getSongById(req.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<SongModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }

        public MessageResult handleDeleted(SongModel model)
        {
            return APIRequest.deleteSong(model.Id).GetAwaiter().GetResult();
        }

        public ModelMessageResult<SongModel> handleAdded(SongModel model)
        {
            var req = new SongFieldsRequest(model);
            IDMessageResult result = APIRequest.createSong(req).GetAwaiter().GetResult();

            Result<SongModel> get_result = APIRequest.getSongById(result.Id).GetAwaiter().GetResult();
            return new ModelMessageResult<SongModel>(result.Error, result.Message, get_result.Items.FirstOrDefault());
        }
    }
}
