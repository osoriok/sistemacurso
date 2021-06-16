using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCurso.Areas.Users.Models;
using SistemaCurso.Data;
using SistemaCurso.Library;

namespace SistemaCurso.Areas.Users.Pages.Account
{
    public class RegistrarModel : PageModel
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;
        private LUsersRoles _usersRole;
        private static InputModel _dataInput;
        private Uploadimage _uploadimage;
        private IWebHostEnvironment _environment;


        public RegistrarModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IWebHostEnvironment environment
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _environment = environment;
            _usersRole = new LUsersRoles();
            _uploadimage = new Uploadimage();
        }


        public void OnGet()
        {
            if(_dataInput != null)
            {
                Input = _dataInput;
                Input.rolesLista = _usersRole.getRoles(_roleManager);
                Input.AvatarImage = null;
            }
            else
            {
                Input = new InputModel
                {
                    rolesLista = _usersRole.getRoles(_roleManager)
                };

            }

            
        }


        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel : InputModelRegister
        {
            public IFormFile AvatarImage { get; set; }
            [TempData]
            public string ErrorMessage { get; set; }
            public List<SelectListItem> rolesLista { get; set; }
        }
        public async Task<IActionResult> OnPost()
        {
            if(await SaveAsync())
            {
                return Redirect("/Users/Users?area=Users");//Users/Users
            }
            else
            {
                return Redirect("/Usuarios/Registrar");
            }
        }
        private async Task<bool> SaveAsync()
        {
            _dataInput = Input;
            var valor = false;
            if (ModelState.IsValid)
            {
                var userList = _userManager.Users.Where(u => u.Email.Equals(Input.email)).ToList();
                if (userList.Count.Equals(0)) {
                    var strategy = _context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () => {
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                var user = new IdentityUser
                                {
                                    UserName = Input.email,
                                    Email = Input.email,
                                    PhoneNumber = Input.phoneNumber
                                };
                                var result = await _userManager.CreateAsync(user, Input.password);
                                if (result.Succeeded) {
                                    await _userManager.AddToRoleAsync(user, Input.role);
                                    var dataUser = _userManager.Users.Where(u => u.Email.Equals(Input.email)).ToList().Last();
                                    var imageByte = await _uploadimage.ByteAvatarImageAsync(Input.AvatarImage, _environment, "images/images/default.png");
                                    var t_user = new TUsers
                                    {
                                        name = Input.name,
                                        lastname = Input.lastNames,
                                        nid = Input.identification,
                                        email = Input.email,
                                        phonenumber = Input.phoneNumber,
                                        iduser = dataUser.Id,
                                        image = imageByte

                                    };
                                    await _context.AddAsync(t_user);
                                    _context.SaveChanges();

                                    transaction.Commit();
                                    _dataInput = null;
                                    valor = true;
                                }
                                else
                                {
                                    foreach(var item in result.Errors)
                                    {
                                        _dataInput.ErrorMessage = item.Description;
                                    }
                                    valor = false;

                                    transaction.Rollback();
                                }
                            }
                            catch (Exception ex)
                            { 
                                _dataInput.ErrorMessage = ex.Message;
                                transaction.Rollback();
                                valor = false;
                            }
                        }
                    });
                }
                else
                {
                    
                    _dataInput.ErrorMessage = $"El {Input.email} ya está registrado";
                    valor = false;
                }
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _dataInput.ErrorMessage += error.ErrorMessage;
                    }

                }
                valor = false;
            }

            return valor;
        }
    }
}

