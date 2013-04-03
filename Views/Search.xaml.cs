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

namespace CadueusPhone7.Views
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.SearchProviders();
            App.ViewModel.NavigatePages(NavigationService, "Results");
        }

    }
}