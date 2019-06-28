using ConsultationApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ConsultationApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();

            var db = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);


            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            //If db is empty
            if (!db.Consultations.Any())
            {
                //Create new consultation data
                var cone = new Consultation { Teacher = "Kristjan Kivikangur", Subject = "Võrgurakendused", Difficulty = Difficulties.Test, Title = "Wordpress", Time = DateTime.Now, Day = Days.Friday };
                var ctwo = new Consultation { Teacher = "Margit Uiboaid", Subject = "Inglise keel", Difficulty = Difficulties.PopQuiz, Title = "Grammar", Time = DateTime.Now, Day = Days.Monday };
                var cthree = new Consultation { Teacher = "Karel Vendla", Subject = "Elu", Difficulty = Difficulties.Exam, Title = "Suvi", Time = DateTime.Now, Day = Days.Thursday };
                var cfour = new Consultation { Teacher = "Silva Kiveste", Subject = "Ajalugu", Difficulty = Difficulties.Test, Title = "Teine maailmasõda", Time = DateTime.Now, Day = Days.Tuesday };
                var cfive = new Consultation { Teacher = "Lembit Edu", Subject = "Multimeedia", Difficulty = Difficulties.PopQuiz, Title = "Blender ", Time = DateTime.Now, Day = Days.Wednesday };
                var csix = new Consultation { Teacher = "Kristjan Kivikangur", Subject = "Agiilsed tarkvaraarenduse metoodikad", Difficulty = Difficulties.Test, Title = "Taesemetöö", Time = DateTime.Now, Day = Days.Monday };
                var cseven = new Consultation { Teacher = "Lury Shkarbanova", Subject = "Andmebaaside alused", Difficulty = Difficulties.Test, Title = "Aviafirma db", Time = DateTime.Now, Day = Days.Thursday };
                var ceight = new Consultation { Teacher = "Helen Nagel", Subject = "Mobiilirakendused", Difficulty = Difficulties.Test, Title = "TicTacToe", Time = DateTime.Now, Day = Days.Friday };
                var cnine = new Consultation { Teacher = "Irina Peetrim'gi", Subject = "Vene keel", Difficulty = Difficulties.Test, Title = "Kokkuvõtlik töö", Time = DateTime.Now, Day = Days.Tuesday };
                var cten = new Consultation { Teacher = "Tatjana Smagol", Subject = "Eesti keel", Difficulty = Difficulties.Test, Title = "Kirjavahemärgid", Time = DateTime.Now, Day = Days.Wednesday };


                db.Consultations.Add(cone);
                db.Consultations.Add(ctwo);
                db.Consultations.Add(cthree);
                db.Consultations.Add(cfour);
                db.Consultations.Add(cfive);
                db.Consultations.Add(csix);
                db.Consultations.Add(cseven);
                db.Consultations.Add(ceight);
                db.Consultations.Add(cnine);
                db.Consultations.Add(cten);
            }


            //Find user SuperAdmin
            var SuperUserExists = db.Users.Any(u => u.UserName == "SuperAdmin");

            //Create superadmin data
            var user = new ApplicationUser() { UserName = "SuperAdmin", Email = "superadmin@admin.com" };

            //If SuperUser not found in db
            if (!SuperUserExists)
            {

                //Submit data to database
                userManager.Create(user, "SuperAdmin123!");
            }

            //Find role admin
            var AdminRoleExists = db.Roles.Any(r => r.Name == "Admin");

            //If Role admin doesn't exist
            if (!AdminRoleExists)
            {
                //Create role Admin
                roleManager.Create(new IdentityRole("Admin"));
                roleManager.Create(new IdentityRole("Teacher"));

                //Add Role to database
                userManager.AddToRole(user.Id, "Admin");
            }
            //Save changes to database
            db.SaveChanges();



            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
