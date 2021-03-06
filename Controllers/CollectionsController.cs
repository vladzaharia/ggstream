﻿using System.Linq;
using System.Threading.Tasks;
using GGStream.Data;
using GGStream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GGStream.Controllers
{
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly Context _context;

        public CollectionsController(Context context)
        {
            _context = context;
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collection.ToListAsync());
        }


        [Route("/admin/{url}")]
        public async Task<IActionResult> Details(string url)
        {
            if (url == null) return NotFound();

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null) return NotFound();

            return View(collection);
        }

        [Route("/admin/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/create")]
        public async Task<IActionResult> Create([Bind("URL,Name,Icon,BaseColor,Private,InstructionType,CallLink")]
            Collection collection)
        {
            if (collection.BaseColor == "#000000") collection.BaseColor = null;

            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Collections");
            }

            return View(collection);
        }

        [Route("/admin/{url}/edit")]
        public async Task<IActionResult> Edit(string url)
        {
            if (url == null) return NotFound();

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null) return NotFound();
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/{url}/edit")]
        public async Task<IActionResult> Edit(string url,
            [Bind("URL,Name,Icon,BaseColor,Private,InstructionType,CallLink")]
            Collection collection)
        {
            if (url != collection.URL) return NotFound();

            if (collection.BaseColor == "#000000") collection.BaseColor = null;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.URL))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(collection);
        }

        [Route("/admin/{url}/delete")]
        public async Task<IActionResult> Delete(string url)
        {
            if (url == null) return NotFound();

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null) return NotFound();

            return View(collection);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/admin/{url}/delete")]
        public async Task<IActionResult> DeleteConfirmed(string url)
        {
            var collection = await _context.Collection.FindAsync(url);
            _context.Collection.Remove(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(string url)
        {
            return _context.Collection.Any(e => e.URL == url);
        }
    }
}