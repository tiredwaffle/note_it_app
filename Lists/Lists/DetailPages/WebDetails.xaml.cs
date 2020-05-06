using ISQLiteBase;
using Lists.Models;
using SQLite;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lists
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailsWeb : ContentPage
    {
        public event EventHandler<Web> WebAdded;

        public event EventHandler<Web> WebUpdated;

        private SQLiteAsyncConnection _connection;

        public DetailsWeb (Web web)
		{
            if (web == null)
                throw new ArgumentNullException(nameof(web));

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            BindingContext = new Web
            {
                Id = web.Id,
                Label = web.Label,
                Link = web.Link,
                Date=web.Date,
                SortDate=web.SortDate
            };
        }

        async void OnShare(object sender, EventArgs e)
        {
            var web = BindingContext as Web;
            await ShareUri(web.Link);
        }

        public async Task ShareUri(string uri)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = uri,
                Title = "Share Link"
            });
        }

        private async void OnOpen(object sender, EventArgs e)
        {
            var web = BindingContext as Web;
            if (String.IsNullOrWhiteSpace(web.Link))
            {
                await DisplayAlert("Error", "Please enter the link", "OK");
                return;
            }
            else
            {
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
        }

        async private void OnSave(object sender, EventArgs e)
        {
            var web = BindingContext as Web;

            if (String.IsNullOrWhiteSpace(web.Link))
            {
                await DisplayAlert("Error", "Please enter the link.", "OK");
                return;
            }

            if (web.Id == 0)
            {
                await _connection.InsertAsync(web);

                WebAdded?.Invoke(this, web);
            }
            else
            {
                await _connection.UpdateAsync(web);

                WebUpdated?.Invoke(this, web);
            }
            await Navigation.PopAsync();
        }
    }
}