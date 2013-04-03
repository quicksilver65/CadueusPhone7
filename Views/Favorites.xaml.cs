using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CadueusPhone7.Model;
using CadueusPhone7.ViewModel;

namespace CadueusPhone7.Views
{
    public partial class Favorites : PhoneApplicationPage
    {
        public Favorites()
        {
            InitializeComponent();
            this.Loaded += (e, s) =>
            {
                this.DataContext = App.ViewModel;
            };
        }
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.NavigatePages(NavigationService, ((ApplicationBarIconButton)sender).Text);
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = ((MenuItem)sender);
            var obj = (MedicalProvider)item.DataContext;
            var parameter = (string)(item).CommandParameter;
            var action = (ProviderAction)int.Parse(parameter);
            App.ViewModel.MedicalProviderAction(action, obj, NavigationService);
        }
    }
}