using AleedTiendaShopping.Common;
using AleedTiendaShopping.Data;
using AleedTiendaShopping.Data.Entities;
using AleedTiendaShopping.Enums;
using AleedTiendaShopping.Helpers;
using AleedTiendaShopping.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Vereyon.Web;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AleedTiendaShopping.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;


        public AccountController(IUserHelper userHelper, DataContext context, ICombosHelper combosHelper, IBlobHelper blobHelper,
            IMailHelper mailHelper, IFlashMessage flashMessage )
        {
            _userHelper = userHelper;
            _context = context;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    _flashMessage.Danger(string.Empty, "Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.");
                }
                else if (result.IsNotAllowed)
                {
                    _flashMessage.Danger(string.Empty, "El usuario no ha sido habilitado, debes de seguir las instrucciones del correo enviado para poder habilitar el usuario.");
                }

                else
                {
                    _flashMessage.Danger(string.Empty, "Email o contraseña incorrectos.");
                }


              
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult NotAuthorized()
        {
            return View();
        }


        public async Task<IActionResult> Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id = Guid.Empty.ToString(),
                Countries = await _combosHelper.GetComboCountriesAsync(),
                States = await _combosHelper.GetComboStatesAsync(0),
                Cities = await _combosHelper.GetComboCitiesAsync(0),
                UserType = UserType.User,
                ImageFullPath = "https://localhost:7116/images/noimage.png",
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {

            if (ModelState.IsValid)
            {


                //validacion de imagenes
                string filename = "";
                if (model.ImageFile == null)
                {

                    filename = "noexiste.png";
                }
                else
                {
                    filename = model.ImageFile.FileName;
                }


                if (!Regex.IsMatch(filename.ToLower(), @"^.*\.(jpg|gif|png|jpeg)$"))
                {
                    _flashMessage.Danger("La imagen debe ser tipo .jpg .gift .png .jpeg");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);

                }

                //cedula no repetida
                User usertwo = await _context.Users
           .FirstOrDefaultAsync(x => x.Document == model.Document);
                if (usertwo != null)
                {
                    _flashMessage.Danger("Documento de identidad ya esta registrado");

                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }

                //proceso correo y creacion

                //almacenamiento azure


                //if (model.ImageFile != null)
                //{
                //    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                //}

                //model.ImageId = imageId;

                //proceso guardado img y de user
                string ruta = "";
                string path = "";
                string pic = "";
                if (model.ImageFile != null)
                {

                    pic = Path.GetFileName(model.Document.ToString() + ".png");

                    path = Path.Combine("wwwroot\\images\\users", pic);
                    ruta = "https://localhost:7116/images/users/" + pic;

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                        model.ImageFullPath = ruta;

                    }


                }


                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    _flashMessage.Danger("Este correo ya está siendo usado.");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }


                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "Shopping - Confirmación de Email",
                    $"<h1>Shopping - Confirmación de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                        $"<hr></br></br><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    _flashMessage.Info("Usuario registrado. Para poder ingresar al sistema, siga las instrucciones que han sido enviadas a su correo.");
                    return View(model);
                }

                _flashMessage.Danger(string.Empty, response.Message);
            



        }
        model.Countries = await _combosHelper.GetComboCountriesAsync();
            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
            return View(model);
        }

       

        public JsonResult GetStates(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int stateId)
        {
            State state = _context.States
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }


        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

           

                EditUserViewModel model = new()
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageFullPath = user.ImageFullPath,
                Cities = await _combosHelper.GetComboCitiesAsync(user.City.State.Id),
                CityId = user.City.Id,
                Countries = await _combosHelper.GetComboCountriesAsync(),
                CountryId = user.City.State.Country.Id,
                StateId = user.City.State.Id,
                States = await _combosHelper.GetComboStatesAsync(user.City.State.Country.Id),
                Id = user.Id,
                Document = user.Document
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //guarda imagen en azure
                //Guid imageId = model.ImageId;

                //if (model.ImageFile != null)
                //{
                //    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                //}

                //validacion de imagenes
                string filename = "";
                if (model.ImageFile == null)
                {

                    filename = "noexiste.png";
                }
                else
                {
                    filename = model.ImageFile.FileName;
                }


                if (!Regex.IsMatch(filename.ToLower(), @"^.*\.(jpg|gif|png|jpeg)$"))
                {
                    _flashMessage.Danger("la imagen debe ser tipo .jpg .gift .png .jpeg");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);

                }

                //cedula no repetida
                User usercc = await _userHelper.GetUserAsync(User.Identity.Name);

              
                if (usercc.Document != model.Document)
                {
                    User usertwo = await _context.Users
                   .FirstOrDefaultAsync(x => x.Document == model.Document);
                    if (usertwo != null)
                    {
                        _flashMessage.Danger("Documento de identidad ya esta registrado");
                        model.Countries = await _combosHelper.GetComboCountriesAsync();
                        model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                        model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                        return View(model);
                    }
                  
                }

                //guarda imagen
                string ruta = model.ImageFullPath;
                string path = "";
                string pic = "";
                if (model.ImageFile != null)
                {

                    pic = Path.GetFileName(model.Document.ToString() + ".png");

                    path = Path.Combine("wwwroot\\images\\users", pic);
                    ruta = "https://localhost:7116/images/users/" + pic;

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                        model.ImageFullPath = ruta;

                    }


                }
                //Actualiza
                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImageFullPath = ruta;
                user.City = await _context.Cities.FindAsync(model.CityId);
                user.Document = model.Document;

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            model.Countries = await _combosHelper.GetComboCountriesAsync();
            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.OldPassword == model.NewPassword)
                {
                    _flashMessage.Danger("Debes ingresar una contraseña diferente.");

                    return View(model);
                }
                User? user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    IdentityResult? result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        _flashMessage.Danger("Valide que los datos ingresados son correctos.");

                    }
                }
                else
                {
                    _flashMessage.Danger("Usuario no encontrado.");

                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    _flashMessage.Danger("El email no corresponde a ningún usuario registrado.");

                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(
                    $"{user.FullName}",
                    model.Email,
                    "Shopping - Recuperación de Contraseña",
                    $"<h1>Shopping - Recuperación de Contraseña</h1>" +
                    $"Para recuperar la contraseña haga click en el siguiente enlace:" +
                    $"<p><a href = \"{link}\">Reset Password</a></p>");
                _flashMessage.Info("Las instrucciones para recuperar la contraseña han sido enviadas a su correo.");

              
                return View();
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    _flashMessage.Info("Contraseña cambiada con éxito.");

                    return View();
                }

               
                _flashMessage.Info("Error cambiando la contraseña.");

                return View(model);
            }

         
            _flashMessage.Info("Usuario no encontrado.");

            return View(model);
        }

        public IActionResult ResendToken()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendToken(ResendTokenViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Username);
                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "Shopping - Confirmación de Email",
                    $"<h1>Shopping - Confirmación de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                        $"<p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    _flashMessage.Info("Email Re-Envíado. Para poder ingresar al sistema, siga las instrucciones que han sido enviadas a su correo.");
                    return RedirectToAction(nameof(Login));
                }

                _flashMessage.Danger(response.Message);
            }
            return View(model);
        }

       
    }
}
