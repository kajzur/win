﻿

#pragma checksum "C:\Users\Matt\documents\visual studio 2013\Projects\HelloWorld\NBPReader\Trends.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "ACA8EE3508A9081261EFDA85C5CEEC2E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NBPReader
{
    partial class Trends : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 12 "..\..\Trends.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).ManipulationStarted += this.pageRoot_ManipulationStarted;
                 #line default
                 #line hidden
                #line 12 "..\..\Trends.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).ManipulationCompleted += this.pageRoot_ManipulationCompleted;
                 #line default
                 #line hidden
                #line 12 "..\..\Trends.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).ManipulationDelta += this.pageRoot_ManipulationDelta;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 68 "..\..\Trends.xaml"
                ((global::Windows.UI.Xaml.Controls.DatePicker)(target)).DateChanged += this.fromDatePicker_DateChanged;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 71 "..\..\Trends.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Button_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

