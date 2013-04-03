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
using CadueusPhone7.ViewModel;
using Microsoft.Phone.Shell;

namespace CadueusPhone7
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
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
    }
}