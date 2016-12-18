using ProjetoApp.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoApp.ViewModels
{
    /// <summary>
    /// Encapsulates data for the BuildingListPage. The page UI
    /// binds to the properties defined here. 
    /// </summary>
    public class BuildingListPageViewModel : BindableBase
    {
        public BuildingListPageViewModel()
        {
            IsLoading = false; 
        }

        /// <summary>
        /// Gets the Buildings to display.
        /// </summary>
        public ObservableCollection<Building> Buildings { get; private set; } = new ObservableCollection<Building>();

        /// <summary>
        /// Keeps an unfiltered view of the Buildings list. 
        /// </summary>
        private List<Building> masterBuildingsList { get; } = new List<Building>();

        /// <summary>
        /// Indicates whether Buildings are being loaded.
        /// </summary>
        private bool _isLoading; 

        /// <summary>SelectedBuilding
        /// Gets or sets a value that specifies whether Buildings are being loaded. 
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }

        /// <summary>
        /// Backing field for the SelectedBuilding property.
        /// </summary>
        private Building _selectedBuilding;

        /// <summary>
        /// Gets or sets the selected order.
        /// </summary>
        public Building SelectedBuilding
        {
            get
            {
                return _selectedBuilding;
            }
            set
            {
                if (SetProperty(ref _selectedBuilding, value))
                {
                    // Clear out the existing customer.
                    SelectedCustomer = null;
                    if (_selectedBuilding != null)
                    {                      
                        Task.Run(() => loadCustomer(_selectedBuilding.CustomerId));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the SelectedBuilding value as type Object, which is required for using the 
        /// strongly-typed x:Bind with ListView.SelectedItem, which is of type Object. 
        /// </summary>
        public object SelectedBuildingAsObject
        {
            get { return SelectedBuilding; }
            set { SelectedBuilding = value as Building; }
        }


        /// <summary>
        /// Loads the specified customer and sets the
        /// SelectedCustomer property.
        /// </summary>
        /// <param name="customerId">The customer to load.</param>
        private async void loadCustomer(Guid customerId)
        {
            var customer = await DataProvider.Instance.GetCustomerAsync(customerId);
            
            await Utilities.CallOnUiThreadAsync(() =>
            {
                SelectedCustomer = customer;
            });
        }

        /// <summary>
        /// The selected customer.
        /// </summary>
        private Customer _selectedCustomer;

        /// <summary>
        /// Gets or sets the selected customer. 
        /// </summary>
        public Customer SelectedCustomer
        {
            get
            {
                return _selectedCustomer;
            }
            set
            {
                SetProperty(ref _selectedCustomer, value);
            }
        }

        /// <summary>
        /// Retrieves Buildings from the data source. 
        /// </summary>
        public async void LoadBuildings()
        {
            await Utilities.CallOnUiThreadAsync(() =>
            {
                IsLoading = true;
                // Buildings.Clear();
                masterBuildingsList.Clear();
            });
            
            var Buildings = await DataProvider.Instance.GetBuildings();

            await Utilities.CallOnUiThreadAsync(() =>
            {
                foreach(Building o in Buildings)
                {
                    Buildings.Add(o);
                    masterBuildingsList.Add(o);
                }
                IsLoading = false; 
            });

        }

        /// <summary>
        /// Submits a query to the data source. 
        /// </summary>
        /// <param name="query"></param>
        public async void QueryBuildings(string query)
        {
            IsLoading = true; 
            Buildings.Clear();
            if (!string.IsNullOrEmpty(query))
            {
                var results = await DataProvider.Instance.GetBuildingsAsync(query);

                await Utilities.CallOnUiThreadAsync(() =>
                {
                    foreach (Building o in results)
                    {
                        Buildings.Add(o);
                    }
                    IsLoading = false; 
                });
            }
        }

        /// <summary>
        /// Stores the order suggestions. 
        /// </summary>
        public ObservableCollection<Building> Buildingsuggestions { get; } = new ObservableCollection<Building>();

        /// <summary>
        /// Queries the database and updates the list of new order suggestions. 
        /// </summary>
        /// <param name="queryText">The query to submit.</param>
        public void UpdateBuildingsuggestions(string queryText)
        {

            Buildingsuggestions.Clear();
            if (!string.IsNullOrEmpty(queryText))
            {

                string[] parameters = queryText.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                var resultList = masterBuildingsList
                    .Where(x => parameters
                        .Any(y =>
                            x.AddressNeigborhood.StartsWith(y) ||
                            x.CustomerName.Contains(y) ||
                            x.ListedValue.ToString().StartsWith(y)))
                    .OrderByDescending(x => parameters
                        .Count(y =>
                            x.AddressNeigborhood.StartsWith(y) ||
                            x.CustomerName.Contains(y) ||
                            x.ListedValue.ToString().StartsWith(y)));

                foreach (Building o in resultList)
                {
                    Buildingsuggestions.Add(o);
                }
            }
        }

        /// <summary>
        /// Deletes the specified order from the database. 
        /// </summary>
        /// <param name="buildingToDelete">The order to delete.</param>
        public async Task DeleteBuilding(Building buildingToDelete)
        {
            var response = await DataProvider.Instance.DeleteBuildingAsync(buildingToDelete.Id);

            if (!response)
            {
                Buildings.Remove(buildingToDelete);
                SelectedBuilding = null;         
            }
            else
            {
                throw new BuildingDeletionException("Fail to Delete Building");
            }
        }

    }

    /// <summary>
    /// Occurs when there's an error deleting an order.
    /// </summary>
    public class BuildingDeletionException : Exception
    {
        public BuildingDeletionException() : base("Error deleting an order.")
        { }

        public BuildingDeletionException(string message) : base(message)
        { }

        public BuildingDeletionException(string message,
            Exception innerException) : base(message, innerException)
        { }
    }
}