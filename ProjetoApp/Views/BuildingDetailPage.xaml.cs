using ProjetoApp.Data;
using ProjetoApp.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ProjetoApp.Views
{
    /// <summary>
    /// Displays and edits an building.
    /// </summary>
    public sealed partial class BuildingDetailPage : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes the page.
        /// </summary>
        public BuildingDetailPage()
        {
            InitializeComponent();
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
        }

        /// <summary>
        /// Check for unsaved changes if the app shuts down.
        /// </summary>
        private void App_Suspending(object sender, SuspendingEventArgs e)
        {

            if (ViewModel.HasChanges)
            {
                // Save a temporary copy of the modified Building so that the user has a chance to save it
                // the next time the app is launched. 
            }

        }

        /// <summary>
        /// Stores the view model. 
        /// </summary>
        private BuildingDetailPageViewModel _viewModel;

        /// <summary>
        /// We use this object to bind the UI to our data.
        /// </summary>
        public BuildingDetailPageViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Loads the specified Building, a cached Building, or creates a new Building.
        /// </summary>
        /// <param name="e">Info about the event.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Determine whether a valid Building was provided.
            var building = e.Parameter as Building;
            if (building != null)
            {
                ViewModel = new BuildingDetailPageViewModel(building);
            }
            else
            {
                // If building is null, check to see whether a customer was provided.
                var customer = e.Parameter as Customer;
                if (customer != null)
                {
                    // Create a new building for the specified customer. 
                    ViewModel = new ViewModels.BuildingDetailPageViewModel(new Building(customer));
                }
                // If no building or customer was provided,
                // check to see if we have a cached building.
                // If we don't, create a blank new building. 
                else if (ViewModel == null)
                {
                    ViewModel = new BuildingDetailPageViewModel(new Building()); 
                }
            }
            base.OnNavigatedTo(e);
        }


        /// <summary>
        /// Check whether there are unsaved changes and warn the user.
        /// </summary>
        /// <param name="e">Info about the navigation operation.</param>
        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {

            if (ViewModel.HasChanges)
            {
                var saveDialog = new SaveChangesDialog()
                {
                    Title = $"Save changes to Building # {ViewModel.Id.ToString()}?",
                    Message = $"to # {ViewModel.Id.ToString()} " + 
                        "has unsaved changes that will be lost. Do you want to save your changes?"
                };
                await saveDialog.ShowAsync();
                SaveChangesDialogResult result = saveDialog.Result;

                switch (result)
                {
                    case SaveChangesDialogResult.Save:
                        await ViewModel.SaveBuilding();
                        break;
                    case SaveChangesDialogResult.DontSave:
                        break;
                    case SaveChangesDialogResult.Cancel:
                        if (e.NavigationMode == NavigationMode.Back)
                        {
                            Frame.GoForward();
                        }
                        else
                        {
                            Frame.GoBack();
                        }
                        e.Cancel = true;
                        // This flag gets cleared on navigation, so restore it. 
                        ViewModel.HasChanges = true; 
                        break;
                }
            }
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Creates an email to the current customer.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">Info about the event.</param>
        private async void emailButton_Click(object sender, RoutedEventArgs e)
        {
            var emailMessage = new EmailMessage();
            emailMessage.Body = $"Dear {ViewModel.CustomerName},";
            emailMessage.Subject = $"A message from system about building " + 
                $"#{ViewModel.Id.ToString()} placed on {ViewModel.DatePlaced.ToString("MM/dd/yyyy")} ";

            if (!string.IsNullOrEmpty(ViewModel.Customer.Email))
            {
                var emailRecipient = new EmailRecipient(ViewModel.Customer.Email);
                emailMessage.To.Add(emailRecipient);
            }

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

        /// <summary>
        /// A workaround for earlier versions of Windows 10.
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

        /// <summary>
        /// Reloads the building.
        /// </summary>
        /// <param name="sender">The button the fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => 
            ViewModel = await BuildingDetailPageViewModel.CreateFromGuid(ViewModel.Id);

        /// <summary>
        /// Reverts the page
        /// </summary>
        /// <param name="sender">The button the fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private async void RevertButton_Click(object sender, RoutedEventArgs e)
        {

            var saveDialog = new SaveChangesDialog()
            {
                Title = $"Save changes to Building # {ViewModel.Id.ToString()}?",
                Message = $"Building # {ViewModel.Id.ToString()} " + 
                    "has unsaved changes that will be lost. Do you want to save your changes?"
            };
            await saveDialog.ShowAsync();
            SaveChangesDialogResult result = saveDialog.Result;

            switch (result)
            {
                case SaveChangesDialogResult.Save:
                    await ViewModel.SaveBuilding();
                    ViewModel = await BuildingDetailPageViewModel.CreateFromGuid(ViewModel.Id);
                    break;
                case SaveChangesDialogResult.DontSave:
                    ViewModel = await BuildingDetailPageViewModel.CreateFromGuid(ViewModel.Id);
                    break;
                case SaveChangesDialogResult.Cancel:
                    break;
            }         
        }


        /// <summary>
        /// Saves the current building.
        /// </summary>
        /// <param name="sender">The button the fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await ViewModel.SaveBuilding();
            }
            catch (BuildingSavingException ex)
            {
                var dialog = new ContentDialog()
                {
                    Title = "Unable to save",
                    Content = $"There was an error saving your building:\n{ex.Message}", 
                    PrimaryButtonText = "OK"                 
                };

                await dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Queries for products.
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">Info about the event.</param>
        private void CharacteristicSearchBox_TextChanged(AutoSuggestBox sender, 
            AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.UpdateCharacteristicSuggestions(sender.Text);
            }
        }

        /// <summary>
        /// Notifies the page that a new item was chosen.
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">Info about the event.</param>
        private void CharacteristicSearchBox_SuggestionChosen(AutoSuggestBox sender, 
            AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var selectedCharacteristic = args.SelectedItem as Characteristic;
                ViewModel.NewCharacteristic = selectedCharacteristic;
            }
        }

        /// <summary>
        /// Adds the new line item to the list of line items.
        /// </summary>
        /// <param name="sender">The button that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void AddCharacteristicButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Characteristics.Add(ViewModel.NewCharacteristic);
            ClearCandidateCharacteristic();
        }

        /// <summary>
        /// Clears the new line item without adding it to the list of line items.
        /// </summary>
        /// <param name="sender">The button that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void CancelCharacteristicButton_Click(object sender, RoutedEventArgs e)
        {
            ClearCandidateCharacteristic();
        }

        /// <summary>
        /// Cleears the new line item entry area.
        /// </summary>
        private void ClearCandidateCharacteristic()
        {
            CharacteristicSearchBox.Text = String.Empty;
            ViewModel.NewCharacteristic = new Characteristic();
        }

        /// <summary>
        /// Removes a line item from the building.
        /// </summary>
        /// <param name="sender">The button that fired the event.</param>
        /// <param name="e">Info about the event.</param>
        private void RemoveCharacteristic_Click(object sender, RoutedEventArgs e)
        {
            var characteristic = ((Button)sender).DataContext as Characteristic;
            ViewModel.Characteristics.Remove(characteristic);
        }

        /// <summary>
        /// Fired when a property value changes. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value changed. 
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. </param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
