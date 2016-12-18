using ProjetoApp.Data;
using ProjetoApp.Commands;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ProjetoApp.ViewModels
{
    [ImplementPropertyChanged]
    public class CustomerViewModel : BindableBase
    {
        public CustomerViewModel() : this(new Customer())
        { }

        public CustomerViewModel(Customer model)
        {
            SaveCommand = new RelayCommand(OnSave);

            Model = model;

            FirstName = _firstNameOriginal = model.FirstName;
            LastName = _lastNameOriginal = model.LastName;
            Email = _emailOriginal = model.Email;
            Phone = _phoneOriginal = model.Phone;
            Address = _addressOriginal = model.Address;
            RG = _rgOriginal = model.RG;
            CPF = _cpfOriginal = model.CPF;
            Relation = _relationOriginal = model.Relation;

            Id = model.Id;

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                IsNewCustomer = true;
            }
            else
            {
                GetBuildingsList(model);
            }
        }

        private string _errorText = null;
        public string ErrorText
        {
            get { return _errorText; }

            set
            {
                SetProperty(ref _errorText, value);
                CanSave = string.IsNullOrWhiteSpace(value);
            }
        }

        public bool HasChanges { get; set;} = false;

        public bool IsNewCustomer { get; private set; } = false;

        public bool CanSave { get; private set; } = true;

        public bool IsLoading { get; private set; } = false;

        public ObservableCollection<Building> Buildings { get; private set; } = 
            new ObservableCollection<Building>();

        public Customer Model { get; private set; }

        private string _firstNameOriginal;
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }

            set
            {
                // Don't check on initial load.
                if (FirstName == null)
                {
                    SetProperty(ref _firstName, value);
                }
                // Make sure text is entered for name.
                else if (string.IsNullOrWhiteSpace(value))
                {
                    ErrorText = "O nome é obrigatório. ";
                }
                else if (SetProperty(ref _firstName, value) == true)
                {
                    OnPropertyChanged(nameof(Name));
                    ErrorText = null;
                }
            }
        }

        private string _lastNameOriginal;
        private string _lastName;
        public string LastName
        {
            get { return _lastName; }

            set
            {
                // Don't check on initial load.
                if (LastName == null)
                {
                    SetProperty(ref _lastName, value);
                }
                // Make sure text is entered for name.
                else if (string.IsNullOrWhiteSpace(value))
                {
                    ErrorText = "Sobrenome obrigatório. ";
                }
                else if (SetProperty(ref _lastName, value) == true)
                {
                    OnPropertyChanged(nameof(Name));
                    ErrorText = null;
                }
            }
        }

        public string Name
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        private string _rgOriginal;
        private string _rg;
        public string RG
        {
            get { return _rg; }

            set
            {
                SetProperty(ref _rg, value);
            }
        }

        private string _cpfOriginal;
        private string _cpf;
        public string CPF
        {
            get { return _cpf; }

            set
            {
                SetProperty(ref _cpf, value);
            }
        }

        private string _relationOriginal;
        private string _relation;
        public string Relation
        {
            get { return _relation; }

            set
            {
                SetProperty(ref _relation, value);
            }
        }

        private string _emailOriginal;
        private string _email;
        public string Email
        {
            get { return _email; }

            set
            {
                // Don't check on initial load.
                if (Email == null)
                {
                    SetProperty(ref _email, value);
                }
                // Check if email address string is valid, convert to lowercase 
                // for easier duplicate checking.
                else if (IsValidEmail(value))
                {
                    value = value.ToLower();
                    if (SetProperty(ref _email, value) == true)
                    {
                        ErrorText = null;
                    }
                }
                else
                {
                    ErrorText = ("Formato de Email inválido. Utilize xxxx@xxx.xxx ");
                }
            }
        }

        private string _phoneOriginal;
        private string _phone;
        public string Phone
        {
            get { return _phone; }

            set
            {
                // Don't check on initial load.
                if (Phone == null)
                {
                    SetProperty(ref _phone, value);
                }
                else if (IsValidPhoneNumber(value))
                {
                    if (SetProperty(ref _phone, value) == true)
                    {
                        ErrorText = null;
                    }
                }
                else
                {
                    ErrorText = ("Número de telefone inválido. Utilize (xx) xxxxx-xxxx");
                }
            }
        }

        private string _addressOriginal;
        private string _address;
        public string Address
        {
            get { return _address; }

            set
            {
                SetProperty(ref _address, value);
            }
        }

       

        private Guid _id;
        public Guid Id
        {
            get { return _id; }

            set
            {
                SetProperty(ref _id, value);
            }
        }
        public int BuildingsCount
        {
            get
            {
                if (Buildings != null)
                {
                    return Buildings.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override string ToString() => $"{Name} ({Email})";

        private async void GetBuildingsList(Customer customer)
        {
            IsLoading = true;

            var buildings = await DataProvider.Instance.GetCustomerBuildings(customer);

            await Utilities.CallOnUiThreadAsync(() =>
            {
                if (buildings != null)
                {
                    foreach(Building o in buildings)
                    {
                        Buildings.Add(o);
                    }
                }
                else
                {
                    // There was a problem retreiving customers. Let the user know.
                    ErrorText = "Erro ao buscar os imoveis.";
                }

                IsLoading = false;
            });
        }

        public RelayCommand SaveCommand { get; private set; }
        private async void OnSave()
        {
            if (CanSave == true)
            {
                Model.FirstName = _firstNameOriginal = FirstName;
                Model.LastName = _lastNameOriginal = LastName;
                Model.Email = _emailOriginal = Email;
                Model.Phone = _phoneOriginal = Phone;
                Model.Address = _addressOriginal = Address;
                Model.RG = _rgOriginal = RG;
                Model.CPF = _cpfOriginal = CPF;
                Model.Relation = _relationOriginal = Relation;

                
                await DataProvider.Instance.SaveCustomerAsync(Model);

                HasChanges = true;
                if (IsNewCustomer == true)
                {
                    IsNewCustomer = false;
                }
            }
        }

        public void Restore()
        {
            FirstName = _firstNameOriginal;
            LastName = _lastNameOriginal;
            Email = _emailOriginal;
            Phone = _phoneOriginal;
            Address = _addressOriginal;
            RG = _rgOriginal;
            CPF = _cpfOriginal;
            Relation = _relationOriginal;

            ErrorText = null;
        }


        public bool IsValidEmail(string email) =>
            new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$").IsMatch(email);

        public bool IsValidPhoneNumber(string phone) =>
            new Regex(@"^\([1-9]{2}\) [2-9][0-9]{3,4}\-[0-9]{4}$").IsMatch(phone);
        // new Regex(@"^\(?([0-9]{2})\)?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$").IsMatch(phone);
    }
}