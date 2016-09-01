using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mvc.Models;
using Microsoft.AspNet.Identity;

namespace mvc.Controllers
{
    public class UserInfoController : Controller
    {
        private Entities db = new Entities();

        // GET: UserInfo
        public ActionResult Index()
        {
            //var users = (from user in db.AspNetUsers
            //             join addr in db.UserAddresses 
            //             on user.UserName equals addr.UserName into ua
            //             from userAddr in ua.DefaultIfEmpty()
            return View(db.UserAddresses.ToList());
        }
        
    
        // GET: UserInfo/Edit/5
        public ActionResult Edit()
        {
            string userName = User.Identity.GetUserName();

            UserAddress userAddress = db.UserAddresses.Where(z => z.UserName == userName).FirstOrDefault(); 

            if (userAddress == null)
            {
                return View(new UserAddress(userName));
            }
            return View(userAddress);
        }

        // POST: UserInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Address,City,State,Zip,UserName")] UserAddress userAddress)
        {
            string userName = User.Identity.GetUserName();
            userAddress.UserName = userName;

            if (ModelState.IsValid)
            {
                if (userAddress.Id == 0)
                {
                    db.UserAddresses.Add(userAddress);
                }
                else
                {
                    db.Entry(userAddress).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userAddress);
        }

        // GET: UserInfo/Delete/5
        public ActionResult Delete(int? id)
        {

            string userName = User.Identity.GetUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }

            if (userName == userAddress.UserName)
            {
                TempData["error"] = "You cannot delete yourself";
                return RedirectToAction("Index");
            }

            return View(userAddress);
        }

        // POST: UserInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserAddress userAddress = db.UserAddresses.Find(id);
            db.UserAddresses.Remove(userAddress);
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
