using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AleedTiendaShopping.Data;
using AleedTiendaShopping.Data.Entities;
using AleedTiendaShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Vereyon.Web;

namespace AleedTiendaShopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public CountriesController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries
                .Include(c=> c.States)
                .ToListAsync());


        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null )
            {
                return NotFound();
            }

           Country country = await _context.Countries
                .Include(c=>c.States)
                .ThenInclude(s=> s.Cities)
                .FirstOrDefaultAsync(c=> c.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

       
        public IActionResult Create()
        {
            Country country = new() { States = new List<State>() };

            return View(country);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger( "Ya existe un país con el mismo nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                    }
                    
                }
                catch(Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }
                
            }
            return View(country);
        }

   

     
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries
                .Include(c=> c.States)
                .FirstOrDefaultAsync(c=> c.Id==id);
            if (country == null)
            {
                return NotFound();
            }

            
            return View(country);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un país con el mismo nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                    }

                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }
                
            }
            return View(country);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            Country country = await _context.Countries
                .Include(c=>c.States)
                .FirstOrDefaultAsync(c=> c.Id==id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
            Country country = await _context.Countries.FindAsync(id);
                _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro Borrado");
            return RedirectToAction(nameof(Index));
        }


        // METODOS PARA LOS ESTADOS //

        public async Task<IActionResult> AddState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FindAsync(id);
            
            if (country == null)
            {
                return NotFound();
            }

            StateViewModel model = new()
            {
                CountryId = country.Id,
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> AddState(StateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Cities = new List<City>(),
                        Country = await _context.Countries.FindAsync(model.CountryId),
                        Name=model.Name,
                    };

                    _context.Add(state);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {Id= model.CountryId});
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un Departamento/Estado con el mismo nombre en este país. ");
                    }
                    else
                    {
                        _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                    }

                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }

            }
            return View(model);
        }


        public async Task<IActionResult> EditState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.States
                .Include(s=>s.Country)
                .FirstOrDefaultAsync(s=>s.Id==id);
            if (state == null)
            {
                return NotFound();
            }

            StateViewModel model = new()
            {
                CountryId = state.Country.Id,
                Id = state.Id,
                Name=state.Name,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, StateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };
                    _context.Update(state);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {Id = model.CountryId});
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un Departamento/Estado con el mismo nombre en este país");
                    }
                    else
                    {
                        _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                    }

                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> DetailsState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.States
                 .Include(s => s.Country)
                 .Include(s => s.Cities)
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        public async Task<IActionResult> DeleteState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s=> s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }


        [HttpPost, ActionName("DeleteState")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStateConfirmed(int id)
        {

            State state = await _context.States
                .Include(s=> s.Country)
                .FirstOrDefaultAsync(s=>s.Id==id);
            _context.States.Remove(state);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro Borrado");
            return RedirectToAction(nameof(Details), new {Id=state.Country.Id} );
        }
       
        // METODOS PARA LAS CIUDADES //

        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.States.FindAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            CityViewModel model = new()
            {
                StateId = state.Id,
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        
                        State = await _context.States.FindAsync(model.StateId),
                        Name = model.Name,
                    };

                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState), new { Id = model.StateId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe una ciudad con el mismo nombre en este Departamento/Estado. ");
                    }
                    else
                    {
                        _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                    }

                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }

            }
            return View(model);
        }


        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            City city = await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            CityViewModel model = new()
            {
                StateId = city.State.Id,
                Id = city.Id,
                Name = city.Name,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, CityViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        Id = model.Id,
                        Name = model.Name,  
                       
                    };
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState), new { Id = model.StateId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger( "Ya existe una Ciudad con el mismo nombre en este departamento/estado");
                    }
                    else
                    {
                        _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                    }

                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> DetailsCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            City city = await _context.Cities
                 .Include(s => s.State)
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            City city = await _context.Cities
                .Include(s => s.State)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }


        [HttpPost, ActionName("DeleteCity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCityConfirmed(int id)
        {

            City city = await _context.Cities
               .Include(s => s.State)
               .FirstOrDefaultAsync(s => s.Id == id);
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro Borrado");
            return RedirectToAction(nameof(DetailsState), new { Id = city.State.Id });
        }

    }

}
