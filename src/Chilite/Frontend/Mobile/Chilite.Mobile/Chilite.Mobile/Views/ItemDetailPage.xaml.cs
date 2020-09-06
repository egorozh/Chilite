using System.ComponentModel;
using Xamarin.Forms;
using Chilite.Mobile.ViewModels;

namespace Chilite.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}