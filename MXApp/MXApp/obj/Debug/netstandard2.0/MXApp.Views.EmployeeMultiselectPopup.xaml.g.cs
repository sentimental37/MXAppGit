//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MXApp.Views {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("C:\\Projects\\MXApp\\MXApp\\MXApp\\Views\\EmployeeMultiselectPopup.xaml")]
    public partial class EmployeeMultiselectPopup : global::Rg.Plugins.Popup.Pages.PopupPage {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.DataTemplate EmployeeItemTemplate;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Syncfusion.ListView.XForms.SfListView lstEmployees;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(EmployeeMultiselectPopup));
            EmployeeItemTemplate = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.DataTemplate>(this, "EmployeeItemTemplate");
            lstEmployees = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Syncfusion.ListView.XForms.SfListView>(this, "lstEmployees");
        }
    }
}
