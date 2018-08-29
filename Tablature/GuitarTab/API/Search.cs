using GuitarTab;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class SearchParametersViewModel : BaseInputViewModel
    {
        public NonRequiredStringInputField Name;
        public NonRequiredStringInputField Artist;
        public NonRequiredStringInputField Album;
        public NonRequiredStringInputField Author;
        public double Rating { get; set; }

        public TagSearch TagSearch { get; set; }

        public SearchParametersViewModel(TagSearch search)
        {
            TagSearch = search;

            Name = new NonRequiredStringInputField("Name", 0, 32);
            Artist = new NonRequiredStringInputField("Artist", 0, 32);
            Album = new NonRequiredStringInputField("Album", 0, 32);
            Author = new NonRequiredStringInputField("Author", 0, 32);
            Rating = 0;
        }

        public void clearSearch()
        {
            Name.clearField();
            Artist.clearField();
            Album.clearField();
            Author.clearField();
            Rating = 0;
            TagSearch.handleReset();
        }

        public SearchTerms getSearchTerms()
        {
            if (hasError()) { return null; }
            string[] new_tags = TagSearch.getSearchedTagNames();
            return new SearchTerms(Name.Value, Artist.Value, Author.Value, Rating, new_tags);
        }

        private bool hasError()
        {
            return (Name.hasErrors() || Artist.hasErrors() || Album.hasErrors() || Author.hasErrors());
        }
    }

    public class TagSearch : BaseViewModel
    {
        public BaseModelCollection<TagModel> Selected { get; set; }
        public BaseModelCollection<TagModel> Unselected { get; set; }

        public TagSearch(List<TagModel> all_tags)
        {
            createCollections(all_tags);
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

    public class SearchTerms
    {
        public string Name { get; }
        public string Artist { get; }
        public string Album { get; }
        public string Author { get; }
        public double Rating { get; }
        public string[] Tags { get; }
        
        public SearchTerms(string name, string artist, string author, double rating, string[] tags)
        {
            Name = name;
            Artist = artist;
            Album = Album;
            Author = author;
            Rating = rating;
            Tags = tags;
        }
    }
}
