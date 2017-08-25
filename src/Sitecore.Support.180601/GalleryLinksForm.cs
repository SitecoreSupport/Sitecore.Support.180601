namespace Sitecore.Support.Shell.Applications.ContentManager.Galleries.Links
{
    using Sitecore.Configuration;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Links;

    public class GalleryLinksForm : Sitecore.Shell.Applications.ContentManager.Galleries.Links.GalleryLinksForm
    {
        protected override string GetFullTooltip(Item reference, ItemLink link)
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

    }
}
