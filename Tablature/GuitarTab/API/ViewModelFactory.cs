using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public abstract class BaseViewModelFactory<T>
    {
        private CollectionEventDelegate<T> select_delegate;

        public BaseViewModelFactory() { }

        public void setSelectDelegate(CollectionEventDelegate<T> select) { select_delegate = select; }

        protected IViewModel<T> addSelectLayer(IViewModel<T> core)
        {
            var select = new SelectView<T>(core);
            select.Selected += ((s, m) => select_delegate?.Invoke(m));
            return select;
        }

        protected IViewModel<T> addExpandLayer(IViewModel<T> core)
        {
            return new ExpandView<T>(core);
        }

        public abstract IViewModel<T> createStandardVM(T model);
        public abstract IViewModel<T> createEditVM(T model);
    }

    public class TagVMFactory : BaseViewModelFactory<TagModel>
    {
        public TagVMFactory() : base() { }

        public override IViewModel<TagModel> createStandardVM(TagModel model)
        {
            var core = new TagViewModel(model);
            return addSelectLayer(core);
        }

        public override IViewModel<TagModel> createEditVM(TagModel model)
        {
            throw new NotImplementedException();
        }
    }

    public class SongVMFactory : BaseViewModelFactory<SongModel>
    {
        public SongVMFactory() : base() { }

        public override IViewModel<SongModel> createStandardVM(SongModel model)
        {
            var core = new SongViewModel(model);
            var expand = addExpandLayer(core);
            return addSelectLayer(expand);
        }

        public override IViewModel<SongModel> createEditVM(SongModel model)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingVMFactory : BaseViewModelFactory<RatingModel>
    {
        public RatingVMFactory() : base() { }

        public override IViewModel<RatingModel> createStandardVM(RatingModel model)
        {
            var core = new RatingViewModel(model);
            var expand = addExpandLayer(core);
            return addSelectLayer(expand);
        }

        public override IViewModel<RatingModel> createEditVM(RatingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
