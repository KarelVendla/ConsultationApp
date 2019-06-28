using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ConsultationApp.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ConsultationApp.Controllers
{
    public class ConsultationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: Consultations
        public ViewResult Index(string SortBy, string ByDay, string ByDifficulty)
        {
            string[] SortByDays = new string[] { "","Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

            string[] SortByDifficulty = new string[] { "", "Exam", "Test", "PopQuiz" };

            SelectList days = new SelectList(SortByDays);
            ViewBag.Days = days;

            SelectList difficulties = new SelectList(SortByDifficulty);
            ViewBag.Difficulties = difficulties;


            var sortconsultations = from t in db.Consultations
                           select t;

            if (!String.IsNullOrEmpty(SortBy))
            {
               return View(sortconsultations.Where(s => s.Teacher.Contains(SortBy) || 
               s.Subject.Contains(SortBy) || 
               s.Title.Contains(SortBy)).ToList());
            }
            if (!String.IsNullOrEmpty(ByDay))
            {
                Days d = (Days)Enum.Parse(typeof(Days), ByDay);

                return View(sortconsultations.Where(s => s.Day == d).ToList());
            }
            if (!String.IsNullOrEmpty(ByDifficulty))
            {
                Difficulties d = (Difficulties)Enum.Parse(typeof(Difficulties), ByDifficulty);

                return View(sortconsultations.Where(s => s.Difficulty == d).ToList());
            }

            return View(db.Consultations.ToList());
        }

        public ViewResult CConsultations()
        {
            var user = User.Identity.Name;
            return View(db.Consultations.Where(u => u.TUsername == user).ToList());
        }

        public ActionResult SConsultations()
        {
            var user = User.Identity.Name;
            return View(db.Consultations.Where(u => u.SUsername == user).ToList());
        }

        [HttpPost]
        public ActionResult SCon(int? id,string username)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                consultation.SUsername = username;
            }

            db.Entry(consultation).State = EntityState.Modified;
            db.SaveChanges();
            return View("SConsultations");
        }

        //[Authorize(Users = "Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult New_Role()
        {
            return View();
        }

        [HttpPost]
        //New View for adding roles
        //[Authorize(Users = "Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult New_Role(FormCollection collection)
        {
            try
            {
                db.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                db.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("ManageRoles");
            }
            catch
            {
                return View();
            }
        }


        //Gets roles and prepopulate list for DropdownList
        //[Authorize(Users = "Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult ManageUserRoles()
        {

            var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => 
            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();


            ViewBag.Roles = list;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult RegisterUser(int? id)
        //{
        //    var user = db.Users.Select(u => u.UserName);
        //    var UserRegister = db.Consultations.Where(b => b.IsRegistered == true);

        //    return RedirectToAction("Index");
        //}


        //Method for binding a role with user
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Users = "Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult AddRoleToUser(string UserName, string RoleName)
        {
            //Access UserManager object property AddToRole, passing in UserId from ApplicationUser and RoleName coming from the form
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            //Get user from db.Users 
            ApplicationUser user = db.Users.Where(u => u.UserName.Equals
            (UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (!manager.IsInRole(user.Id, RoleName))
            {
                manager.AddToRole(user.Id, RoleName);
                ViewBag.ResultMessage = "Role added to user!";
            }
            //List roles to add to user
            var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => 
            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.Roles = list;

            return View("ManageUserRoles");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Users = "Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult GetRoles(string UserName)
        {

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                //Get object using UserName
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals
                (UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();


                var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                //Uses AccountContoller object for getting roles
                ViewBag.RolesForThisUser = manager.GetRoles(user.Id);

                //Populates DropDownList with roles bound to user
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Users = "Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteRoleFromUser(string UserName, string RoleName)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (manager.IsInRole(user.Id, RoleName))
            {
                manager.RemoveFromRole(user.Id, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            //Populates DropDownList with roles that are reomovable from user
            var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }
        //Displays all roles in a list
        [Authorize(Roles = "Admin")]
        public ActionResult ManageRoles()
        {
            var roles = db.Roles.ToList();
            return View(roles);
        }

        //Method for deleting role from database
        public ActionResult DeleteRole(string RoleName)
        {
            var thisRole = db.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            db.Roles.Remove(thisRole);
            db.SaveChanges();
            return RedirectToAction("Admin_Panel");
        }

        //Displays username/email/role in a 
        [Authorize(Roles = "Admin")]
        public ActionResult Admin_Panel()
        {

            var UserRoles = db.Roles.ToList();
            SelectList UserRolesList = new SelectList(UserRoles);

            var Users = db.Users.ToList();

            return View(Users);
        }

        // GET: Consultations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }

        // GET: Consultations/Create
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Consultations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create([Bind(Include = "Id,Teacher,Subject,Difficulty,Title,Time,Day")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                consultation.TUsername = User.Identity.Name;
                db.Consultations.Add(consultation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(consultation);
        }

        // GET: Consultations/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }

        // POST: Consultations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit([Bind(Include = "Id,Teacher,Subject,Difficulty,Title,Time,Day")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consultation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consultation);
        }

        // GET: Consultations/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }
        // POST: Consultations/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consultation consultation = db.Consultations.Find(id);
            db.Consultations.Remove(consultation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
