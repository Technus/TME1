using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace TME1.UI.MarkupExtensions;
/// <summary>
/// Helper to create <see cref="ComboBoxTemplateSelector"/>
/// </summary>
public class ComboBoxTemplateSelectorExtension : MarkupExtension
{
  public DataTemplate? SelectedItemTemplate { get; set; }
  public DataTemplateSelector? SelectedItemTemplateSelector { get; set; }
  public DataTemplate? DropdownItemsTemplate { get; set; }
  public DataTemplateSelector? DropdownItemsTemplateSelector { get; set; }

  public override object ProvideValue(IServiceProvider serviceProvider)
      => new ComboBoxTemplateSelector
      {
        SelectedItemTemplate = SelectedItemTemplate,
        SelectedItemTemplateSelector = SelectedItemTemplateSelector,
        DropdownItemsTemplate = DropdownItemsTemplate,
        DropdownItemsTemplateSelector = DropdownItemsTemplateSelector
      };
}

/// <summary>
/// Selector for combobox 
/// </summary>
public class ComboBoxTemplateSelector : DataTemplateSelector
{
  public DataTemplate? SelectedItemTemplate { get; set; }
  public DataTemplateSelector? SelectedItemTemplateSelector { get; set; }
  public DataTemplate? DropdownItemsTemplate { get; set; }
  public DataTemplateSelector? DropdownItemsTemplateSelector { get; set; }

  public override DataTemplate? SelectTemplate(object item, DependencyObject container)
  {
    var visualElementToCheck = container;

    // Search up the visual tree, stopping at either a ComboBox or
    // a ComboBoxItem (or null). This will determine which template to use
    while (visualElementToCheck is not null and not ComboBox and not ComboBoxItem)
      visualElementToCheck = VisualTreeHelper.GetParent(visualElementToCheck);

    // If stopped at a ComboBoxItem, you're in the dropdown
    var inDropDown = visualElementToCheck is ComboBoxItem;

    if (inDropDown)
      return DropdownItemsTemplate ?? DropdownItemsTemplateSelector?.SelectTemplate(item, container);
    else
      return SelectedItemTemplate ?? SelectedItemTemplateSelector?.SelectTemplate(item, container);
  }
}
