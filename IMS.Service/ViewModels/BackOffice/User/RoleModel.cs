using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.User
{
    public class RoleModel : ServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RoleListViewModel
    {
        public List<string> Modules { get; set; }
        public List<string> Permissions { get; set; }
        public int Id { get; set; }
        public string Role { get; set; }
    }
}
