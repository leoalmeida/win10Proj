using ProjetoApp.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProjetoApp.ViewModels
{
    /// <summary>
    /// Encapsulates data for the Building detail page. 
    /// </summary>
    public class BuildingDetailPageViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Creates an BuildingDetailPageViewModel that wraps the specified EnterpriseModels.Building.
        /// </summary>
        /// <param name="building">The building to wrap.</param>
        public BuildingDetailPageViewModel(Building building)
        {
            _building = building;
            
            if (building.Customer == null)
            {
                Task.Run(() => loadCustomer(_building.CustomerId));
            }
        }

        /// <summary>
        /// Creates a BuildingDetailPageViewModel from an Building ID.
        /// </summary>
        /// <param name="buildingId">The ID of the building to retrieve. </param>
        /// <returns></returns>
        public async static Task<BuildingDetailPageViewModel> CreateFromGuid(Guid buildingId)
        {
            var building = await getBuilding(buildingId);
            return new BuildingDetailPageViewModel(building);
        }



        /// <summary>
        /// The EnterpriseModels.Building this object wraps. 
        /// </summary>
        private Building _building;

        /// <summary>
        /// Loads the customer with the specified ID. 
        /// </summary>
        /// <param name="customerId">The ID of the customer to load.</param>
        private async void loadCustomer(Guid customerId)
        {

            var customer = await DataProvider.Instance.GetCustomerAsync(customerId);

            await Utilities.CallOnUiThreadAsync(() =>
            {
                Customer = customer;
            });
        }

        /// <summary>
        /// Returns the building with the specified ID.
        /// </summary>
        /// <param name="buildingId">The ID of the building to retrieve.</param>
        /// <returns>The building, if it exists; otherwise, null. </returns>
        private static async Task<Building> getBuilding(Guid buildingId)
        {
            var building = await DataProvider.Instance.GetBuildingAsync(buildingId);
            return building;
        }

        /// <summary>
        /// Gets a value that specifies whether the user can refresh the page.
        /// </summary>
        public bool CanRefresh
        {
            get
            {
                return _building != null && !HasChanges && IsExistingBuilding;
            }
        }

        /// <summary>
        /// Gets a value that specifies whether the user can revert changes. 
        /// </summary>
        public bool CanRevert
        {
            get
            {
                return _building != null && HasChanges && IsExistingBuilding;
            }
        }

        /// <summary>
        /// Gets or sets the building's ID.
        /// </summary>
        public Guid Id
        {
            get
            {
                return _building.Id;
            }
            set
            {
                if (_building.Id != value)
                {
                    _building.Id = value;
                    OnPropertyChanged(nameof(Id));
                    HasChanges = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the ID of the building's customer.
        /// </summary>
        public Guid CustomerId
        {
            get
            {
                return _building.CustomerId;
            }
            set
            {
                if (_building.CustomerId != value)
                {
                    _building.CustomerId = value;
                    OnPropertyChanged(nameof(CustomerId));
                    HasChanges = true;
                }
            }
        }


        /// <summary>
        /// Gets or sets a value that indicates whether the user has changed the building. 
        /// </summary>
        bool _hasChanges = false;
        public bool HasChanges
        {
            get
            {
                return _hasChanges;
            }
            set
            {
                if (value != _hasChanges)
                {
                    // Only record changes after the building has loaded. 
                    if (IsLoaded)
                    {
                        _hasChanges = value;
                        OnPropertyChanged(nameof(HasChanges));
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value that specifies whether htis is an existing building.
        /// </summary>
        public bool IsExistingBuilding => !IsNewBuilding;

        public bool IsLoaded => _building != null && (IsNewBuilding || _building.Customer != null);

        public bool IsNotLoaded => !IsLoaded;

        public bool IsNewBuilding => _building.ListedValue == 0; 

        
        /// <summary>
        /// Gets or sets the customer for this building. This value is null
        /// unless you manually retrieve the customer (using CustomerId) and 
        /// set it. 
        /// </summary>
        public Customer Customer
        {
            get
            {
                return _building.Customer;
            }
            set
            {
                if (_building.Customer != value)
                {
                    var isLoadingOperation = _building.Customer == null &&
                        value != null && !IsNewBuilding;
                    _building.Customer = value;
                    OnPropertyChanged(nameof(Customer));
                    if (isLoadingOperation)
                    {
                        OnPropertyChanged(nameof(IsLoaded));
                        OnPropertyChanged(nameof(IsNotLoaded));
                    }
                    else
                    {
                        HasChanges = true;
                    }
                }

            }
        }

        private ObservableCollection<Characteristic> _characteristics = new ObservableCollection<Characteristic>();

        /// <summary>
        /// Gets the line items in this invoice. 
        /// </summary>
        public virtual ObservableCollection<Characteristic> Characteristics
        {
            get
            {
                return _characteristics;
            }
            set
            {
                if (_characteristics != value)
                {
                    if (value != null)
                    {
                        value.CollectionChanged += characteristics_Changed;
                    }

                    if (_characteristics != null)
                    {
                        _characteristics.CollectionChanged -= characteristics_Changed;
                    }
                    _characteristics = value;
                    OnPropertyChanged(nameof(Characteristics));
                    HasChanges = true;
                }

            }
        }

        /// <summary>
        /// Notifies anyone listening to this object that a line item changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void characteristics_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Characteristics != null)
            {
                _building.Characteristics = Characteristics.ToList<Characteristic>();
            }

            OnPropertyChanged(nameof(Characteristics));
            OnPropertyChanged(nameof(CustomerId));
            OnPropertyChanged(nameof(ListedValue));
            HasChanges = true;
        }

        private CharacteristicWrapper _newCharacteristic;
        /// <summary>
        /// Gets or sets the line item that the user is currently working on.
        /// </summary>
        public CharacteristicWrapper NewCharacteristic
        {
            get
            {
                return _newCharacteristic;
            }
            set
            {
                if (value != _newCharacteristic)
                {

                    if (value != null)
                    {
                        value.PropertyChanged += _newCharacteristic_PropertyChanged;
                    }
                    if (_newCharacteristic != null)
                    {
                        _newCharacteristic.PropertyChanged -= _newCharacteristic_PropertyChanged;
                    }

                    _newCharacteristic = value;
                    OnPropertyChanged(nameof(NewCharacteristic));
                }
            }

        }

        private void _newCharacteristic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(NewCharacteristic));
            OnPropertyChanged(nameof(HasNewCharacteristic));
        }

        /// <summary>
        /// Gets or sets whether there is a new line item in progress.
        /// </summary>
        public bool HasNewCharacteristic
        {
            get
            {
                return NewCharacteristic != null;
            }
        }

        /// <summary>
        /// Gets or sets the date this building was placed. 
        /// </summary>
        public DateTime DatePlaced
        {
            get
            {
                return _building.DatePlaced;
            }
            set
            {
                if (_building.DatePlaced != value)
                {
                    _building.DatePlaced = value;
                    OnPropertyChanged(nameof(DatePlaced));
                    HasChanges = true;
                }

            }
        }

        /// <summary>
        /// Gets or sets the date this building was filled. 
        /// This value is automatically updated when the 
        /// BuildingStatus changes. 
        /// </summary>
        public DateTime? DateFilled
        {
            get
            {
                return _building.DateFilled;

            }
            set
            {
                if (value != _building.DateFilled)
                {
                    _building.DateFilled = value;
                    OnPropertyChanged(nameof(DateFilled));
                    HasChanges = true;
                }
            }
        }

        /// <summary>
        /// Gets the subtotal. This value is calculated automatically. 
        /// </summary>
        public decimal ListedValue => _building.ListedValue;


        /// <summary>
        /// Gets the tax. This value is calculated automatically. 
        /// </summary>
        public decimal MarketValue => _building.MarketValue;

        /// <summary>
        /// Gets or sets the building address, which may be different
        /// from the customer's primary address. 
        /// </summary>
        public string AddressStreet
        {
            get
            {
                return _building.AddressStreet;
            }
            set
            {
                if (_building.AddressStreet != value)
                {
                    _building.AddressStreet = value;
                    OnPropertyChanged(nameof(AddressStreet));
                    HasChanges = true;
                }
             }
        }  

        /// <summary>
        /// Gets or sets the building status.
        /// </summary>
        public BuildingStatus Status
        {
            get
            {
                return _building.Status;
            }
            set
            {
                if (_building.Status != value)
                {
                    _building.Status = value;
                    OnPropertyChanged(nameof(Status));

                    // Update the DateFilled value.
                    DateFilled = _building.Status == BuildingStatus.Void ? (Nullable<DateTime>)DateTime.Now : null;
                    HasChanges = true;

                }

            }
        }

        /// <summary>
        /// Gets or sets the name of the building's customer. 
        /// </summary>
        public string CustomerName
        {
            get
            {
                return _building.CustomerName;
            }
            set
            {
                if (_building.CustomerName != value)
                {
                    _building.CustomerName = value;
                    OnPropertyChanged("CustomerName");
                }

            }
        }

        /// <summary>
        /// Gets a string representation of the building.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{_building.AddressStreet} {_building.AddressStreetNumber} {_building.AddressComplement}";

        /// <summary>
        /// Fired when a property value changes. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value changed. 
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Converts an Building to an BuildingDetailPageViewModel.
        /// </summary>
        /// <param name="building">The EnterpriseModels.Building to convert.</param>
        public static implicit operator BuildingDetailPageViewModel(Building building)
        {

            return new BuildingDetailPageViewModel(building);
        }


        /// <summary>
        /// Converts an BuildingDetailPageViewModel to an Building.
        /// </summary>
        /// <param name="building">The BuildingDetailPageViewModel to convert.</param>
        public static implicit operator Building(BuildingDetailPageViewModel building)
        {
            building._building.Characteristics = building.Characteristics.ToList<Characteristic>();
            return building._building;
        }

        /// <summary>
        /// Gets the set of building status values so we can populate the building status combo box. 
        /// </summary>
        public List<string> BuildingStatusValues => Enum.GetNames(typeof(BuildingStatus)).ToList();


        /// <summary>
        /// Saves the current building to the database. 
        /// </summary>
        public async Task SaveBuilding()
        {
            Building result = null;
            try
            {
                result = await DataProvider.Instance.SaveBuildingAsync(_building);
            }
            catch (Exception ex)
            {
                throw new BuildingSavingException("Unable to save. There might have been a problem " +
                    "connecting to the database. Please try again.", ex);
            }

            if (result != null)
            {
                await Utilities.CallOnUiThreadAsync(() => HasChanges = false);
            }
            else
            {
                await Utilities.CallOnUiThreadAsync(() => new BuildingSavingException(
                    "Unable to save. There might have been a problem " +
                    "connecting to the database. Please try again."));
            }
        }

        /// <summary>
        /// Stores the product suggestions. 
        /// </summary>
        public ObservableCollection<Characteristic> CharacteristicSuggestions { get; } =
            new ObservableCollection<Characteristic>();

        /// <summary>
        /// Queries the database and updates the list of new product suggestions. 
        /// </summary>
        /// <param name="queryText">The query to submit.</param>
        public async void UpdateCharacteristicSuggestions(string queryText)
        {
            CharacteristicSuggestions.Clear();

            if (!string.IsNullOrEmpty(queryText))
            {
                var suggestions = await DataProvider.Instance.GetSuggestedCharacteristics();

                foreach (Characteristic p in suggestions)
                {
                    CharacteristicSuggestions.Add(p);
                }
            }
        }

    }

    public class CharacteristicWrapper : INotifyPropertyChanged
    {
        Characteristic _item;

        public CharacteristicWrapper()
        {
            _item = new Characteristic();
        }

        public CharacteristicWrapper(Characteristic item)
        {
            _item = item;
        }

        public string Name
        {
            get
            {
                return _item.Name;
            }
            set
            {
                if (_item.Name != value)
                {
                    _item.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public CharacteristicType CharacteristicType
        {
            get
            {
                return _item.Type;
            }
            set
            {
                if (_item.Type != value)
                {
                    _item.Type = value;
                    OnPropertyChanged(nameof(CharacteristicType));
                }

            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Converts an Characteristic to an CharacteristicWrapper.
        /// </summary>
        /// <param name="building">The Characteristic to convert.</param>
        public static implicit operator CharacteristicWrapper(Characteristic item) =>

             new CharacteristicWrapper(item);


        /// <summary>
        /// Converts an CharacteristicWrapper to an Characteristic.
        /// </summary>
        /// <param name="item">The CharacteristicWrapper to convert.</param>
        public static implicit operator Characteristic(CharacteristicWrapper item) =>
            item._item;        

        /// <summary>
        /// Gets the set of order status values so we can populate the order status combo box. 
        /// </summary>
        public List<string> CharacteristicTypeValues => Enum.GetNames(typeof(CharacteristicType)).ToList();

    }

    public class BuildingSavingException : Exception
    {

        public BuildingSavingException() : base("Error saving an building.")
        {
        }

        public BuildingSavingException(string message) : base(message)
        {
        }

        public BuildingSavingException(string message,
            Exception innerException) : base(message, innerException)
        {

        }
    }
}
