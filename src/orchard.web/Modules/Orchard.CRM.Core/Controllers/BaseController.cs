/// Orchard Collaboration is a plugin for Orchard CMS that provides an integrated ticketing system for it.
/// Copyright (C) 2014-2015  Siyamand Ayubi
///
/// This file is part of Orchard Collaboration.
///
///    Orchard Collaboration is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by
///    the Free Software Foundation, either version 3 of the License, or
///    (at your option) any later version.
///
///    Orchard Collaboration is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with Orchard Collaboration.  If not, see <http://www.gnu.org/licenses/>.

using Orchard.CRM.Core.Services;
using Orchard.CRM.Core.ViewModels;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Localization;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.CRM.Core.Models;
using Orchard.Indexing;

namespace Orchard.CRM.Core.Controllers
{
    public abstract class BaseController : Controller, IUpdateModel
    {
        protected readonly IIndexProvider indexProvider;
        protected readonly IContentManager contentManager;
        protected readonly IThemeAwareViewEngine themeAwareViewEngine;
        protected readonly ICRMContentOwnershipService crmContentOwnershipService;
        protected readonly IDisplayHelperFactory displayHelperFactory;
        protected readonly IWidgetService widgetService;
        protected readonly IPartSerializationManager partSerializationManager;
        protected readonly string ControllerContentType;
        protected readonly string EditMetadataType;
        protected readonly IBasicDataService basicDataService;
        protected readonly IExtendedContentManager extendedContentManager;
        protected readonly ITransactionManager transactionManager;
        protected readonly IOrchardServices services;
        protected readonly IContentOwnershipHelper contentOwnershipHelper;
        protected readonly IActivityStreamService streamService;

        public BaseController(
            string controllerContentType,
            string editMetadataType,
            IIndexProvider indexProvider,
            IOrchardServices services,
            ICRMContentOwnershipService crmContentOwnershipService,
            ITransactionManager transactionManager,
            IExtendedContentManager extendedContentManager,
            IContentManager contentManager,
            IPartSerializationManager partSerializationManager,
            IWidgetService widgetService,
            IThemeAwareViewEngine themeAwareViewEngine,
            IShapeFactory shapeFactory,
            IDisplayHelperFactory displayHelperFactory,
            IBasicDataService basicDataService,
            IContentOwnershipHelper contentOwnershipHelper,
            IActivityStreamService streamService)
        {
            this.streamService = streamService;
            this.indexProvider = indexProvider;
            this.services = services;
            this.basicDataService = basicDataService;
            this.transactionManager = transactionManager;
            this.ControllerContentType = controllerContentType;
            this.EditMetadataType = editMetadataType;
            this.extendedContentManager = extendedContentManager;
            this.contentManager = contentManager;
            this.partSerializationManager = partSerializationManager;
            this.widgetService = widgetService;
            this.themeAwareViewEngine = themeAwareViewEngine;
            this.displayHelperFactory = displayHelperFactory;
            this.Shape = shapeFactory;
            this.crmContentOwnershipService = crmContentOwnershipService;
            this.T = NullLocalizer.Instance;
            this.contentOwnershipHelper = contentOwnershipHelper;
        }

        public dynamic Shape { get; set; }
        private Localizer T { get; set; }

        public virtual ActionResult Display(int id)
        {
            ContentItem contentItem = string.IsNullOrEmpty(this.ControllerContentType) ?
                this.contentManager.Get(id, VersionOptions.Published) :
                this.extendedContentManager.Get(id, this.ControllerContentType);

            if (contentItem == null)
                return HttpNotFound();

            if (!this.IsDisplayAuthorized())
            {
                return new HttpUnauthorizedResult();
            }

            dynamic model = contentManager.BuildDisplay(contentItem);

            this.OnDisplay(contentItem, model);
            return this.CreateActionResultBasedOnAjaxRequest(model, "DisplayAjax");
        }

        public virtual ActionResult Create(string id)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(this.ControllerContentType) && id.CompareTo(this.ControllerContentType) != 0)
            {
                throw new ArgumentOutOfRangeException("the value of input parameter 'id' must be " + this.ControllerContentType);
            }

            var contentItem = this.contentManager.New(id);

            if (!this.IsCreateAuthorized())
                return new HttpUnauthorizedResult();

            this.OnCreating(contentItem);
            dynamic model = this.contentManager.BuildEditor(contentItem);

            if (!string.IsNullOrEmpty(this.EditMetadataType))
            {
                model.Metadata.Type = this.EditMetadataType;
            }

            this.OnCreated(model);

            return this.CreateActionResultBasedOnAjaxRequest(model, "Create");
        }

        public virtual ActionResult Edit(int id, string type)
        {
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(this.ControllerContentType) && type.CompareTo(this.ControllerContentType) != 0)
            {
                throw new ArgumentOutOfRangeException("the value of input parameter 'type' must be " + this.ControllerContentType);
            }

            ContentItem contentItem = string.IsNullOrEmpty(type) ?
                this.contentManager.Get(id) :
                this.extendedContentManager.Get(id, type);

            if (contentItem == null)
                return HttpNotFound();

            if (!this.IsEditAuthorized(contentItem))
                return new HttpUnauthorizedResult();

            dynamic model = this.contentManager.BuildEditor(contentItem);

            if (!string.IsNullOrEmpty(this.EditMetadataType))
            {
                model.Metadata.Type = this.EditMetadataType;
            }

            this.OnEdit(contentItem, model);

            return this.CreateActionResultBasedOnAjaxRequest(model, "Edit");
        }

        [HttpPost]
        public virtual ActionResult CreatePOST(string id, string returnUrl)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(this.ControllerContentType) && id.CompareTo(this.ControllerContentType) != 0)
            {
                throw new ArgumentOutOfRangeException("the value of input parameter 'id' must be " + this.ControllerContentType);
            }

            var contentItem = this.contentManager.New(id);

            if (!this.IsCreateAuthorized())
                return new HttpUnauthorizedResult();

            this.contentManager.Create(contentItem, VersionOptions.Draft);

            // Edit owner can not be set in the ContentItemPermissionDriver, because the other drivers check the permissions and there is no guarantee 
            // that ContentItemPermissionDriver.Editor runs first of all
            if (contentItem.Is<ContentItemPermissionPart>())
            {
                EditOwner(contentItem, true);
            }

            this.OnCreatingPost(contentItem);

            dynamic model = this.contentManager.UpdateEditor(contentItem, this);
            this.OnCreatePost(contentItem);
            if (!ModelState.IsValid)
            {
                this.transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View("Create", (object)model);
            }

            this.contentManager.Publish(contentItem);
            this.streamService.WriteChangesToStreamActivity(contentItem, null);

            var documentIndex = this.indexProvider.New(contentItem.Id);
            this.contentManager.Index(contentItem, documentIndex);
            this.indexProvider.Store(TicketController.SearchIndexName, documentIndex);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                return this.Json(this.CreateAjaxMessageModel(contentItem, string.Empty), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Display", new { id = contentItem.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult EditPOST(int id, string returnUrl)
        {
            var contentItem = this.contentManager.Get(id, VersionOptions.DraftRequired);

            if (contentItem == null)
                return HttpNotFound();

            if (!this.IsEditAuthorized(contentItem))
                return new HttpUnauthorizedResult();

            dynamic snapshot = this.streamService.TakeSnapshot(contentItem);

            // Edit owner can not be set in the ContentItemPermissionDriver, because the other drivers check the permissions and there is no guarantee 
            // that ContentItemPermissionDriver.Editor runs first of all
            if (contentItem.Is<ContentItemPermissionPart>())
            {
                this.EditOwner(contentItem, false);
            }

            dynamic model = this.contentManager.UpdateEditor(contentItem, this);
            this.OnEditPost(contentItem);
            if (!ModelState.IsValid)
            {
                this.transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View("Edit", (object)model);
            }

            this.contentManager.Publish(contentItem);

            var documentIndex = this.indexProvider.New(contentItem.Id);
            this.contentManager.Index(contentItem, documentIndex);
            this.indexProvider.Store(TicketController.SearchIndexName, documentIndex);
            this.streamService.WriteChangesToStreamActivity(contentItem, snapshot);
            this.transactionManager.RequireNew();
            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                return this.Json(this.CreateAjaxMessageModel(contentItem, string.Empty), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.RedirectToAction("Edit", new RouteValueDictionary { { "Id", contentItem.Id } });
            }
        }

        public virtual ActionResult Remove(int[] ids, string returnUrl)
        {
            if (!this.services.Authorizer.Authorize(Permissions.AdvancedOperatorPermission))
                return new HttpUnauthorizedResult();

            ContentItem firstContentItem = null;

            if (ids == null || ids.Length == 0)
            {
                this.ModelState.AddModelError("ids", "At least one contentItemId must be provided");
            }

            var user = this.services.WorkContext.CurrentUser;
            foreach (var id in ids)
            {
                var contentItem = this.contentManager.Get(id, VersionOptions.Latest);

                if (contentItem != null)
                {
                    if (firstContentItem == null)
                    {
                        firstContentItem = contentItem;
                    }

                    this.contentManager.Remove(contentItem);
                    string contentDescription = string.Format("{0} {1}", contentItem.ContentType, CRMHelper.GetContentItemTitle(contentItem));
                    this.streamService.Add(user.Id, contentItem.Id, contentItem.VersionRecord.Id, new[] { "Remove the item" }, contentDescription);
                }
            }

            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                return this.Json(this.CreateAjaxMessageModel(firstContentItem, string.Empty), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Redirect(returnUrl);
            }
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        protected virtual void OnCreated(dynamic model) { }
        protected virtual void OnCreating(ContentItem contentItem) { }
        protected virtual void OnDisplay(ContentItem contentItem, dynamic model) { }
        protected virtual void OnEdit(ContentItem contentItem, dynamic model) { }
        protected virtual void OnEditPost(ContentItem contentItem) { }
        protected virtual void OnCreatePost(ContentItem contentItem) { }
        protected virtual void OnCreatingPost(ContentItem contentItem) { }

        protected abstract bool IsCreateAuthorized();
        protected abstract bool IsDisplayAuthorized();

        protected virtual bool IsEditAuthorized(IContent contentItem)
        {
            return this.crmContentOwnershipService.CurrentUserCanEditContent(contentItem);
        }

        protected ActionResult CreateActionResultBasedOnAjaxRequest(dynamic model, string partialView)
        {
            return this.CreateActionResultBasedOnAjaxRequest(model, partialView, string.Empty);
        }

        protected ActionResult CreateActionResultBasedOnAjaxRequest(dynamic model, string partialView, string mainView)
        {
            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                AjaxMessageViewModel ajaxMessageModel = new AjaxMessageViewModel { IsDone = true };
                this.widgetService.GetWidgets(model, this.HttpContext);
                ajaxMessageModel.Html = CRMHelper.RenderPartialViewToString(this, partialView, model);

                return this.Json(ajaxMessageModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (string.IsNullOrEmpty(mainView))
                {
                    // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                    return this.View((object)model);
                }
                else
                {
                    return this.View(mainView, (object)model);
                }
            }
        }

        protected AjaxMessageViewModel CreateAjaxMessageModel(ContentItem contentItem, string returnedShape)
        {
            AjaxMessageViewModel ajaxMessageModel = new AjaxMessageViewModel { Id = contentItem.Id, IsDone = ModelState.IsValid, Data = this.partSerializationManager.Convert(contentItem) };

            if (!string.IsNullOrEmpty(returnedShape))
            {
                dynamic returnedShapeModel = new Composite();
                returnedShapeModel.ContentItem = contentItem;
                returnedShapeModel.IsAlternative = false;
                ajaxMessageModel.Html = this.RenderShapePartial(returnedShapeModel, returnedShape);
            }

            return ajaxMessageModel;
        }

        protected string RenderShapePartial(dynamic model, string shapeType)
        {
            var shape = this.Shape.Partial(TemplateName: shapeType, Model: model);
            var display = this.GetDisplayHelper();
            return Convert.ToString(display(shape));
        }

        protected void EditOwner(ContentItem contentItem, bool createMode)
        {
            string prefix = "ContentItemPermissionPart";
            PostedEditOwnerViewModel model = new PostedEditOwnerViewModel();
            this.TryUpdateModel(model, prefix);
            this.EditOwner(contentItem, model, createMode);
        }

        protected void EditOwner(ContentItem contentItem, PostedEditOwnerViewModel model, bool createMode)
        {
            EditContentPermissionViewModel editContentPermissionViewModel = new EditContentPermissionViewModel();
            editContentPermissionViewModel.AccessType = ContentItemPermissionAccessTypes.Assignee;
            editContentPermissionViewModel.RemoveOldPermission = false;

            if (model.UserId.HasValue)
            {
                editContentPermissionViewModel.Targets.Add(new TargetContentItemPermissionViewModel { UserId = model.UserId.Value, Checked = true });
            }
            else if (!string.IsNullOrEmpty(model.GroupId))
            {
                var targetContentItemPermissionViewModel = Converter.DecodeGroupId(model.GroupId);
                if (targetContentItemPermissionViewModel != null)
                {
                    editContentPermissionViewModel.Targets.Add(targetContentItemPermissionViewModel);
                }
            }

            // try to check that the owner is changed in the model or not
            bool ownerIsChanged = false;
            var contentItemPermissionPart = contentItem.As<ContentItemPermissionPart>();
            if (contentItemPermissionPart.Record.Items == null)
            {
                contentItemPermissionPart.Record.Items = new List<ContentItemPermissionDetailRecord>();
            }

            var currentPermissionItems = contentItemPermissionPart.Record.Items.Where(c => c.AccessType == ContentItemPermissionAccessTypes.Assignee).ToList();
            foreach (var target in editContentPermissionViewModel.Targets)
            {
                // user of the model doesn't exist in the current items
                if (target.UserId.HasValue && currentPermissionItems.Count(c => c.User != null && c.User.Id == target.UserId.Value) == 0)
                {
                    ownerIsChanged = true;
                    break;
                }

                // team of the model doesn't exist in the current items
                if (target.TeamId.HasValue && currentPermissionItems.Count(c => c.Team != null && c.Team.Id == target.TeamId.Value) == 0)
                {
                    ownerIsChanged = true;
                    break;
                }

                // businessUnit of the model doesn't exist in the current items
                if (target.BusinessUnitId.HasValue && currentPermissionItems.Count(c => c.BusinessUnit != null && c.BusinessUnit.Id == target.BusinessUnitId.Value) == 0)
                {
                    ownerIsChanged = true;
                    break;
                }
            }

            // owner is set to null
            if (currentPermissionItems.Count > 0 && editContentPermissionViewModel.Targets.Count == 0)
            {
                ownerIsChanged = true;
            }

            // return in case owner doesn't change
            if (!ownerIsChanged)
            {
                return;
            }

            if (createMode || this.contentOwnershipHelper.IsChangingPermissionsValid(editContentPermissionViewModel, new[] { contentItem }, ModelState))
            {
                this.contentOwnershipHelper.Update(editContentPermissionViewModel, new[] { contentItem });
            }
        }

        protected dynamic GetDisplayHelper()
        {
            // We can specify any view name, just to get a View only, the shape template finding will be taken care by DisplayHelperFactory.
            // Here the "Brandking" view is always existed, we can also use something like "Layout" ...
            var viewResult = themeAwareViewEngine.FindPartialView(this.ControllerContext, "Layout", false, false);
            var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, new StringWriter());
            return displayHelperFactory.CreateHelper(viewContext, new ViewDataContainer { ViewData = viewContext.ViewData });
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}