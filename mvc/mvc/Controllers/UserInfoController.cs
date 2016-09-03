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
    [Authorize]
    public class UserInfoController : Controller
    {
        private Entities db = new Entities();

        // GET: UserInfo
        public ActionResult Index()
        {
            //var users = from userVal in db.AspNetUsers
            //             join addr in db.UserAddresses
            //             on userVal.UserName equals addr.UserName into a
            //             from b in a.DefaultIfEmpty(new UserAddress())
            //             select {  };

            var allUsers = (from c in db.AspNetUsers
                    join o in db.UserAddresses on c.UserName equals o.UserName into t
                    from a in t.DefaultIfEmpty()
                    select new
                    {
                        UserName = c.UserName,
                        Id = c.Id,
                        Address = a.Address,
                        City = a.City,
                        State = a.State,
                        Zip = a.Zip
                    }).ToList();
            allUsers.GroupBy(z => z.UserName).Select(u => u.First());
            return View(allUsers.Select(z => new UserAddress(z.UserName, z.Address, z.City, z.State, z.Zip, z.Id)).ToList());
        }
        
    
        // GET: UserInfo/Edit/5
        public ActionResult Edit()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();

            UserAddress userAddress = db.UserAddresses.Where(z => z.UserName == userName).FirstOrDefault();

            if (userAddress == null)
            {
                return View(new UserAddress(userName,string.Empty, string.Empty, string.Empty, string.Empty,userId));
            }
            else { userAddress.userId = userId; }
            return View(userAddress);
        }

        // POST: UserInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Address,City,State,Zip,UserName,UserId")] UserAddress userAddress)
        {

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
        public ActionResult Delete(string userId)
        {
            string userName = User.Identity.GetUserName();

            if (String.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspNetUser us = db.AspNetUsers.Find(userId);

            if (us == null)
            {
                return HttpNotFound();
            }

            if (userName == us.UserName)
            {
                TempData["error"] = "You cannot delete yourself";
                return RedirectToAction("Index");
            }
            
            UserAddress userAddress = db.UserAddresses.Where(z => z.UserName == us.UserName).FirstOrDefault();
            
            if(userAddress == null)
            {
                return View(new UserAddress(us.UserName, string.Empty, string.Empty,string.Empty,string.Empty, userId));
            }
            else
            {
                userAddress.userId = userId;
            }

            return View(userAddress);
        }

        // POST: UserInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string userId)
        {
            AspNetUser us = db.AspNetUsers.Find(userId);
            UserAddress userAddress = db.UserAddresses.Where(z => z.UserName == us.UserName).FirstOrDefault();
            db.AspNetUsers.Remove(us);

            if(userAddress != null)
            {
                db.UserAddresses.Remove(userAddress);
            }

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
