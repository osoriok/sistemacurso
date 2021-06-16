using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCurso.Areas.Users.Models;
using SistemaCurso.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCurso.Library
{
    public class LUser : ListObject
    {

        public LUser(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _usersRole = new LUsersRoles();
        }
        
        public async Task<List<InputModelRegister>> getTUsuariosAsync(String valor, int id)
        {
            List<TUsers> listUser;
            List<SelectListItem> _listRoles;
            List<InputModelRegister> userList = new List<InputModelRegister>();
            if (valor == null && id.Equals(0))
            {
                listUser = _context.TUsers.ToList();
            }
            else
            {
                if (id.Equals(0))
                {
                    listUser = _context.TUsers.Where(u => u.nid.StartsWith(valor) || u.name.StartsWith(valor) ||
                    u.lastname.StartsWith(valor) || u.email.StartsWith(valor)).ToList();
                }
                else
                {
                    listUser = _context.TUsers.Where(u => u.id.Equals(id)).ToList();
                }
            }
            
            if (!listUser.Count.Equals(0))
            {
                foreach (var item in listUser)
                {
                    _listRoles = await _usersRole.getRole(_userManager, _roleManager, item.iduser);
                    var user = _context.Users.Where(u => u.Id.Equals(item.iduser)).ToList().Last();
                    userList.Add(new InputModelRegister
                    {
                        identification = item.iduser,
                        Id = item.id,
                        ID = item.iduser,
                        name = item.name,
                        lastNames = item.lastname,
                        phoneNumber = item.phonenumber,

                        email = item.email,
                        role = _listRoles[0].Text,
                        Image = item.image,
                        IdentityUser = user
                    });
                    _listRoles.Clear();
                }
            }
            return userList;
        }
        }

    }
