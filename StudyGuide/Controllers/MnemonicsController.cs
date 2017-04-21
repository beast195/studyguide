using StudyGuide.Logic.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyGuide.Controllers
{
    public class MnemonicsController : Controller
    {

        private readonly IMnemonicService _mnemonicService;

        public MnemonicsController(IMnemonicService mnemonicService)
        {
            _mnemonicService = mnemonicService;
        }

        // GET: Mnemonics
        public ActionResult Index()
        {
            _mnemonicService.GetMnemonic("a,c");
            return View();
            
        }

        // GET: Mnemonics/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Mnemonics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mnemonics/Create
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

        // GET: Mnemonics/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Mnemonics/Edit/5
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

        // GET: Mnemonics/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Mnemonics/Delete/5
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
