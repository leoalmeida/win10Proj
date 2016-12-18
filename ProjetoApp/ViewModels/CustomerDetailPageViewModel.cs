using ProjetoApp.Commands;
using PropertyChanged;

namespace ProjetoApp.ViewModels
{
    [ImplementPropertyChanged]
    /// <summary>
    /// Encapsulates data for the Customer detail page. 
    /// </summary>
    public class CustomerDetailPageViewModel : BindableBase
    {
        /// <summary>
        /// Creates a CustomerDetailPageViewModel that wraps the specified EnterpriseModels.Customer
        /// </summary>
        public CustomerDetailPageViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelEditsCommand = new RelayCommand(OnCancelEdits);
            StartEditCommand = new RelayCommand(OnStartEdit);
            RefreshCommand = new RelayCommand(OnRefresh);
        }

        private CustomerViewModel _customer;

        /// <summary>
        /// Gets and sets the current customer values.
        /// </summary>
        public CustomerViewModel Customer
        {
            get { return _customer; }

            set
            {
                if (SetProperty(ref _customer, value) == true)
                {
                    EditDataContext = value;
                    if (string.IsNullOrWhiteSpace(Customer.Name))
                    {
                        IsInEdit = true;
                    }
                }
            }
        }

        private bool _isInEdit = false;
        /// <summary>
        /// Gets and sets the current edit mode.
        /// </summary>
        public bool IsInEdit
        {
            get { return _isInEdit; }

            set
            {
                if (SetProperty(ref _isInEdit, value) == true)
                {
                    OnPropertyChanged(nameof(EditDataContext));
                }
            }
        }

        private string _errorText = null;
        /// <summary>
        /// Gets and sets the relevant error text.
        /// </summary>
        public string ErrorText
        {
            get { return _errorText; }

            set
            {
                SetProperty(ref _errorText, value);
            }
        }

        public CustomerViewModel EditDataContext { get; set; }

        public RelayCommand SaveCommand { get; private set; }

        /// <summary>
        /// Saves customer data that has been edited.
        /// </summary>
        private void OnSave()
        {
            if (Customer.CanSave == true)
            {
                Customer.SaveCommand.Execute(Customer);
                IsInEdit = false;
            }
        }

        public RelayCommand CancelEditsCommand { get; private set; }
        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        private void OnCancelEdits()
        {
            RefreshCommand.Execute(null);
            IsInEdit = false;
        }

        public RelayCommand StartEditCommand { get; private set; }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        private void OnStartEdit()
        {
            IsInEdit = true;
        }

        public RelayCommand RefreshCommand { get; private set; }

        /// <summary>
        /// Resets the customer detail fields to the current values.
        /// </summary>
        private void OnRefresh()
        {
            if (Customer.IsNewCustomer == false)
            {
                EditDataContext = null;
                Customer.Restore();
                EditDataContext = Customer;
            }
        }
    }
}
