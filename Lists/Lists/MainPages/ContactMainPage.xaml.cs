using Lists.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ISQLiteBase;
using System.Data;
using Xamarin.Essentials;

namespace Lists
{
    public partial class MainPageContact : ContentPage
    {
        private SQLiteAsyncConnection _connection;

        private ObservableCollection<Contacts> _cont;

        private bool _isDataLoaded;

        public MainPageContact()
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
            await _connection.CreateTableAsync<Contacts>();

            var cont = await _connection.Table<Contacts>().ToListAsync();

            _cont = new ObservableCollection<Contacts>(cont);
            listView.ItemsSource = _cont;
        }

        async private void OnAdd(object sender, EventArgs e)
        {
            string date = DateTime.Today.ToString("dd.MM.yyyy");
            string sortdate = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

            var page = new DetailsContact(new Contacts { FirstName = "Contact", LastName="New", Date = date, SortDate = sortdate });
            page.ContactAdded += (source, cont) =>
            {
                _cont.Add(cont);
            };

            await Navigation.PushAsync(page);
        }

        async void Selected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;

            var selectedCont = e.SelectedItem as Contacts;

            listView.SelectedItem = null;
            
                var page = new DetailsContact(selectedCont);
                page.ContactUpdated += (source, cont) =>
                {
                    selectedCont.Id = cont.Id;
                    selectedCont.FirstName = cont.FirstName;
                    selectedCont.LastName = cont.LastName;
                    selectedCont.Phone = cont.Phone;
                    selectedCont.Email = cont.Email;
                };

                await Navigation.PushAsync(page);
            
        }

        async void OnDelete(object sender, EventArgs e)
        {
            var cont = (sender as MenuItem).CommandParameter as Contacts;
            if (await DisplayAlert("Warning", $"Are you sure you want to delete {cont.FullName}?", "Yes", "No"))
            {
                _cont.Remove(cont);
                await _connection.DeleteAsync(cont);
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = GetNote(e.NewTextValue);
        }

        IEnumerable<Contacts> GetNote(string searchText = null)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return _cont;
            }
            return _cont.Where(c => c.FullName.Contains(searchText));
        }

        async private void OnInfo(object sender, EventArgs e)
        {
            var page = new InfoPage();
            await Navigation.PushAsync(page);
        }

        async private void OnSort(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Sort by...", null, null, "Name", "Date");
            if (action == "Name")
            {
                listView.ItemsSource = _cont.OrderBy(c => c.FullName);
            }
            else if (action == "Date")
            {
                listView.ItemsSource = _cont.OrderBy(c => c.SortDate);
            }
        }

        private async void OnCall(object sender, EventArgs e)
        {
            var cont = (sender as MenuItem).CommandParameter as Contacts;
            string number;
            if (cont.Phone.StartsWith("0"))
            {
                number = "+38" + cont.Phone;
            }
            else if (cont.Phone.StartsWith("8"))
            {
                number = "+3" + cont.Phone;
            }
            else number = cont.Phone;
            await PlacePhoneCall(number);
        }

        public async Task PlacePhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                throw (anEx);
            }
            catch (FeatureNotSupportedException ex)
            {
                throw (ex);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
