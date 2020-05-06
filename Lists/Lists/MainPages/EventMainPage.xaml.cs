using ISQLiteBase;
using Lists.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lists
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EventMainPage : ContentPage
	{
        private SQLiteAsyncConnection _connection;

        private ObservableCollection<Event> _eve;

        private bool _isDataLoaded;

        public EventMainPage ()
		{
			InitializeComponent ();
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
            await _connection.CreateTableAsync<Event>();

            var eve = await _connection.Table<Event>().ToListAsync();
            
            _eve = new ObservableCollection<Event>(eve);
            listView.ItemsSource = _eve;
        }

        async private void OnAdd(object sender, EventArgs e)
        {
            string date = DateTime.Today.ToString("dd.MM.yyyy");
            string sortdate = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

            var page = new EventDetails(new Event { Label = "New Event", Date = date, SortDate = sortdate, StartDate=DateTime.Today});
            page.EventAdded += (source, eve) =>
            {
                _eve.Add(eve);
            };

            await Navigation.PushAsync(page);
        }

        async void Selected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var selectedEvent = e.SelectedItem as Event;

            listView.SelectedItem = null;
            var page = new EventDetails(selectedEvent);
            page.EventUpdated += (source, eve) =>
            {
                selectedEvent.Id = eve.Id;
                selectedEvent.Label = eve.Label;
                selectedEvent.StartDate = eve.StartDate;
                selectedEvent.EndDate = eve.EndDate;
                selectedEvent.Details = eve.Details;
            };

            await Navigation.PushAsync(page);
        }

        async private void OnSort(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Sort by...", null, null, "Name", "Date");
            if (action == "Name")
            {
                listView.ItemsSource = _eve.OrderBy(c => c.Label);
            }
            else if (action == "Date")
            {
                listView.ItemsSource = _eve.OrderBy(c => c.SortDate);
            }
        }

        async void OnDelete(object sender, EventArgs e)
        {
            var eve = (sender as MenuItem).CommandParameter as Event;
            if (await DisplayAlert("Warning", $"Are you sure you want to delete {eve.Label}?", "Yes", "No"))
            {
                _eve.Remove(eve);
                await _connection.DeleteAsync(eve);
            }
        }
       
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = GetNote(e.NewTextValue);
        }

        IEnumerable<Event> GetNote(string searchText = null)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return _eve;
            }
            return _eve.Where(c => c.Label.Contains(searchText));
        }

        async private void OnInfo(object sender, EventArgs e)
        {
            var page = new InfoPage();
            await Navigation.PushAsync(page);
        }
    }
}