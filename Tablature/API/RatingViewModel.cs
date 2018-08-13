using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class RatingViewModel : IViewModel<RatingModel>
    {
        public RatingModel Base { get; }

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

        public int Id { get { return Base.Id; } }

        public int SongId { get { return Base.SongId; } }

        public int UserId { get { return Base.UserId; } }

        private string rating;
        public string Rating
        {
            get { return rating; }
            set
            {
                string error = RatingError;
                setDoubleProperty(ref rating, value, 0, 5, ref error);
                RatingError = error;
            }
        }

        private string rating_error;
        public string RatingError
        {
            get { return rating_error; }
            set { SetProperty(ref rating_error, value); }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                string error = TextError;
                setStringProperty(ref text, value, ref error);
                TextError = error;
            }
        }

        private string text_error;
        public string TextError
        {
            get { return text_error; }
            set { SetProperty(ref text_error, value); }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public event EventHandler<RatingModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<RatingModel>> ConfirmEdit;

        public EditRatingViewModel(RatingModel model)
        {
            Base = model;

            Rating = model.Rating.ToString();
            RatingError = "";
            Text = model.Text;
            TextError = "";

            initCommands();
        }

        public void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
        }

        public void handleReset()
        {
            Rating = Base.Rating.ToString();
            RatingError = "";
            Text = Base.Text;
            TextError = "";
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (RatingError != String.Empty || TextError != String.Empty) { return; }

            var updater = new RatingUpdater(Double.Parse(Rating), Text);
            var args = new UpdateEventArgs<RatingModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }
    }
}
