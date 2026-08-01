using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Models;
using ClinicManagement.Data;
using Microsoft.AspNetCore.Authorization;

namespace ClinicManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return View(users);
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();


            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
                return NotFound();


            return View(user);
        }



        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.ToList();

            return View();
        }



        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Username,PasswordHash,FullName,Phone,Email,IsActive,RoleId")] User user)
        {

            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;


                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);


                _context.Users.Add(user);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            ViewBag.Roles = _context.Roles.ToList();

            return View(user);
        }




        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();


            var user = await _context.Users.FindAsync(id);


            if (user == null)
                return NotFound();


            ViewBag.Roles = _context.Roles.ToList();


            return View(user);
        }





        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Username,PasswordHash,FullName,Phone,Email,IsActive,RoleId")] User user)
        {

            if (id != user.Id)
                return NotFound();



            if (ModelState.IsValid)
            {
                try
                {
                    var oldUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.Id == id);


                    if (oldUser == null)
                        return NotFound();



                    oldUser.Username = user.Username;
                    oldUser.FullName = user.FullName;
                    oldUser.Phone = user.Phone;
                    oldUser.Email = user.Email;
                    oldUser.IsActive = user.IsActive;
                    oldUser.RoleId = user.RoleId;


                 
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        oldUser.PasswordHash =
                            BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                    }


                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                        return NotFound();

                    throw;
                }


                return RedirectToAction(nameof(Index));
            }


            ViewBag.Roles = _context.Roles.ToList();

            return View(user);
        }





        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();



            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);



            if (user == null)
                return NotFound();



            return View(user);
        }





        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var user = await _context.Users.FindAsync(id);


            if (user != null)
            {
                _context.Users.Remove(user);
            }


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }





        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}