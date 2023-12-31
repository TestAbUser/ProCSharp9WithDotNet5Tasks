﻿using AutoLot.Mvc.Controllers;
using AutoLot.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AutoLot.MVC.TagHelpers.Base
{
    public abstract class ItemLinkTagHelperBase : TagHelper
    {
        protected readonly IUrlHelper UrlHelper;

        protected ItemLinkTagHelperBase(IActionContextAccessor contextAccessor, IUrlHelperFactory
        urlHelperFactory)
        {
            UrlHelper = urlHelperFactory.GetUrlHelper(contextAccessor.ActionContext);
        }

        public int? ItemId { get; set; }

        protected void BuildContent(TagHelperOutput output,
            string actionName, string className, string displayText, string fontAwesomeName)
        {
            output.TagName = "a"; // Replaces <item-list> with <a> tag
            var target = ItemId.HasValue
            ? UrlHelper.Action(actionName, nameof(CarsDalController).RemoveController(), new { id = ItemId })
            : UrlHelper.Action(actionName, nameof(CarsDalController).RemoveController());
            output.Attributes.SetAttribute("href", target);
            output.Attributes.Add("class", className);
            output.Content.AppendHtml($@"{displayText} <i class=""fas fa-{fontAwesomeName}""></i>");
        }
    }
}