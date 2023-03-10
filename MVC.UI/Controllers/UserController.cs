using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVC.UI.Models;
using Web.Dta;
using Web.Dto;
using Web.Logic;

namespace MVC.UI.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            List<tbl_user> list = tbl_userItem.GetAll();
            return View(list);
        }

        // GET: User/Details/5
        public ActionResult Details(string Username)
        {
            tbl_user existing = tbl_userItem.GetByPK(Username);
            return View(existing);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_user item)
        {
            // TODO: Add insert logic here
            tbl_user existing = tbl_userItem.GetByPK(item.Username);
            if (existing != null)
            {
                ModelState.AddModelError("", "Username sudah ada.");
                return View(item);
            }
            else
            {
                item.Password = Web.Logic.Security.MD5Hash(item.Password);
                tbl_userItem.Insert(item);
                return RedirectToAction("Index");
            }
        }

        //// POST: User/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: User/Edit/5
        public ActionResult Edit(string Username)
        {
            tbl_user existing = tbl_userItem.GetByPK(Username);
            return View(existing);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(string Username, tbl_user item)
        {
            try
            {
                tbl_user existing = tbl_userItem.GetByPK(item.Username);
                if (existing == null)
                {
                    ModelState.AddModelError("", "Data tidak ditemukan");
                    return View(item);
                }
                if (!string.IsNullOrEmpty(item.Password))
                    existing.Password = Web.Logic.Security.MD5Hash(item.Password);
                existing.FullName = item.FullName;
                existing.edited = DateTime.Now;
                existing.editor = Utilities.Username;
                existing.MachineName = Utilities.GetComputerName();
                existing.IPAddress = Utilities.GetIpAddress();
                existing.IsActive = 0;
                // TODO: Add update logic here
                tbl_user result = tbl_userItem.Update(existing);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(item);
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(string Username)
        {
            tbl_user existing = tbl_userItem.GetByPK(Username);
            if (existing != null)
            {
                tbl_userItem.Delete(existing.Username);
            }
            return RedirectToAction("Index");
        }

        // POST: User/Delete/5
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                //sementara
                string result = string.Empty;
                try
                {
                    tbl_user item = tbl_userItem.GetByPK(login.UserName);
                    if (item != null && item.Password == Web.Logic.Security.MD5Hash(login.Password))
                    {
                        //FormsAuthentication.SetAuthCookie(item.Username, true);
                        Session["Username"] = item.Username;
                        item.IsLogin = 1;
                        item.LastLogin = DateTime.Now;
                        item.IPAddress = Utilities.GetIpAddress();
                        item.MachineName = Utilities.GetComputerName();
                        tbl_userItem.Update(item);
                        Log.Info(string.Format("User :{0} logged in", item.Username));

                        string test = Utilities.Username;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login gagal. Periksa Nama Pengguna dan Kata Sandi Anda.");
                    }


                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Nama pengguna dan kata sandi salah");
                    return View(login);
                }
            }
            return View(login);
        }

        public ActionResult Logout()
        {
            //FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session["Username"] = null;
            return RedirectToAction("Login");
        }
    }
}
