namespace ISpanShop.MVC.Areas.Admin.Models.Members
{
    public class AdminPermissionIndexVm
    {
        public List<AdminPermissionItemVm> Admins { get; set; } = new();
        public List<PermissionOptionVm> AvailablePermissions { get; set; } = new();
    }

    public class AdminPermissionItemVm
    {
        public int UserId { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public List<string> CurrentPermissions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class PermissionOptionVm
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }

    public class UpdatePermissionRequest
    {
        public int UserId { get; set; }
        public List<string> PermissionKeys { get; set; } = new();
    }
}
