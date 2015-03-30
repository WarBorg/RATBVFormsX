using RATBVFormsX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RATBVFormsX.Views
{
    public partial class BusStationsView : TabbedPage
    {
        private BusStationsViewModel ViewModel;

        public BusStationsView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            ViewModel = this.BindingContext as BusStationsViewModel;
        }

        private void BusStation_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            ViewModel.ShowSelectedBusTimeTableCommand.Execute(e.SelectedItem);

            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}
