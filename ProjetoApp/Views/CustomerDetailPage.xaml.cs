using ProjetoApp.Data;
using ProjetoApp.ViewModels;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ProjetoApp.Views
{
    /// <summary>Characteristic
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomerDetailPage : Page
    {
        /// <summary>
        /// Used to bind the UI to the data.
        /// </summary>
        public CustomerDetailPageViewModel ViewModel { get; set; } = new CustomerDetailPageViewModel();

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public CustomerDetailPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        /// <summary>
        /// Displays the selected customer data.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CustomerViewModel customer = e.Parameter as CustomerViewModel;
            if (customer == null)
            {
                ViewModel.Customer = new CustomerViewModel();
                Bindings.Update();
                PageHeaderText.Text = "Novo Cliente";
            }
            else if (ViewModel.Customer != customer)
            {
                ViewModel.Customer = customer;
                Bindings.Update();
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Navigates from the current page.
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.IsInEdit = false;

            base.OnNavigatingFrom(e);
        }

        private void CustomerSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            UserControls.CollapsibleSearchBox searchBox = sender as UserControls.CollapsibleSearchBox;

            if (searchBox != null)
            {
                searchBox.AutoSuggestBox.QuerySubmitted += CustomerSearchBox_QuerySubmitted;
                searchBox.AutoSuggestBox.TextChanged += CustomerSearchBox_TextChanged;
                searchBox.AutoSuggestBox.PlaceholderText = "Pesquisar clientes...";
            }
        }
        /// <summary>
        /// Queries the database for a customer result matching the search text entered.
        /// </summary>
        private async void CustomerSearchBox_TextChanged(AutoSuggestBox sender,
            AutoSuggestBoxTextChangedEventArgs args)
        {
            // We only want to get results when it was a user typing,
            // otherwise we assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // If no search query is entered, refresh the complete list.
                if (string.IsNullOrEmpty(sender.Text))
                {
                    sender.ItemsSource = null;
                }
                else
                {
                    sender.ItemsSource = await DataProvider.Instance.GetCustomersAsync(sender.Text);
                }
            }
        }

        /// <summary>
        /// Search by customer name, email, or phone number, then display results.
        /// </summary>
        private void CustomerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Customer customer = args.ChosenSuggestion as Customer;
            if (customer != null)
            {
                Frame.Navigate(typeof(CustomerDetailPage), new CustomerViewModel(customer));
            }
        }

        /// <summary>
        /// A workaround for earlier versions of Windows 10.
        /// </summary>
        private void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.CommandBar", "DefaultLabelPosition"))
            {
                (sender as CommandBar).DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                
                // Disable dynamic overflow on this page. There are only a few commands, and it causes
                // layout problems when save and cancel commands are shown and hidden.
                (sender as CommandBar).IsDynamicOverflowEnabled = false;
            }
        }

        /// <summary>
        /// Navigates to the buildings page from the customer.
        /// </summary>
        private void ViewBuildingButton_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(BuildingDetailPage), ((FrameworkElement)sender).DataContext,
                new DrillInNavigationTransitionInfo());

        /// <summary>
        /// Adds a new building to the customer.
        /// </summary>
        private void AddBuilding_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(BuildingDetailPage), ViewModel.Customer.Model);

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Customer.IsNewCustomer == true
                && Frame.CanGoBack == true)
            {
                Frame.GoBack();
            }
        }
    }
}
