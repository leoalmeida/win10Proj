using ProjetoApp.ViewModels;
using PropertyChanged;
using System;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ProjetoApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ImplementPropertyChanged]
    public sealed partial class CustomerListPage : Page
    {
        private enum SortOption { ByName, ByRG };
        private enum GroupOption { ByName, ByRG };
        public CustomerListPageViewModel ViewModel { get; set; } = new CustomerListPageViewModel();

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public CustomerListPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.CommandBar", "DefaultLabelPosition"))
            {
                Window.Current.SizeChanged += CurrentWindow_SizeChanged;
            }
        }

        private void CurrentWindow_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile" && e.Size.Width >= (double)App.Current.Resources["MediumWindowSnapPoint"])
            {
                mainCommandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
            }
            else
            {
                mainCommandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
            }
        }

        /// <summary>
        /// Retrieves a list of customers when the user navigates to the page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back
                && ViewModel.SelectedCustomer != null
                && ViewModel.SelectedCustomer.HasChanges == true)
            {
                ViewModel.UpdateCustomerInList(ViewModel.SelectedCustomer);
                ViewModel.SelectedCustomer.HasChanges = false;
            }
        }

        /// <summary>
        /// Navigates to a blank customer details page for the user to fill in.
        /// </summary>
        private void CreateCustomer_Click(object sender, RoutedEventArgs e) =>
            GoToDetailsPage(null);
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
        /// Provide suggestions in the search box as the user types.
        /// </summary>
        private void CustomerSearchBox_TextChanged(AutoSuggestBox sender,
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
                    var cvs = Resources["CustomersCVS"] as CollectionViewSource;
                    if (cvs != null)
                    {
                        if (string.IsNullOrWhiteSpace(sender.Text))
                        {
                            cvs.Source = ViewModel.Customers;
                        }
                    }
                    sender.ItemsSource = null;
                }
                else
                {
                    string[] parameters = sender.Text.Split(new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

                    sender.ItemsSource = ViewModel.Customers
                        .Where(x => parameters
                            .Any(y =>
                                x.Address.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.FirstName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.LastName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.RG.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.CPF.StartsWith(y, StringComparison.OrdinalIgnoreCase)))
                        .OrderByDescending(x => parameters
                            .Count(y =>
                                x.Address.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.FirstName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.LastName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.RG.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.CPF.StartsWith(y, StringComparison.OrdinalIgnoreCase)));
                }
            }
        }

        /// <summary>
        /// Search by customer first or last name, address, or RG then display results.
        /// </summary>
        private void CustomerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var cvs = Resources["CustomersCVS"] as CollectionViewSource;
            if (cvs != null)
            {
                if (string.IsNullOrWhiteSpace(args.QueryText))
                {
                    if (string.IsNullOrEmpty(args.QueryText))
                    {
                        cvs.Source = ViewModel.Customers;
                    }
                }
                else
                {
                    cvs.IsSourceGrouped = false;
                    string[] parameters = sender.Text.Split(new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

                    cvs.Source = ViewModel.Customers
                        .Where(x => parameters
                            .Any(y =>
                                x.Address.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.FirstName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.LastName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.RG.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.CPF.StartsWith(y, StringComparison.OrdinalIgnoreCase)))
                        .OrderByDescending(x => parameters
                            .Count(y =>
                                x.Address.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.FirstName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.LastName.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.RG.StartsWith(y, StringComparison.OrdinalIgnoreCase) ||
                                x.CPF.StartsWith(y, StringComparison.OrdinalIgnoreCase)));
                }
            }
        }


        #region Context Menu Handlers
        private void MenuFlyoutSortByName_Click(object sender, RoutedEventArgs e) =>
            SortList(SortOption.ByName);

        private void MenuFlyoutSortByRG_Click(object sender, RoutedEventArgs e) =>
            SortList(SortOption.ByRG);

        private void MenuFlyoutGroupByName_Click(object sender, RoutedEventArgs e) =>
            GroupList(GroupOption.ByName);

        private void MenuFlyoutGroupByRG_Click(object sender, RoutedEventArgs e) =>
            GroupList(GroupOption.ByRG);

        #endregion

        /// <summary>
        /// Sorts a list of customers by last name or RG.
        /// </summary>
        private void SortList(SortOption option)
        {
            CollectionViewSource cvs = Resources["CustomersCVS"] as CollectionViewSource;
            if (cvs != null)
            {
                cvs.IsSourceGrouped = false;
                switch (option)
                {
                    case SortOption.ByName:
                        cvs.Source = ViewModel.Customers.OrderByDescending(customer => customer.LastName);
                        break;
                    case SortOption.ByRG:
                        cvs.Source = ViewModel.Customers.OrderByDescending(customer => customer.RG);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Groups a list of customers by last name or RG.
        /// </summary>
        private void GroupList(GroupOption option)
        {
            CollectionViewSource cvs = Resources["CustomersCVS"] as CollectionViewSource;
            if (cvs != null)
            {
                cvs.IsSourceGrouped = true;
                switch (option)
                {
                    case GroupOption.ByName:
                        cvs.Source = ViewModel.Customers.GroupBy(customer => customer.LastName);
                        break;
                    case GroupOption.ByRG:
                        cvs.Source = ViewModel.Customers.GroupBy(customer => customer.RG);
                        break;
                    default:
                        cvs.IsSourceGrouped = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Workaround to support earlier versions of Windows.
        /// </summary>
        private void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.CommandBar", "DefaultLabelPosition"))
            {
                (sender as CommandBar).DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                
            }
        }

        /// <summary>
        /// Keyboard control for selecting a customer and displaying details
        /// </summary>
        private void ListView_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                GoToDetailsPage(ViewModel.SelectedCustomer);
            }
        }

        /// <summary>
        /// Double click control for selecting a customer and displaying details.
        /// </summary>
        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var customer = (sender as FrameworkElement).DataContext as CustomerViewModel;
            GoToDetailsPage(customer);
        }

        /// <summary>
        /// Menu flyout click control for selecting a customer and displaying details.
        /// </summary>
        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem)
            {
                var customer = (sender as FrameworkElement).DataContext as CustomerViewModel;
                GoToDetailsPage(customer);
            }
            else
            {
                GoToDetailsPage(ViewModel.SelectedCustomer);
            }
        }

        /// <summary>
        /// Opens the order detail page for the user to create an order for the selected customer.
        /// </summary>
        private void AddBuilding_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem)
            {
                GoToOrderPage((sender as FrameworkElement).DataContext as CustomerViewModel);
            }
            else
            {
                GoToOrderPage(ViewModel.SelectedCustomer);
            }
        }

        /// <summary>
        /// Navigates to the customer detail page for the provided customer.
        /// </summary>
        private void GoToDetailsPage(CustomerViewModel customer) =>
            Frame.Navigate(typeof(CustomerDetailPage), customer,
                new DrillInNavigationTransitionInfo());

        /// <summary>
        /// Navigates to the order detail page for the provided customer.
        /// </summary>
        private void GoToOrderPage(CustomerViewModel customer) =>
            Frame.Navigate(typeof(BuildingDetailPage), customer.Model);
    }
}
