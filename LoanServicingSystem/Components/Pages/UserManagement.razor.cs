using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;


namespace LoanServicingSystem.Components.Pages
{
    public partial class UserManagement : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;

        private List<CoreData.Identity.Users> AllUsers { get; set; } = new();
        public List<Roles> AvailableRoles { get; set; } = new();

        public bool ShowSidebar { get; set; } = false;
        public bool IsUpdate { get; set; } = false;
        public bool IsSaving { get; set; } = false;
        public string? ModalErrorMessage { get; set; }

        public CoreData.Identity.Users ActiveUser { get; set; } = new();

        // Search & Filter state
        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; ApplyFilters(); }
        }

        private string _selectedRoleFilter = string.Empty;
        public string SelectedRoleFilter
        {
            get => _selectedRoleFilter;
            set { _selectedRoleFilter = value; ApplyFilters(); }
        }

        public IEnumerable<CoreData.Identity.Users> FilteredUsers { get; set; } = Array.Empty<CoreData.Identity.Users>();

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    AllUsers = await CoreData.Identity.Users.GetAllUsersWithRolesAsync(DatabaseConnection);
                    AvailableRoles = await Roles.GetAllRolesAsync(DatabaseConnection);
                    ApplyFilters();
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private void ApplyFilters()
        {
            var query = AllUsers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedRoleFilter))
            {
                query = query.Where(u => u.RoleName == SelectedRoleFilter);
            }

            FilteredUsers = query.ToList();
        }

        // --- UI Action Handlers ---

        protected void HandleInviteUser()
        {
            IsUpdate = false;
            ModalErrorMessage = null;
            ActiveUser = new CoreData.Identity.Users(); // Fresh entity
            ShowSidebar = true;
            StateHasChanged();
        }

        protected void HandleEditRole(Guid userId)
        {
            var user = AllUsers.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                IsUpdate = true;
                ModalErrorMessage = null;
                // Clone the essential properties to avoid modifying the grid item directly before save
                ActiveUser = new CoreData.Identity.Users
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    RoleId = user.RoleId
                };
                ShowSidebar = true;
            }
        }

        protected void CloseSidebar()
        {
            ShowSidebar = false;
        }

        protected async Task SaveUserAsync()
        {
            ModalErrorMessage = null;

            // Manual C# validation since we removed the DataAnnotations
            if (string.IsNullOrWhiteSpace(ActiveUser.Username) ||
                string.IsNullOrWhiteSpace(ActiveUser.Email) ||
                ActiveUser.RoleId == Guid.Empty)
            {
                ModalErrorMessage = "Please fill in all required fields.";
                return;
            }

            IsSaving = true;
            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection missing.");

                if (IsUpdate)
                {
                    // Update existing user's role
                    await CoreData.Identity.Users.UpdateUserRoleAsync(DatabaseConnection, ActiveUser.Id, ActiveUser.RoleId, "SystemAdmin");
                }
                else
                {
                    // Validate password for new users
                    if (string.IsNullOrWhiteSpace(ActiveUser.PasswordHash))
                    {
                        ModalErrorMessage = "A temporary password is required for new users.";
                        IsSaving = false;
                        return;
                    }

                    // Complete the entity setup before inserting
                    ActiveUser.Id = Guid.NewGuid();
                    ActiveUser.IsActive = true;
                    ActiveUser.UpdatedBy = "SystemAdmin";

                    await CoreData.Identity.Users.InsertUserAsync(DatabaseConnection, ActiveUser);
                }

                // Close sidebar and refresh grid
                ShowSidebar = false;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ModalErrorMessage = $"Error saving user: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }
        protected async Task HandleToggleStatus(Guid userId)
        {
            if (DatabaseConnection == null) return;
            var user = AllUsers.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                await CoreData.Identity.Users.ToggleUserStatusAsync(DatabaseConnection, userId, !user.IsActive, "SystemAdmin");
                await LoadDataAsync(); // Refresh grid
            }
        }

        // --- UI Helper Methods ---

        protected string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "U";
            return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
        }

        protected string GetRoleBadgeColor(string roleName)
        {
            return roleName switch
            {
                "Admin" => "bg-purple-100 text-purple-800",
                "Loan Officer" => "bg-blue-100 text-blue-800",
                "Collections" => "bg-amber-100 text-amber-800",
                _ => "bg-slate-100 text-slate-800"
            };
        }

        protected string GetAvatarColor(string roleName)
        {
            return roleName switch
            {
                "Admin" => "bg-indigo-100 text-indigo-700",
                "Loan Officer" => "bg-blue-100 text-blue-700",
                "Collections" => "bg-amber-100 text-amber-700",
                _ => "bg-slate-100 text-slate-700"
            };
        }


    }
}