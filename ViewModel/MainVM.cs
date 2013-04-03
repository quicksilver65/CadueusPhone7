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
using System.Collections.ObjectModel;
using CadueusPhone7.Model;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using MvcApplication1.Models;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Text;

namespace CadueusPhone7.ViewModel
{
    public enum ProviderAction
    {
        MakeCall=1,
        SaveFavorite=2,
        DeleteFavorite=3,
        Directions=4
    }
    public class MainVM: INotifyPropertyChanged
    {
        #region Private Variables
        private string searchUrl = "http://www.azdevelop.net/client/mobileprovider/api/Provider/GetProvidersByDimensionCollection";
        private string cityUrl = "http://www.azdevelop.net/client/mobileprovider/api/Provider/GetDimensions?field=city";
        private string specialtyUrl = "http://www.azdevelop.net/client/mobileprovider/api/Provider/GetDimensions?field=specialty";
        private string latLongUrl = "http://www.azdevelop.net/client/mobileprovider/api/Provider/GetLatLong?Id=";
        private string isolatedStoreName = "toast";

        private ObservableCollection<string> cities;
        private ObservableCollection<MedicalProvider> providers;
        private ObservableCollection<string> specialties;
        private ObservableCollection<MedicalProvider> favorites;
        private SearchViewModel selectedCriteria;
        private MedicalProvider selectedProvider; 
        #endregion

        #region Public Properties
        public MedicalProvider SelectedProvider
        {
            get { return selectedProvider; }
            set { selectedProvider = value; Notify("SelectedProvider"); }
        }
        public SearchViewModel SelectedCriteria
        {
            get { return selectedCriteria; }
            set
            {
                if (value != null)
                {
                    selectedCriteria = value;
                    Notify("SelectedCriteria");
                }
            }
        }
        public string UpperTitle { get { return "PROJECT BASED CODE CAMP 2012"; } set { } }
        public ObservableCollection<MedicalProvider> Favorites
        {
            get { return favorites; }
            set
            {
                if (value != null)
                {
                    favorites = value;
                    Notify("Favorites");
                }
            }
        }
        public ObservableCollection<string> Specialties
        {
            get { return specialties; }
            set
            {
                if (value != null)
                {
                    specialties = value;
                    Notify("Specialties");
                }
            }
        }
        public ObservableCollection<MedicalProvider> Providers
        {
            get { return providers; }
            set
            {
                if (value != null)
                {
                    providers = value;
                    Notify("Providers");
                }
            }
        }
        public ObservableCollection<string> Cities
        {
            get { return cities; }
            set
            {
                if (value != null)
                {
                    cities = value;
                    Notify("Cities");
                }
            }
        } 
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;


        public MainVM()
        {
            Providers = new ObservableCollection<MedicalProvider>();
            Cities = new ObservableCollection<string>();
            Specialties = new ObservableCollection<string>();
            Favorites = new ObservableCollection<MedicalProvider>();
            SelectedCriteria = new SearchViewModel();
            LoadCities();
            LoadSpecialties();
            LoadFavorites();
        }


        #region Public Methods
        public void NavigatePages(NavigationService service, string name)
        {
            switch (name)
            {
                case "Home":
                    service.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                    break;
                case "Information":
                    service.Navigate(new Uri("/Views/Information.xaml", UriKind.Relative));
                    break;
                case "Search":
                    service.Navigate(new Uri("/Views/Search.xaml", UriKind.Relative));
                    break;
                case "Favorites":
                    service.Navigate(new Uri("/Views/Favorites.xaml", UriKind.Relative));
                    break;
                case "Results":
                    service.Navigate(new Uri("/Views/ProviderResults.xaml", UriKind.Relative));
                    break;
            }
        }
        public void MedicalProviderAction(ProviderAction action, MedicalProvider obj, NavigationService service)
        {
            SelectedProvider = obj;
            switch (action)
            {
                case ProviderAction.MakeCall:
                    PhoneCallTask pct = new PhoneCallTask() { PhoneNumber = obj.PhoneNumber.ToString(), DisplayName = obj.FirstName + " " + obj.LastName };
                    pct.Show();
                    break;
                case ProviderAction.SaveFavorite:
                    if (!Favorites.Any(p => p.Id == obj.Id))
                    {
                        Favorites.Add(obj);
                        SaveFavorites();
                    }
                    break;
                case ProviderAction.DeleteFavorite:
                    if (Favorites.Any(p => p.Id == obj.Id))
                    {
                        Favorites.Remove(obj);
                        SaveFavorites();
                    }
                    break;
                case ProviderAction.Directions:
                    var geoUrl = this.latLongUrl + obj.Id;
                    WebClient wc = new WebClient();
                    wc.OpenReadCompleted += (s, e) =>
                    {
                        var reader = new StreamReader(e.Result);
                        string jsonString = reader.ReadToEnd().ToString();
                        var latLong = JsonConvert.DeserializeObject<LatLong>(jsonString);

                        //Code for just a map
                        //BingMapsTask bingMapsTask = new BingMapsTask();
                        //bingMapsTask.Center = new GeoCoordinate(double.Parse(latLong.Latitude), double.Parse(latLong.Longtitude));
                        //bingMapsTask.ZoomLevel = 2;
                        //bingMapsTask.Show();

                        //Code for driving directions
                        BingMapsDirectionsTask bingMapsDirectionsTask = new BingMapsDirectionsTask();
                        GeoCoordinate endLocation = new GeoCoordinate(double.Parse(latLong.Latitude), double.Parse(latLong.Longtitude));

                        GeoCoordinate startLocation = new GeoCoordinate(33.369824, -111.811588);

                        LabeledMapLocation endLocationLML = new LabeledMapLocation("Dr. " + obj.FirstName + " " + obj.LastName, endLocation);
                        LabeledMapLocation startLocationLML = new LabeledMapLocation("Home", startLocation);

                        bingMapsDirectionsTask.End = endLocationLML;
                        bingMapsDirectionsTask.Start = startLocationLML;
                        bingMapsDirectionsTask.Show();

                    };
                    wc.OpenReadAsync(new Uri(geoUrl));
                    break;
            }
        }
        public void SearchProviders()
        {
            List<SearchCriteria> list = new List<SearchCriteria>();

            if (SelectedCriteria.City != null && SelectedCriteria.City != string.Empty)
                list.Add(new SearchCriteria() { Selector = "city", Parameter = SelectedCriteria.City });
            if (SelectedCriteria.Specialty != null && SelectedCriteria.Specialty != string.Empty)
                list.Add(new SearchCriteria() { Selector = "specialty", Parameter = SelectedCriteria.Specialty });
            if (SelectedCriteria.LastName != null && SelectedCriteria.LastName != string.Empty)
                list.Add(new SearchCriteria() { Selector = "lastName", Parameter = SelectedCriteria.LastName });

            if (list.Count == 0)
                list.Add(new SearchCriteria() { Selector = "Nothing", Parameter = "Nothing" });

            var jsonString = JsonConvert.SerializeObject(list);

            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";
            wc.UploadStringCompleted += (s, e) =>
            {
                try
                {
                    var str = e.Result;
                    var pList = JsonConvert.DeserializeObject<MedicalProvider[]>(e.Result);
                    Providers.Clear();
                    pList.ToList().ForEach((obj) => Providers.Add(obj));
                    SelectedCriteria.City = null;
                    SelectedCriteria.Specialty = null;
                    SelectedCriteria.LastName = null;
                }
                catch { }
            };
            wc.UploadStringAsync(new Uri(this.searchUrl), "POST", jsonString);
        } 
        #endregion

        #region Private Methods
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        private void LoadCities()
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (s, e) =>
            {
                var reader = new StreamReader(e.Result);
                string jsonString = reader.ReadToEnd().ToString();
                var list = JsonConvert.DeserializeObject<List<string>>(jsonString);
                list.ForEach((obj) => Cities.Add(obj));
            };
            wc.OpenReadAsync(new Uri(this.cityUrl));
        }
        private void LoadSpecialties()
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (s, e) =>
            {
                var reader = new StreamReader(e.Result);
                string jsonString = reader.ReadToEnd().ToString();
                var list = JsonConvert.DeserializeObject<List<string>>(jsonString);
                list.ForEach((obj) => Specialties.Add(obj));
            };
            wc.OpenReadAsync(new Uri(this.specialtyUrl));
        }
        private void SaveFavorites()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(isolatedStoreName))
                    store.DeleteFile(isolatedStoreName);

                using (var stream = store.OpenFile(isolatedStoreName, FileMode.OpenOrCreate))
                {
                    var content = JsonConvert.SerializeObject(Favorites);
                    stream.Write(UTF8Encoding.UTF8.GetBytes(content), 0, content.Length);
                    stream.Flush();
                    stream.Close();
                }
            }
        }
        private void LoadFavorites()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(isolatedStoreName))
                {
                    using (var stream = store.OpenFile(isolatedStoreName, FileMode.Open))
                    {
                        Favorites.Clear();
                        try
                        {
                            byte[] array = new byte[stream.Length];
                            stream.Read(array, 0, array.Length);
                            stream.Flush();
                            var jsonString = UTF8Encoding.UTF8.GetString(array, 0, array.Length);
                            var list = JsonConvert.DeserializeObject<List<MedicalProvider>>(jsonString);
                            list.ForEach((obj) => Favorites.Add(obj));
                        }
                        catch
                        {
                            //SaveFavorites();
                        }
                    }
                }
            }
        } 
        #endregion
    }
}
