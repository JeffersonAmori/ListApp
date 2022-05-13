using ListApp.Models;
using Sharpnado.CollectionView.RenderedViews;
using Xamarin.Forms;

namespace ListApp.Views.TemplateSelectors
{
    public class ListVisualElementHeaderFooterGroupingTemplateSelector : DataTemplateSelector
    {
        public SizedDataTemplate HeaderTemplate { get; set; }

        public SizedDataTemplate FooterTemplate { get; set; }

        public SizedDataTemplate GroupHeaderTemplate { get; set; }

        public DataTemplate ListTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                ListHeader _ => HeaderTemplate,
                ListFooter _ => FooterTemplate,
                ListGroupHeader _ => GroupHeaderTemplate,
                _ => ListTemplate,
            };
        }
    }
}
