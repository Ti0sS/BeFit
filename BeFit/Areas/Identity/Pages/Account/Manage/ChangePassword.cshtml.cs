using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeFit.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ChangePasswordModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Aktualne hasło")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nowe hasło")]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Powtórz nowe hasło")]
            [Compare("NewPassword", ErrorMessage = "Hasła muszą być identyczne.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("Nie znaleziono użytkownika.");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("Nie znaleziono użytkownika.");

            var result = await _userManager.ChangePasswordAsync(
                user,
                Input.OldPassword,
                Input.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Hasło zostało zmienione.";

            return RedirectToPage();
        }
    }
}
