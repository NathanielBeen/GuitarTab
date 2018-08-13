using GuitarTab;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class SearchParametersViewModel : BaseInputViewModel
    {
        private string error;
        public string Error
        {
            get { return error; }
            set { SetProperty(ref error, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                string error = NameError;
                setNonRequiredStringProperty(ref name, value, ref error);
                NameError = error;
            }
        }

        private string name_error;
        public string NameError
        {
            get { return name_error; }
            set { SetProperty(ref name_error, value); }
        }

        private string artist;
        public string Artist
        {
            get { return artist; }
            set
            {
                string error = ArtistError;
                setNonRequiredStringProperty(ref artist, value, ref error);
                ArtistError = error;
            }
        }

        private string artist_error;
        public string ArtistError
        {
            get { return artist_error; }
            set { SetProperty(ref artist_error, value); }
        }

        private string album;
        public string Album
        {
            get { return album; }
            set
            {
                string error = AlbumError;
                setNonRequiredStringProperty(ref album, value, ref error);
                AlbumError = error;
            }
        }

        private string album_error;
        public string AlbumError
        {
            get { return album_error; }
            set { SetProperty(ref album_error, value); }
        }

        private string author_name;
        public string AuthorName
        {
            get { return author_name; }
            set
            {
                string error = AuthorNameError;
                setNonRequiredStringProperty(ref author_name, value, ref error);
                AuthorNameError = error;
            }
        }

        private string author_name_error;
        public string AuthorNameError
        {
            get { return author_name_error; }
            set { SetProperty(ref author_name_error, value); }
        }

        private string rating;
        public string Rating
        {
            get { return rating; }
            set
            {
                string error = RatingError;
                setNonRequiredStringProperty(ref rating, value, ref error);
                RatingError = error;
            }
        }

        private string rating_error;
        public string RatingError
        {
            get { return rating_error; }
            set { SetProperty(ref rating_error, value); }
        }

        public ICommand SubmitCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        private TagSearch tag_search;

        public SearchParametersViewModel(TagSearch search)
        {
            tag_search = search;
        }

        private void handleClearSearch()
        {
            Name = String.Empty;
            NameError = String.Empty;
            Artist = String.Empty;
            ArtistError = String.Empty;
            Album = String.Empty;
            AlbumError = String.Empty;
            AuthorName = String.Empty;
            AuthorNameError = String.Empty;
            Rating = String.Empty;
            RatingError = String.Empty;
        }

        private void handleSubmitSearch()
        {
            if (hasError()) { return; }

            double? new_rating;
            if (Double.TryParse(Rating, out double result)) { new_rating = result; }
            else { new_rating = null; }

            string[] new_tags = tag_search.getSearchedTagNames();
            Result<SongModel> res = Utility.attemptSearch(Name, Artist, Album, AuthorName, new_rating, new_tags);

            if (res.Error != null) { Error = res.Error.Message; }
            else
            {
                Error = string.Empty;
                Searched?.Invoke(this, res.Items);
                handleClearSearch();
            }
        }

        private bool hasError()
        {
            return (NameError != String.Empty || ArtistError != String.Empty || AlbumError != String.Empty ||
                AuthorNameError != String.Empty || RatingError != String.Empty);
        }

        public event EventHandler<List<SongModel>> Searched;
    }

    public class TagSearch : BaseViewModel
    {
        public BaseModelCollection<TagModel> Selected { get; set; }
        public BaseModelCollection<TagModel> Unselected { get; set; }

        public ICommand ResetCommand { get; set; }

        public TagSearch(List<TagModel> all_tags)
        {
            createCollections(all_tags);
            ResetCommand = new RelayCommand(handleReset);
        }

        private void createCollections(List<TagModel> all_tags)
        {
            var factory = new TagVMFactory();
            Selected = new BaseModelCollection<TagModel>(factory, handleTagRemoved);
            Unselected = new BaseModelCollection<TagModel>(factory, handleTagAdded);

            Unselected.populateModels(all_tags);
        }

        private void handleTagAdded(TagModel model)
        {
            Selected.addViewVM(model);
            Unselected.removeVM(model);
        }

        private void handleTagRemoved(TagModel model)
        {
            Unselected.addViewVM(model);
            Selected.removeVM(model);
        }

        public string[] getSearchedTagNames()
        {
            return (from view in Selected.Collection select view.Base.Name).ToArray();
        }

        public void handleReset()
        {
            foreach (var view in Selected.Collection)
            {
                Selected.Collection.Remove(view);
                Unselected.Collection.Add(view);
            }
        }
    }

    public class SearchResult
    {
        public BaseModelCollection<SongModel> Result { get; }

        public SearchResult()
        {
            var factory = new SongVMFactory();
            Result = new BaseModelCollection<SongModel>(factory, handleSelected);
        }

        public void populateSearch(List<SongModel> models)
        {
            Result.populateModels(models);
        }

        public void clearSearch() { Result.Collection.Clear(); }

        private void handleSelected(SongModel model)
        {
            if (model == null) { return; }
            Selected?.Invoke(this, model);
        }

        public event EventHandler<SongModel> Selected;
    }
}
