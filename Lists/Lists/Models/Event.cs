using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lists.Models
{
    public class Event : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _label;

        [MaxLength(255)]
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
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        private string _details;

        [MaxLength(14)]
        public string Details
        {
            get { return _details; }
            set
            {
                if (_details == value)
                    return;
                _details = value;
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
