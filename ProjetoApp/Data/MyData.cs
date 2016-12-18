using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoApp.Data
{
    public class MyData
    {
        public List<Building> Buildings { get; set; }
        public List<Characteristic> Characteristics { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Negotiation> Negotiations { get; set; }
        public List<User> Users { get; set; }


        public List<Option> MyOptions { get; set; }
        public List<Option> FriendsOptions { get; set; }
        public Option CurrentOption { get; set; }
    }
}
