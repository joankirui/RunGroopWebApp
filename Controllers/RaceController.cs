﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races =  await _raceRepository.GetAll();
            return View(races);
        }
        public async  Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetById(id);
            return View(race);
        }

        public IActionResult Create()
        {
            var curUserID = _httpContextAccessor.HttpContext?.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserID };
            return View(createRaceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = raceVM.AppUserId,
                    Address = new Address
                    {
                        City = raceVM.Address.City,
                        State = raceVM.Address.State,
                        Street = raceVM.Address.Street
                    }
                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }

            return View(raceVM);
        }
        //GET
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetById(id);
            if (race == null) return View("Error");
            var raceVM = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory
            };
            return View(raceVM);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race");
                return View("Edit", raceVM);
            }

            var userRace = await _raceRepository.GetByIdNoTracking(id);

            if (userRace != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userRace.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(raceVM);
                }

                var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

                var race = new Race()
                {
                    Id = id,
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = raceVM.AddressId,
                    Address = raceVM.Address,

                };

                _raceRepository.Update(race);

                return RedirectToAction("Index");

            }
            return View(raceVM);

        }

        public async Task<IActionResult> Delete(int id)
        {
            var raceDetails = await _raceRepository.GetById(id);
            if (raceDetails == null) return View("Error");
            return View(raceDetails);

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var raceDetails = await _raceRepository.GetById(id);
            if (raceDetails == null)
            {
                return View("Error");
            }

            _raceRepository.Delete(raceDetails);
            return RedirectToAction("Index");
        }
    }
}
