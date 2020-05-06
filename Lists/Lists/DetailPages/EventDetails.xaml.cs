using ISQLiteBase;
using Lists.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lists
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EventDetails : ContentPage
	{
        public event EventHandler<Event> EventAdded;

        public event EventHandler<Event> EventUpdated;

        private SQLiteAsyncConnection _connection;

        public EventDetails (Event eve)
		{
            if (eve == null)
                throw new ArgumentNullException(nameof(eve));

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            BindingContext = new Event
            {
                Id = eve.Id,
                Label = eve.Label,
                StartDate = eve.StartDate,
                EndDate = eve.EndDate,
                Details = eve.Details,
                Date = eve.Date,
                SortDate = eve.SortDate
            };
        }
  
        private async void OnSave(object sender, EventArgs e)
        {
            var eve = BindingContext as Event;

            if (String.IsNullOrWhiteSpace(eve.Label))
            {
                await DisplayAlert("Error", "Please enter the label.", "OK");
                return;
            }
             
            if (eve.StartDate > eve.EndDate)
            {
                await DisplayAlert("Error", "Please enter correct dates.", "OK");
                return;
            }

            if (eve.Id == 0)
            {
                await _connection.InsertAsync(eve);
                EventAdded?.Invoke(this, eve);
            }
            else
            { 
            EventUpdated?.Invoke(this, eve);
            }

            await Navigation.PopAsync();
        }

        async void OnShare(object sender, EventArgs e)
        {
            var note = BindingContext as Event;
            await ShareText(note.Label + ": " + note.StartDate.Date + " - " + note.EndDate.Date);
        }

        public async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Share Event"
            });
        }
    }
}