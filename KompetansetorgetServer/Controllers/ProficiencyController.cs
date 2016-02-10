﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KompetansetorgetServer.ContextDbs;
using KompetansetorgetServer.Models;
using System.Threading.Tasks;


namespace KompetansetorgetServer.Controllers
{
    public class ProficiencyController : Controller
    {
		private KompetanseContext db;

		public ProficiencyController()
		{
			this.db = new KompetanseContext ();
		}

        public ActionResult Index()
        {
            return View ();
        }

        public ActionResult Details(int id)
        {
            return View ();
        }

        public ActionResult Create()
        {
			
            return View ();
        } 

		public ActionResult TestGetStudentProficiency()
		{
			string username = "viktos08";
			GetStudentProficiency(username);
			return RedirectToAction("Index", "Home");
		}	

		//Void kinda defeats the purpose, but its for testing purposes only
		private async void GetStudentProficiency(string username)
		{
			Student student = await GetStudent(username);

			Console.WriteLine (student.IdProficiency);

			Proficiency prof = await db.Proficiencies.FindAsync (student.IdProficiency);
			Console.WriteLine ("Idrettsfag = " + prof.Idrettsfag);
			Console.WriteLine ("Datateknologi = " + prof.Datateknologi);
			Console.WriteLine ("Lærer = " + prof.Lerer );
		}

		private async Task<Student> GetStudent(string username) 
		{
			return await db.Students.FindAsync (username);
		}

//		private async Student getStudent1(string username) 
//		{
//			var student = await db.Students.Where(x => x.Username== username).ToListAsync();
//			return student;
//		}
//			foreach (Student student in studDb.Students) 
//			{
//				if (student.Username.Equals(username))
//				{
//					return student.IdProficiency;
//				}
//			}
//			return null;





        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try {
                return RedirectToAction ("Index");
            } catch {
                return View ();
            }
        }
        
        public ActionResult Edit(int id)
        {
            return View ();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try {
                return RedirectToAction ("Index");
            } catch {
                return View ();
            }
        }

        public ActionResult Delete(int id)
        {
            return View ();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try {
                return RedirectToAction ("Index");
            } catch {
                return View ();
            }
        }
    }
}