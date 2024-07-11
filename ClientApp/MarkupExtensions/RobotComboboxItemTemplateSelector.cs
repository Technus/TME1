using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace TME1.ClientApp.MarkupExtensions;
/// <summary>
/// Helper to create <see cref="ComboBoxTemplateSelector"/>
/// </summary>
[MarkupExtensionReturnType(typeof(ComboBoxTemplateSelector))]
public class ComboBoxTemplateSelectorExtension : MarkupExtension
{
  /// <summary>
  /// Template for item inside the combobox
  /// </summary>
  public DataTemplate? SelectedItemTemplate { get; set; }
  /// <summary>
  /// Dependant template selector for item inside the combobox
  /// In case <see cref="SelectedItemTemplate"/> is <see langword="null"/>
  /// </summary>
  public DataTemplateSelector? SelectedItemTemplateSelector { get; set; }
  /// <summary>
  /// Template for item inside the combobox dropdown
  /// </summary>
  public DataTemplate? DropdownItemsTemplate { get; set; }
  /// <summary>
  /// Dependant template selector for item inside the combobox dropdown
  /// In case <see cref="DropdownItemsTemplate"/> is <see langword="null"/>
  /// </summary>
  public DataTemplateSelector? DropdownItemsTemplateSelector { get; set; }

  /// <summary>
  /// Provides <see cref="ComboBoxTemplateSelector"/> while copying properties from markup extension 
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
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
/// Template selector for combobox 
/// </summary>
public class ComboBoxTemplateSelector : DataTemplateSelector
{
  /// <summary>
  /// Template for item inside the combobox
  /// </summary>
  public DataTemplate? SelectedItemTemplate { get; set; }
  /// <summary>
  /// Dependant template selector for item inside the combobox
  /// In case <see cref="SelectedItemTemplate"/> is <see langword="null"/>
  /// </summary>
  public DataTemplateSelector? SelectedItemTemplateSelector { get; set; }
  /// <summary>
  /// Template for item inside the combobox dropdown
  /// </summary>
  public DataTemplate? DropdownItemsTemplate { get; set; }
  /// <summary>
  /// Dependant template selector for item inside the combobox dropdown
  /// In case <see cref="DropdownItemsTemplate"/> is <see langword="null"/>
  /// </summary>
  public DataTemplateSelector? DropdownItemsTemplateSelector { get; set; }

  /// <summary>
  /// Searches up the visual tree for proper combobox container, then applies the right template
  /// </summary>
  /// <param name="item"></param>
  /// <param name="container"></param>
  /// <returns>data template for <paramref name="item"/></returns>
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
