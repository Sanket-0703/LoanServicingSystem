using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class UserManagement : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        // =========================================
        // Page State
        // =========================================

        public bool IsLoading { get; set; } = true;

        public bool ShowSidebar { get; set; } = false;

        public bool IsUpdate { get; set; } = false;

        public bool IsSaving { get; set; } = false;

        public string? ModalErrorMessage { get; set; }

        // =========================================
        // Data Collections
        // =========================================

        private List<Users> AllUsers { get; set; } = new();

        public IEnumerable<Users> FilteredUsers { get; set; } = Array.Empty<Users>();

        public List<Roles> AvailableRoles { get; set; } = new();

        public Users ActiveUser { get; set; } = new();

        // =========================================
        // Search & Filters
        // =========================================

        private string _searchTerm = string.Empty;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                ApplyFilters();
            }
        }

        private string _selectedRoleFilter = string.Empty;

        public string SelectedRoleFilter
        {
            get => _selectedRoleFilter;
            set
            {
                _selectedRoleFilter = value;
                ApplyFilters();
            }
        }

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads users and available roles
        /// when the page is initialized.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        // =========================================
        // Data Loading
        // =========================================

        /// <summary>
        /// Retrieves users and roles from the database.
        /// </summary>
        private async Task LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    AllUsers = await Users.GetAllUsersWithRolesAsync(DatabaseConnection);

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

        /// <summary>
        /// Applies search and role filters
        /// to the user list.
        /// </summary>
        private void ApplyFilters()
        {
            IEnumerable<Users> query = AllUsers;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedRoleFilter))
            {
                query = query.Where(u =>
                    u.RoleName == SelectedRoleFilter);
            }

            FilteredUsers = query.ToList();
        }

        // =========================================
        // Sidebar Actions
        // =========================================

        /// <summary>
        /// Opens the sidebar for creating
        /// a new user.
        /// </summary>
        protected void HandleInviteUser()
        {
            IsUpdate = false;

            ModalErrorMessage = null;

            ActiveUser = new Users();

            ShowSidebar = true;

            StateHasChanged();
        }

        /// <summary>
        /// Opens the sidebar for editing
        /// an existing user's role.
        /// </summary>
        protected void HandleEditRole(Guid userId)
        {
            var user = AllUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return;

            IsUpdate = true;

            ModalErrorMessage = null;

            // Clone the entity to avoid updating
            // the grid before saving.
            ActiveUser = new Users
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId
            };

            ShowSidebar = true;
        }

        /// <summary>
        /// Closes the user sidebar.
        /// </summary>
        protected void CloseSidebar()
        {
            ShowSidebar = false;
        }

        // =========================================
        // Save Operations
        // =========================================

        /// <summary>
        /// Creates a new user or updates
        /// an existing user's role.
        /// </summary>
        protected async Task SaveUserAsync()
        {
            ModalErrorMessage = null;

            // Validate required fields
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
                if (DatabaseConnection == null)
                    throw new Exception("Database connection missing.");

                if (IsUpdate)
                {
                    await Users.UpdateUserRoleAsync(
                        DatabaseConnection,
                        ActiveUser.Id,
                        ActiveUser.RoleId,
                        "SystemAdmin");
                }
                else
                {
                    // Temporary password is required
                    if (string.IsNullOrWhiteSpace(ActiveUser.PasswordHash))
                    {
                        ModalErrorMessage = "A temporary password is required for new users.";

                        IsSaving = false;

                        return;
                    }

                    ActiveUser.Id = Guid.NewGuid();
                    ActiveUser.IsActive = true;
                    ActiveUser.UpdatedBy = "SystemAdmin";

                    await Users.InsertUserAsync(
                        DatabaseConnection,
                        ActiveUser);
                }

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

        /// <summary>
        /// Enables or disables a user account.
        /// </summary>
        protected async Task HandleToggleStatus(Guid userId)
        {
            if (DatabaseConnection == null)
                return;

            var user = AllUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return;

            await Users.ToggleUserStatusAsync(
                DatabaseConnection,
                userId,
                !user.IsActive,
                "SystemAdmin");

            await LoadDataAsync();
        }

        // =========================================
        // UI Helpers
        // =========================================

        /// <summary>
        /// Returns initials for the avatar.
        /// </summary>
        protected string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "U";

            return name.Length >= 2
                ? name[..2].ToUpper()
                : name.ToUpper();
        }

        /// <summary>
        /// Returns the badge color
        /// based on the user's role.
        /// </summary>
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

        /// <summary>
        /// Returns the avatar color
        /// based on the user's role.
        /// </summary>
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