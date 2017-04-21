using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyGuide.Controllers
{
    public class StoryTimeController : Controller
    {
        // GET: StoryTime
        public ActionResult Index()
        {
            return View();
        }

        // GET: StoryTime/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StoryTime/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoryTime/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: StoryTime/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StoryTime/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: StoryTime/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StoryTime/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
