using Lists.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ISQLiteBase;
using Xamarin.Essentials;

namespace Lists
{
    public partial class MainPageWeb : ContentPage
    {
        private SQLiteAsyncConnection _connection;

        private ObservableCollection<Web> _web;

        private bool _isDataLoaded;

        public MainPageWeb()
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
            await _connection.CreateTableAsync<Web>();

            var web = await _connection.Table<Web>().ToListAsync();

            _web = new ObservableCollection<Web>(web);
            listView.ItemsSource = _web;
        }

        async private void OnAdd(object sender, EventArgs e)
        {
            string date = DateTime.Today.ToString("dd.MM.yyyy");
            string sortdate = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            var page = new DetailsWeb(new Web { Label = "Web-page", Date=date, SortDate=sortdate});
            page.WebAdded += (source, web) =>
                {
                    _web.Add(web);
                };
            await Navigation.PushAsync(page);
        }

        async void Selected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var selectedWeb = e.SelectedItem as Web;

            listView.SelectedItem = null;
                 var page = new DetailsWeb(selectedWeb);
                 page.WebUpdated += (source, web) =>
                 {
                     selectedWeb.Id = web.Id;
                     selectedWeb.Label = web.Label;
                     selectedWeb.Label = web.Label;
                     selectedWeb.Link = web.Link;
                 };

                 await Navigation.PushAsync(page);
        }

        async private void OnSort(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Sort by...", null, null, "Label", "Date");
            if (action == "Label")
            {
                listView.ItemsSource = _web.OrderBy(c => c.Label);
            }
            else if (action == "Date")
            {
                listView.ItemsSource = _web.OrderBy(c => c.SortDate);
            }
        }

        async void OnDelete(object sender, EventArgs e)
        {
            var web = (sender as MenuItem).CommandParameter as Web;
            if (await DisplayAlert("Warning", $"Are you sure you want to delete {web.Label}?", "Yes", "No"))
            {
                _web.Remove(web);
                await _connection.DeleteAsync(web);
            }
        }

        void OnOpen(object sender, EventArgs e)
        {
            var web = (sender as MenuItem).CommandParameter as Web;
            string link;
            if (web.Link.StartsWith("www"))
            {
                link = "http://" + web.Link;
            }
            else if (!web.Link.Contains("www"))
            {
                link = "http://www." + web.Link;
            }
            else link = web.Link;
            Device.OpenUri(new Uri(link));
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = GetNote(e.NewTextValue);
        }

        IEnumerable<Web> GetNote(string searchText = null)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return _web;
            }
            return _web.Where(c => c.Label.Contains(searchText));
        }

        public async Task ShareUri(string uri)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = uri,
                Title = "Share Link"
            });
        }

        async private void OnInfo(object sender, EventArgs e)
        {
            var page = new InfoPage();
            await Navigation.PushAsync(page);
        }
    }
}
