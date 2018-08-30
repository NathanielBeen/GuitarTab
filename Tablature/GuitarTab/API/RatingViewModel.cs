using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    //need a version that also gets the name of the author, not just the id
    public class RatingViewModel : IViewModel<RatingModel>
    {
        public RatingModel Base { get; }
        public VMType ViewType { get { return VMType.BASE; } }

        public int Id
        {
            get { return Base.Id; }
        }

        public int SongId
        {
            get { return Base.SongId; }
        }

        public int UserId
        {
            get { return Base.UserId; }
        }

        public double Rating
        {
            get { return Base.Rating; }
        }

        public string Text
        {
            get { return Base.Text; }
        }

        public RatingViewModel(RatingModel model)
        {
            Base = model;
        }
    }

    public class EditRatingViewModel : BaseInputViewModel, IEditModel<RatingModel>
    {
        public RatingModel Base { get; }

        public VMType ViewType { get { return VMType.BASE_EDIT; } }

        public int Id { get { return Base.Id; } }

        public int SongId { get { return Base.SongId; } }

        public int UserId { get { return Base.UserId; } }

        public DoubleInputField Rating { get; private set; }
        public StringInputField Text { get; private set; }

        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public event EventHandler<RatingModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<RatingModel>> ConfirmEdit;

        public EditRatingViewModel(RatingModel model)
        {
            Base = model;

            initFields();
            initCommands();
        }

        private void initFields()
        {
            Rating = new DoubleInputField("Rating", 0, 5);
            Text = new StringInputField("Text", 0, 256);
            Rating.Value = Base.Rating.ToString();
            Text.Value = Base.Text;
        }

        public void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
        }

        public void handleReset()
        {
            Rating.Value = Base.Rating.ToString();
            Text.Value = Base.Text;
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (Rating.hasErrors() || Text.hasErrors()) { return; }

            var updater = new RatingUpdater(Rating.getConvertedField(), Text.Value);
            var args = new UpdateEventArgs<RatingModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }
    }
}
