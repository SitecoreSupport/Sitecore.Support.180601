﻿// Generated by .NET Reflector from C:\inetpub\wwwroot\sc82u5\Website\Bin\Sitecore.Client.dll
namespace Sitecore.Support.Shell.Applications.ContentManager.Galleries.Links
{
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Links;
    using Sitecore.Resources;
    using Sitecore.Shell;
    using Sitecore.Shell.Applications.ContentManager.Galleries;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI;

    public class GalleryLinksForm : GalleryForm
    {
        protected Border Links;

        protected internal virtual string GetFullTooltip(Item reference, ItemLink link)
        {
            string str = string.Empty;
            Item ownerItem = Factory.GetDatabase(link.SourceDatabaseName).GetItem(link.SourceItemID, link.SourceItemLanguage, link.SourceItemVersion);
            if (ownerItem != null)
            {
                Field field = new Field(link.SourceFieldID, ownerItem);
                string str2 = string.Format(Translate.Text("The reference from '{0}' field."), field.Name);
                string languageAndVersionInfo = this.GetLanguageAndVersionInfo(field);
                str = string.Format("title=\"{0} {1}\"", str2, languageAndVersionInfo);
            }
            return str;
        }

        protected internal virtual string GetLanguageAndVersionInfo(Field field)
        {
            string str = field.Item.Version.ToString();
            string str2 = field.Item.Language.ToString();
            string str3 = string.Empty;
            if (field.Unversioned)
            {
                return (Translate.Text("Language:") + " " + str2);
            }
            if (!field.Shared && !field.Unversioned)
            {
                str3 = string.Format(Translate.Text("Language: {0}, Version: {1}"), str2, str);
            }
            return str3;
        }

        protected internal string GetLinkTooltip(Item reference, ItemLink link)
        {
            if (!this.LinkHasSourceField(link))
            {
                return this.GetShortTooltip();
            }
            return this.GetFullTooltip(reference, link);
        }

        protected virtual ItemLink[] GetReferences(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return item.Links.GetAllLinks(true, true);
        }

        protected virtual ItemLink[] GetRefererers(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            Sitecore.Links.LinkDatabase linkDatabase = Globals.LinkDatabase;
            Assert.IsNotNull(linkDatabase, "Link database cannot be null");
            return linkDatabase.GetItemReferrers(item, true);
        }

        protected internal virtual string GetShortTooltip()
        {
            string str = Translate.Text("Quick Info");
            string str2 = string.Format(Translate.Text("The reference from '{0}' section."), str);
            return string.Format("title=\"{0}\"", str2);
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            base.Invoke(message, true);
            message.CancelBubble = true;
            message.CancelDispatch = true;
        }

        private bool IsHidden(Item item)
        {
            while (item != null)
            {
                if (item.Appearance.Hidden)
                {
                    return true;
                }
                item = item.Parent;
            }
            return false;
        }

        protected internal virtual bool LinkHasSourceField(ItemLink link)
        {
            Assert.IsNotNull(link, "link");
            return (link.SourceFieldID != ItemIDs.Null);
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                StringBuilder result = new StringBuilder();
                Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
                if (itemFromQueryString != null)
                {
                    this.ProcessReferrers(itemFromQueryString, result);
                    this.ProcessReferences(itemFromQueryString, result);
                }
                this.Links.Controls.Add(new LiteralControl(result.ToString()));
            }
        }

        protected virtual void ProcessReferences(Item item, StringBuilder result)
        {
            List<Pair<Item, ItemLink>> references = new List<Pair<Item, ItemLink>>();
            foreach (ItemLink link in this.GetReferences(item))
            {
                Database database = Factory.GetDatabase(link.TargetDatabaseName, false);
                if (database != null)
                {
                    Item item2 = database.GetItem(link.TargetItemID);
                    if (((item2 == null) || !this.IsHidden(item2)) || UserOptions.View.ShowHiddenItems)
                    {
                        references.Add(new Pair<Item, ItemLink>(item2, link));
                    }
                }
            }
            this.RenderReferences(result, references);
        }

        protected virtual void ProcessReferrers(Item item, StringBuilder result)
        {
            List<Pair<Item, ItemLink>> referrers = new List<Pair<Item, ItemLink>>();
            foreach (ItemLink link in this.GetRefererers(item))
            {
                Database database = Factory.GetDatabase(link.SourceDatabaseName, false);
                if (database != null)
                {
                    Item item2 = database.GetItem(link.SourceItemID);
                    if (((item2 == null) || !this.IsHidden(item2)) || UserOptions.View.ShowHiddenItems)
                    {
                        referrers.Add(new Pair<Item, ItemLink>(item2, link));
                    }
                }
            }
            this.RenderReferrers(result, referrers);
        }

        private void RenderReferences(StringBuilder result, List<Pair<Item, ItemLink>> references)
        {
            result.Append("<div class=\"scMenuHeader\">" + Translate.Text("Items that the selected item refer to:") + "</div>");
            if (references.Count == 0)
            {
                result.Append("<div class=\"scNone\">" + Translate.Text("None") + "</div>");
            }
            else
            {
                foreach (Pair<Item, ItemLink> local1 in references)
                {
                    Item reference = local1.Part1;
                    ItemLink link = local1.Part2;
                    if (reference == null)
                    {
                        result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>", new object[] { Images.GetImage("Applications/16x16/error.png", 0x10, 0x10, "absmiddle", "0px 4px 0px 0px"), Translate.Text("Not found"), link.TargetDatabaseName, link.TargetItemID }));
                    }
                    else
                    {
                        string linkTooltip = this.GetLinkTooltip(reference, link);
                        result.Append(string.Concat(new object[] { "<a href=\"#\" class=\"scLink\" ", linkTooltip, " onclick='javascript:return scForm.invoke(\"item:load(id=", reference.ID, ",language=", reference.Language, ",version=", reference.Version, ")\")'>", Images.GetImage(reference.Appearance.Icon, 0x10, 0x10, "absmiddle", "0px 4px 0px 0px"), reference.GetUIDisplayName(), " - [", reference.Paths.Path, "]</a>" }));
                    }
                }
            }
        }

        private void RenderReferrers(StringBuilder result, List<Pair<Item, ItemLink>> referrers)
        {
            result.Append(this.ReferrersHeader);
            if (referrers.Count == 0)
            {
                result.Append("<div class=\"scNone\">" + Translate.Text("None") + "</div>");
            }
            else
            {
                result.Append("<div class=\"scRef\">");
                foreach (Pair<Item, ItemLink> local1 in referrers)
                {
                    Item item = local1.Part1;
                    ItemLink link = local1.Part2;
                    Item sourceItem = null;
                    if (link != null)
                    {
                        sourceItem = link.GetSourceItem();
                    }
                    if (item == null)
                    {
                        result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>", new object[] { Images.GetImage("Applications/16x16/error.png", 0x10, 0x10, "absmiddle", "0px 4px 0px 0px"), Translate.Text("Not found"), link.SourceDatabaseName, link.SourceItemID }));
                    }
                    else
                    {
                        string str = item.Language.ToString();
                        string str2 = item.Version.ToString();
                        if (sourceItem != null)
                        {
                            str = sourceItem.Language.ToString();
                            str2 = sourceItem.Version.ToString();
                        }
                        result.Append(string.Concat(new object[] { "<a href=\"#\" class=\"scLink\" onclick='javascript:return scForm.invoke(\"item:load(id=", item.ID, ",language=", str, ",version=", str2, ")\")'>", Images.GetImage(item.Appearance.Icon, 0x10, 0x10, "absmiddle", "0px 4px 0px 0px"), item.GetUIDisplayName() }));
                        if ((link != null) && !link.SourceFieldID.IsNull)
                        {
                            Field field = item.Fields[link.SourceFieldID];
                            if (!string.IsNullOrEmpty(field.DisplayName))
                            {
                                result.Append(" - ");
                                result.Append(field.DisplayName);
                                if (sourceItem != null)
                                {
                                    Field field2 = sourceItem.Fields[link.SourceFieldID];
                                    if ((field2 != null) && !field2.HasValue)
                                    {
                                        result.Append(" <span style=\"color:#999999\">");
                                        result.Append(Translate.Text("[inherited]"));
                                        result.Append("</span>");
                                    }
                                }
                            }
                        }
                        result.Append(" - [" + item.Paths.Path + "]</a>");
                    }
                }
                result.Append("</div>");
            }
        }

        protected virtual string ReferrersHeader
        {
            get
            {
                return string.Format("<div class=\"scMenuHeader\">{0}</div>", Translate.Text("Items that refer to the selected item:"));
            }
        }
    }
}
