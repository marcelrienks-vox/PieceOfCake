﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Scrummage.DataAccess;
using Scrummage.Interfaces;
using Scrummage.Models;

//todo: fix save, and delete methods in Member controller when called from views
//todo: update Members controller (link to Avatar controller)
//todo: create Unit tests for Members controller (link to Avatar controller)
namespace Scrummage.Controllers {
	public class MemberController : Controller {

		#region Properties
		private readonly IUnitOfWork _unitOfWork;
		#endregion

		public MemberController() {
			_unitOfWork = new UnitOfWork();
		}

		public MemberController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		#region Actions
		// GET: /Member/
		public ActionResult Index() {
			return View(_unitOfWork.MemberRepository.All());
		}

		// GET: /Member/Details/5
		public ActionResult Details(int id = 0) {
			var member = _unitOfWork.MemberRepository.Find(id);
			if (member == null) {
				return HttpNotFound();
			}
			return View(member);
		}

		// GET: /Member/Create
		public ActionResult Create() {
			ViewBag.Roles = _unitOfWork.RoleRepository.All();
			return View();
		}

		// POST: /Member/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Member member, HttpPostedFileBase file, FormCollection formCollection) {
			//If file was uploaded read bytes, else read bytes from default avatar
			#region Avatar
			byte[] bytes;
			if (file != null && file.ContentLength > 0) {
				bytes = new byte[file.ContentLength];
				file.InputStream.Read(bytes, 0, file.ContentLength);

			} else {
				bytes = System.IO.File.ReadAllBytes(ControllerContext.HttpContext.Server.MapPath(@"~\Images\default_avatar.jpg"));
			}

			//Create new Avatar model
			member.Avatar = new Avatar {
				Image = bytes
			};

			//clear avatar model state error (as avatar was added above)
			if (ModelState["Avatar"] != null && ModelState["Avatar"].Errors.Count == 1 && ModelState["Avatar"].Errors[0].ErrorMessage == "The Avatar field is required.") {
				ModelState["Avatar"].Errors.Clear();
			}
			#endregion

			#region Roles
			//Todo: fix this code to 'find roles where in' to run through the repository
			var rolesTitles = formCollection["roleSelect"].Split(',');

			//var matches = from person in people 
			//	where names.Contains(person.Firstname) 
			//	select person;

			var roles = (from role in new Context().Roles
			             where rolesTitles.Contains(role.Title)
			             select role).ToList();
			#endregion

			//create model
			if (ModelState.IsValid) {
				_unitOfWork.MemberRepository.Create(member);
				_unitOfWork.MemberRepository.Save();
				return RedirectToAction("Index");
			}

			return View(member);
		}

		// GET: /Member/Edit/5
		public ActionResult Edit(int id = 0) {
			var member = _unitOfWork.MemberRepository.Find(id);
			if (member == null) {
				return HttpNotFound();
			}
			return View(member);
		}

		// POST: /Member/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Member member) {
			if (ModelState.IsValid) {
				_unitOfWork.MemberRepository.Update(member);
				_unitOfWork.MemberRepository.Save();
				return RedirectToAction("Index");
			}
			return View(member);
		}

		// GET: /Member/Delete/5
		public ActionResult Delete(int id = 0) {
			var member = _unitOfWork.MemberRepository.Find(id);
			if (member == null) {
				return HttpNotFound();
			}
			return View(member);
		}

		// POST: /Member/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			_unitOfWork.MemberRepository.Delete(id);
			_unitOfWork.MemberRepository.Save();
			return RedirectToAction("Index");
		}
		#endregion

		protected override void Dispose(bool disposing) {
			_unitOfWork.RoleRepository.Dispose();
			base.Dispose(disposing);
		}
	}
}