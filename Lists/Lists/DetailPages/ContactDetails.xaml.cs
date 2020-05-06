using System;
using System.Threading.Tasks;
using ISQLiteBase;
using Lists.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Lists
{
    public partial class DetailsContact : ContentPage
    {
        public event EventHandler<Contacts> ContactAdded;

        public event EventHandler<Contacts> ContactUpdated;

        private SQLiteAsyncConnection _connection;

        public DetailsContact(Contacts contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

             BindingContext = new Contacts
              {
                  FirstName = contact.FirstName,
                  LastName = contact.LastName,
                  Phone = contact.Phone,
                  Email = contact.Email,
                  Id=contact.Id,
                  Date=contact.Date,
                  SortDate=contact.SortDate
              };
        }

        async void OnShare(object sender, EventArgs e)
        {
            var note = BindingContext as Contacts;
            await ShareText(note.FirstName+" "+note.LastName+": "+note.Phone);
        }

        public async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Share Number"
            });
        }

        async void OnSave(object sender, System.EventArgs e)
        {
            var contact = BindingContext as Contacts;

            if (String.IsNullOrWhiteSpace(contact.FirstName))
            {
                await DisplayAlert("Error", "Please enter the name.", "OK");
                return;
            }

            if (contact.Id == 0)
            {
                await _connection.InsertAsync(contact);

                ContactAdded?.Invoke(this, contact);
            }
            else
            {
                await _connection.UpdateAsync(contact);

                ContactUpdated?.Invoke(this, contact);
            }

            await Navigation.PopAsync();
        }

        private void OnCall(object sender, EventArgs e)
        {
            var cont = BindingContext as Contacts;
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
            PlacePhoneCall(number);
        }

        public void PlacePhoneCall(string number)
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
