using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace CadueusPhone7.Model
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private string specialty;
        private string city;
        private string lastName;
        public string City
        {
            get { return city; }
            set
            {

                city = value;
                Notify("City");

            }
        }
        public string Specialty
        {
            get { return specialty; }
            set
            {

                specialty = value;
                Notify("Specialty");

            }
        }
        public string LastName
        {
            get { return lastName; }
            set
            {

                lastName = value;
                Notify("LastName");

            }
        }

        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
