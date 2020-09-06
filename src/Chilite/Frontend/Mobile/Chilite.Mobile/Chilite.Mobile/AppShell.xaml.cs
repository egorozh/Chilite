using System;
using System.Collections.Generic;
using Chilite.Mobile.ViewModels;
using Chilite.Mobile.Views;
using Xamarin.Forms;

namespace Chilite.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
