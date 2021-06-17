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
using Newtonsoft.Json;
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
        public static InputModel _dataInput;
        private Uploadimage _uploadimage;
        private static InputModelRegister _dataUser1, _dataUser2;
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


        public void OnGet(int id)
        {
            if (id.Equals(0))
            {
                _dataUser2 = null;
            }
            if (_dataInput != null || _dataUser1 != null || _dataUser2 != null)
            {
                if (_dataInput != null)
                {
                    Input = _dataInput;
                    Input.rolesLista = _usersRole.getRoles(_roleManager);
                    Input.AvatarImage = null;
                }
                else
                {
                    if (_dataUser1 != null || _dataUser2 != null)
                    {
                        if (_dataUser2 != null)
                            _dataUser1 = _dataUser2;
                        Input = new InputModel
                        {
                            Id = _dataUser1.Id,
                            name = _dataUser1.name,
                            lastNames = _dataUser1.lastNames,
                            identification = _dataUser1.identification,
                            ID = _dataUser1.identification,
                            email = _dataUser1.email,
                            Image = _dataUser1.Image,
                            phoneNumber = _dataUser1.IdentityUser.PhoneNumber,
                            rolesLista = getRoles(_dataUser1.role),
                        };
                        if (_dataInput != null)
                        {
                            Input.ErrorMessage = _dataInput.ErrorMessage;
                        }
                    }


                }
            }else
            {
                Input = new InputModel
                {
                    rolesLista = _usersRole.getRoles(_roleManager)
                };
            }

            _dataUser2 = _dataUser1;
            _dataUser1 = null;
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
        public async Task<IActionResult> OnPost(String dataUser)
        {

            if (dataUser == null)
            {
                if (_dataUser2 == null)
                {
                    if (await SaveAsync())
                    {  

                        return Redirect("/Users/Users?area=Users");//Users/Users
                    }
                    else
                    {
                        return Redirect("/Usuarios/Registrar");
                    }
                }
                else
                {
                    if (await UpdateAsync())
                    {
                        var url = $"/Users/Account/Details?id={_dataUser2.Id}";
                        _dataUser2 = null;
                        return Redirect(url);
                    }
                    else
                    {
                        return Redirect("/Usuarios/Registrar");
                    }
                }

            }
            else
            {
                _dataUser1 = JsonConvert.DeserializeObject<InputModelRegister>(dataUser);
                return Redirect("/Usuarios/Registrar?id=1");
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

        private List<SelectListItem> getRoles(String role)
        {
            List<SelectListItem> rolesLista = new List<SelectListItem>();
            rolesLista.Add(new SelectListItem
            {
                Text = role
            });
            var roles = _usersRole.getRoles(_roleManager);
            roles.ForEach(item => {
                if (item.Text != role)
                {
                    rolesLista.Add(new SelectListItem
                    {
                        Text = item.Text
                    });
                }
            });
            return rolesLista;
        }
        private async Task<bool> UpdateAsync()
        {
            var valor = false;
            byte[] imageByte = null;
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var identityUser = _userManager.Users.Where(u => u.Id.Equals(_dataUser2.ID)).ToList().Last();
                        identityUser.UserName = Input.email;
                        identityUser.Email = Input.email;
                        identityUser.PhoneNumber = Input.phoneNumber;
                        _context.Update(identityUser);
                        await _context.SaveChangesAsync();

                        if (Input.AvatarImage == null)
                        {
                            imageByte = _dataUser2.Image;
                        }
                        else
                        {
                            imageByte = await _uploadimage.ByteAvatarImageAsync(Input.AvatarImage, _environment, "");
                        }
                        var t_user = new TUsers
                        {
                            id = _dataUser2.Id,
                            name = Input.name,
                            lastname = Input.lastNames,
                            nid = Input.identification,
                            phonenumber = Input.phoneNumber,

                            email = Input.email,
                            iduser = _dataUser2.ID,
                            image = imageByte,
                        };
                        _context.Update(t_user);
                        _context.SaveChanges();
                        if (_dataUser2.role != Input.role)
                        {
                            await _userManager.RemoveFromRoleAsync(identityUser, _dataUser2.role);
                            await _userManager.AddToRoleAsync(identityUser, Input.role);
                        }
                        transaction.Commit();

                        valor = true;
                    }
                    catch (Exception ex)
                    {
                        _dataInput.ErrorMessage = ex.Message;
                        transaction.Rollback();
                        valor = false;
                    }
                }
            });
            
            return valor;
        }

    }
}

