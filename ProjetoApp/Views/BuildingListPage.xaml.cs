using ProjetoApp.Data;
using ProjetoApp.ViewModels;
using PropertyChanged;
using System;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ProjetoApp.Views
{
    /// <summary>
    /// Displays the list of buildings.
    /// </summary>
    [ImplementPropertyChanged]
    public sealed partial class BuildingListPage : Page
    {
        /// <summary>
        /// We use this object to bind the UI to our data. 
        /// </summary>
        public BuildingListPageViewModel ViewModel { get;  } = new BuildingListPageViewModel();

        /// <summary>
        /// Creates a new BuildingListPage'
        /// </summary>
        public BuildingListPage()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Retrieve the list of Buildings when the user navigates to the page. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ViewModel.Buildings.Count < 1)
            {
                ViewModel.LoadBuildings();
            }
        }

        /// <summary>
        /// Opens the building in the building details page for editing. 
        /// </summary>
        /// <param name="sender">The button that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void EditButton_Click(object sender, RoutedEventArgs e) => 
            Frame.Navigate(typeof(BuildingDetailPage), ViewModel.SelectedBuilding);

        /// <summary>
        /// Deletes the currently selected building.
        /// </summary>
        /// <param name="sender">The button that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private async void DeleteBuilding_Click(object sender, RoutedEventArgs e)
        {

            try
            { 
                var deleteBuilding = ViewModel.SelectedBuilding;
                await ViewModel.DeleteBuilding(deleteBuilding);
            }
            catch(BuildingDeletionException ex)
            {
                var dialog = new ContentDialog()
                {
                    Title = "Unable to delete Building",
                    Content = $"There was an error when we tried to delete " + 
                        $"building  #{ViewModel.SelectedBuilding.Id.ToString()}:\n{ex.Message}",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }

        }

        /// <summary>
        /// Workaround to support earlier versions of Windows. 
        /// </summary>
        /// <param name="sender">The command bar that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent(
                "Windows.UI.Xaml.Controls.CommandBar", "DefaultLabelPosition"))
            {
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    (sender as CommandBar).DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                }
                else
                {
                    (sender as CommandBar).DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                }
            }
        }

        private void BuildingSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            UserControls.CollapsibleSearchBox searchBox = sender as UserControls.CollapsibleSearchBox;

            if (searchBox != null)
            {
                searchBox.AutoSuggestBox.QuerySubmitted += BuildingSearch_QuerySubmitted;
                searchBox.AutoSuggestBox.TextChanged += BuildingSearch_TextChanged;
                searchBox.AutoSuggestBox.PlaceholderText = "Pesquisar imóveis...";
                searchBox.AutoSuggestBox.ItemTemplate = (DataTemplate)this.Resources["SearchSuggestionItemTemplate"];
                searchBox.AutoSuggestBox.ItemContainerStyle = (Style)this.Resources["SearchSuggestionItemStyle"];
            }
        }
        /// <summary>
        /// Creates an email about the currently selected invoice. 
        /// </summary>
        /// <param name="sender">The hyperlink button that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private async void EmailButton_Click(object sender, RoutedEventArgs e)
        {

            var emailMessage = new EmailMessage();
            emailMessage.Body = $"Dear {ViewModel.SelectedBuilding.CustomerName},";
            emailMessage.Subject = $"A message from Server about building " + 
                $"#{ViewModel.SelectedBuilding.AddressStreet} placed on " + 
                $"{ViewModel.SelectedBuilding.DatePlaced.ToString("MM/dd/yyyy")}";

            if (!string.IsNullOrEmpty(ViewModel.SelectedCustomer.Email))
            {
                var emailRecipient = new EmailRecipient(ViewModel.SelectedCustomer.Email);
                emailMessage.To.Add(emailRecipient);
            }
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

        /// <summary>
        ///  Loads the specified building in the building details page. 
        /// </summary>
        /// <param name="building"></param>
        private void GoToDetailsPage(ProjetoApp.Data.Building building) => 
            Frame.Navigate(typeof(BuildingDetailPage), building);

        /// <summary>
        /// Searchs the list of orders.
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">Info about the event. </param>
        private void BuildingSearch_QuerySubmitted(AutoSuggestBox sender, 
            AutoSuggestBoxQuerySubmittedEventArgs args) => 
                ViewModel.QueryBuildings(args.QueryText);

        /// <summary>
        /// Updates the suggestions for the AutoSuggestBox as the user types. 
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">Info about the event.</param>
        private void BuildingSearch_TextChanged(AutoSuggestBox sender, 
            AutoSuggestBoxTextChangedEventArgs args)
        {
            // We only want to get results when it was a user typing, 
            // otherwise we assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.QueryBuildings(sender.Text);
            }
        }

        /// <summary>
        /// Navigates to the building detail page when the user
        /// double-clicks an building. 
        /// </summary>
        /// <param name="sender">The element that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => 
            Frame.Navigate(typeof(BuildingDetailPage), ((FrameworkElement)sender).DataContext as Negotiation);

        private void ListView_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                GoToDetailsPage(ViewModel.SelectedBuilding);
            }
        }

        /// <summary>
        /// Navigates to the building details page.
        /// </summary>
        /// <param name="sender">The element that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void MenuFlyoutViewDetails_Click(object sender, RoutedEventArgs e) => 
            Frame.Navigate(typeof(BuildingDetailPage), ViewModel.SelectedBuilding, 
                new DrillInNavigationTransitionInfo());

        /// <summary>
        /// Refreshes the building list.
        /// </summary>
        /// <param name="sender">The button that fired this event.</param>
        /// <param name="e">Info about the event.</param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e) => 
            ViewModel.LoadBuildings();
    }
}
