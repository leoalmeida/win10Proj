//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

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
