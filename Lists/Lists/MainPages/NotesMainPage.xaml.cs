using Lists.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ISQLiteBase;

namespace Lists
{
    public partial class MainPageNotes : ContentPage
    {
        private SQLiteAsyncConnection _connection;

        private ObservableCollection<Notes> _note;

        private bool _isDataLoaded;

        public MainPageNotes()
        {
            InitializeComponent();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            if (_isDataLoaded)
                return;

            _isDataLoaded = true;
            await LoadData();
            base.OnAppearing();
        }

        private async Task LoadData()
        {
            await _connection.CreateTableAsync<Notes>();

            var note = await _connection.Table<Notes>().ToListAsync();

            _note = new ObservableCollection<Notes>(note);
            listView.ItemsSource = _note;
        }

        IEnumerable<Notes> GetNote(string searchText = null)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return _note;
            }
            return _note.Where(n => n.Label.Contains(searchText));
        }

        async private void OnInfo(object sender, EventArgs e)
        {
            var page = new InfoPage();
            await Navigation.PushAsync(page);
        }

        async private void OnSort(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Sort by...", null, null, "Label", "Date");
            if (action == "Label")
            {
                listView.ItemsSource = _note.OrderBy(c => c.Label);
            }
            else if (action == "Date")
            {
                listView.ItemsSource = _note.OrderBy(c => c.SortDate);
            }
        }

        async void OnDelete(object sender, EventArgs e)
        {
            var note = (sender as MenuItem).CommandParameter as Notes;
            if (await DisplayAlert("Warning", $"Are you sure you want to delete {note.Label}?", "Yes", "No"))
            {
                _note.Remove(note);
                await _connection.DeleteAsync(note);
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = GetNote(e.NewTextValue);
        }

        async private void OnAdd(object sender, EventArgs e)
        {
            string date = DateTime.Today.ToString("dd.MM.yyyy");
            string sortdate = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            
            var page = new DetailsNote(new Notes { Label = "Text note ", Date = date, SortDate=sortdate});
            page.NoteAdded += (source, note) =>
            {
                _note.Add(note);
            };

            await Navigation.PushAsync(page);
        }

        async void Selected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var selectedNote = e.SelectedItem as Notes;

            listView.SelectedItem = null;

            var page = new DetailsNote(selectedNote);
            page.NoteUpdated += (source, note) =>
            {
                selectedNote.Id = note.Id;
                selectedNote.Label = note.Label;
                selectedNote.TextNote = note.TextNote;
                selectedNote.Date = note.Date;
            };
            await Navigation.PushAsync(page);
        }
    }
}
