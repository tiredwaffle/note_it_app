using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using SQLite;


namespace Lists.Models
{
    public class Notes : INotifyPropertyChanged
    { 
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _label;

        [MaxLength(14)]
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label == value)
                    return;
                _label = value;
                OnPropertyChanged();
            }
        }

        private string _textNote;

        public string TextNote
        {
            get { return _textNote; }
            set
            {
                if (_textNote == value)
                    return;
                _textNote = value;
                OnPropertyChanged();
            }
        }
        
        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                if (_link == value)
                    return;
                _link = value;
                OnPropertyChanged();
            }
        }

        public string _date;

        public string Date
        {
            get { return _date; }
            set
            {
                if (_date == value)
                    return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public string _sdate;

        public string SortDate
        {
            get { return _sdate; }
            set
            {
                if (_sdate == value)
                    return;
                _sdate = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
