﻿using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for BrowsingControl.xaml
    /// </summary>
    public partial class BrowsingControl : UserControl
    {
        public BrowsingControl(ViewModelBase dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }
    }
}
