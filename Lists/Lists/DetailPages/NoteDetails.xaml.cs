using ISQLiteBase;
using Lists.Models;
using SQLite;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Lists
{
	
	public partial class DetailsNote : ContentPage
	{
        public event EventHandler<Notes> NoteAdded;

        public event EventHandler<Notes> NoteUpdated;

        private SQLiteAsyncConnection _connection;
 
        public DetailsNote(Notes note)
		{
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            BindingContext = new Notes
            {
                Id = note.Id,
                Label = note.Label,
                TextNote = note.TextNote,
                Date=note.Date
            };
        }
        
        public async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Share Note"
            });
        }
    
        async void OnShare(object sender, EventArgs e)
        {
            var note = BindingContext as Notes;
            await ShareText(note.TextNote);
        }

        async private void TextChanged(object sender, TextChangedEventArgs e)
        {
            var note = BindingContext as Notes;

            if (note.Id == 0)
            {
                await _connection.InsertAsync(note);

                NoteAdded?.Invoke(this, note);
            }
            else
            {
                await _connection.UpdateAsync(note);

                NoteUpdated?.Invoke(this, note);
            }

        }
    }
}