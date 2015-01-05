﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PieceOfCake.Data.Models;
using PieceOfCake.Web.Controllers;
using PieceOfCake.Web.Test.DataAccess;
using PieceOfCake.Web.Test.Factories;
using PieceOfCake.Web.Test.Factories.ModelFactories;
using PieceOfCake.Web.ViewModels;

namespace PieceOfCake.Web.Test.Controllers
{
    //[TestClass]
    public class UserControllerTest
    {
        #region Properties

        private readonly FakeUnitOfWork _fakeUnitOfWork;

        #endregion

        public UserControllerTest()
        {
            _fakeUnitOfWork = new FakeUnitOfWork();
            AutoMapperConfig.ConfigureMappings();
        }

        #region Index tests

        [TestMethod]
        public void TestSuccessfulIndexGet()
        {
            var testUsers = new UserFactory().BuildList();
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = testUsers;

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var userViewModels = ((IEnumerable<UserViewModel>)result.Model).ToList();
            Assert.AreEqual(1, userViewModels.Count);
            PerformCommonAsserts(testUsers.First(), userViewModels.First());
        }

        #endregion

        #region Details tests

        [TestMethod]
        public void TestFailedDetailsGet()
        {
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = new UserFactory().BuildList();

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Details(9);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void TestSuccessfulDetailsGet()
        {
            var testUsers = new UserFactory().BuildList();
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = testUsers;

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Details() as ViewResult;
            Assert.IsNotNull(result);

            var userViewModel = (UserViewModel)result.Model;
            PerformCommonAsserts(testUsers.First(), userViewModel);
        }

        #endregion

        #region Create tests

        [TestMethod]
        public void TestSuccessfulCreateGet()
        {
            var testRoles = new RoleFactory().BuildList();
            //'FakeUnitOfWork.RoleRepository' must be cast to 'FakeRepository<Role>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<Role>)_fakeUnitOfWork.RoleRepository).ModelList = testRoles;

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void TestFailedCreatePost()
        {
            var testRoles = new RoleFactory().BuildList();
            //'FakeUnitOfWork.RoleRepository' must be cast to 'FakeRepository<Role>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<Role>)_fakeUnitOfWork.RoleRepository).ModelList = testRoles;

            var customHttpPostedFileBase = HttpPostedFileBaseFactory.CreateCustomHttpPostedFileBase();

            var testUser = new UserFactory().Build();
            var testUserViewModel = Mapper.Map(testUser, new UserViewModel());

            var controller = new UserController(_fakeUnitOfWork);
            controller.ModelState.AddModelError("key", "model is invalid"); //Causes ModelState.IsValid to return false
            var result = controller.Create(testUserViewModel, customHttpPostedFileBase, new FormCollection
            {
                {"roleSelect", new RoleFactory().Build().Title}
            }) as ViewResult;
            Assert.IsNotNull(result);

            var userViewModel = (UserViewModel)result.Model;
            PerformCommonAsserts(testUser, userViewModel);
        }

        [TestMethod]
        public void TestSuccessfulCreatePostWithAvatar()
        {
            var testRoles = new RoleFactory().BuildList();
            //'FakeUnitOfWork.RoleRepository' must be cast to 'FakeRepository<Role>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<Role>)_fakeUnitOfWork.RoleRepository).ModelList = testRoles;

            var customHttpPostedFileBase = HttpPostedFileBaseFactory.CreateCustomHttpPostedFileBase();

            var testUser = new UserFactory().Build();
            var testUserViewModel = Mapper.Map(testUser, new UserViewModel());

            //Convert role titles to comma delimited string
            var roleTitles =
                testRoles.Aggregate(String.Empty, (current, role) => current + role.Title + ", ")
                    .TrimEnd(", ".ToCharArray());

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Create(testUserViewModel, customHttpPostedFileBase, new FormCollection
            {
                {"roleSelect", roleTitles}
            }) as ViewResult;

            Assert.IsNull(result);

            Assert.IsTrue(((FakeRepository<User>)_fakeUnitOfWork.UserRepository).IsCreated);
            Assert.IsTrue(((FakeRepository<User>)_fakeUnitOfWork.UserRepository).IsSaved);
        }

        #endregion

        #region Edit tests

        [TestMethod]
        public void TestFailedEditGet()
        {
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = new UserFactory().BuildList();

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Edit(9);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void TestSuccessfulEditGet()
        {
            var testRoles = new RoleFactory().BuildList();
            //'FakeUnitOfWork.RoleRepository' must be cast to 'FakeRepository<Role>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<Role>)_fakeUnitOfWork.RoleRepository).ModelList = testRoles;

            var testUsers = new UserFactory().BuildList();
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = testUsers;

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Edit() as ViewResult;
            Assert.IsNotNull(result);

            var userViewModel = (UserViewModel)result.Model;
            PerformCommonAsserts(testUsers.First(), userViewModel);
        }

        //[TestMethod]
        //public void TestFailedEditPost()
        //{
        //    var testUser = new UserFactory().Build();
        //    var testUserViewModel = Mapper.Map(testUser, new actual());

        //    var controller = new UserController(_fakeUnitOfWork);
        //    controller.ModelState.AddModelError("key", "model is invalid"); //Causes ModelState.IsValid to return false
        //    var result = controller.Edit(testUserViewModel) as ViewResult;
        //    Assert.IsNotNull(result);

        //    var actual = (actual)result.Model;
        //    Assert.AreSame(testUserViewModel, actual);
        //}

        //[TestMethod]
        //public void TestSuccessfulEditPost()
        //{
        //    var testUser = new UserFactory().Build();
        //    var testUserViewModel = Mapper.Map(testUser, new actual());

        //    var controller = new UserController(_fakeUnitOfWork);
        //    var result = controller.Edit(testUserViewModel);
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        //    Assert.IsTrue(((FakeRepository<expected>)_fakeUnitOfWork.UserRepository).IsUpdated);
        //    Assert.IsTrue(((FakeRepository<expected>)_fakeUnitOfWork.UserRepository).IsSaved);
        //}

        #endregion

        #region Delete tests

        [TestMethod]
        public void TestFailedDeleteGet()
        {
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = new UserFactory().BuildList();

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Delete(9);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void TestSuccessfulDeleteGet()
        {
            var testUsers = new UserFactory().BuildList();
            //'FakeUnitOfWork.UserRepository' must be cast to 'FakeRepository<expected>', as 'FakeRepository' exposes some properties that 'IRepository' does not
            ((FakeRepository<User>)_fakeUnitOfWork.UserRepository).ModelList = testUsers;

            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.Delete() as ViewResult;
            Assert.IsNotNull(result);

            var userViewModel = (UserViewModel)result.Model;
            PerformCommonAsserts(testUsers.First(), userViewModel);
        }

        [TestMethod]
        public void TestSuccessfulDeletePost()
        {
            var controller = new UserController(_fakeUnitOfWork);
            var result = controller.DeleteConfirmed(0);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.IsTrue(((FakeRepository<User>)_fakeUnitOfWork.UserRepository).IsDeleted);
            Assert.IsTrue(((FakeRepository<User>)_fakeUnitOfWork.UserRepository).IsSaved);
        }

        #endregion

        #region Common Asserts

        private static void PerformCommonAsserts(User expected, UserViewModel actual)
        {
            Assert.AreEqual(expected.Email, actual.Email);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.ShortName, actual.ShortName);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.Avatar.Id, actual.AvatarViewModel.Id);
            Assert.AreEqual(expected.Avatar.Image, actual.AvatarViewModel.Image);
            Assert.AreEqual(expected.Roles.Count, actual.RoleViewModels.Count);
            Assert.AreEqual(expected.Roles.First().Id, actual.RoleViewModels.First().Id);
            Assert.AreEqual(expected.Roles.First().Title, actual.RoleViewModels.First().Title);
        }

        #endregion
    }
}