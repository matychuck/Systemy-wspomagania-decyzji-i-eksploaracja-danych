﻿#pragma checksum "..\..\3DWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7BDC1846C295629F6CB194F4FCDB7B3626EA4D01"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

using SWD;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SWD {
    
    
    /// <summary>
    /// _3DWindow
    /// </summary>
    public partial class _3DWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Viewport3D mainViewport;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.OrthographicCamera camera;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.ModelVisual3D Light1;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.ModelVisual3D Light2;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.ModelVisual3D Light3;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvasOn3D;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas controlPane;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxX;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxY;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxZ;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\3DWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button draw3DButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SWD;component/3dwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\3DWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mainViewport = ((System.Windows.Controls.Viewport3D)(target));
            return;
            case 2:
            this.camera = ((System.Windows.Media.Media3D.OrthographicCamera)(target));
            return;
            case 3:
            this.Light1 = ((System.Windows.Media.Media3D.ModelVisual3D)(target));
            return;
            case 4:
            this.Light2 = ((System.Windows.Media.Media3D.ModelVisual3D)(target));
            return;
            case 5:
            this.Light3 = ((System.Windows.Media.Media3D.ModelVisual3D)(target));
            return;
            case 6:
            this.canvasOn3D = ((System.Windows.Controls.Canvas)(target));
            
            #line 55 "..\..\3DWindow.xaml"
            this.canvasOn3D.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.OnViewportMouseUp);
            
            #line default
            #line hidden
            
            #line 56 "..\..\3DWindow.xaml"
            this.canvasOn3D.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.OnViewportMouseDown);
            
            #line default
            #line hidden
            
            #line 57 "..\..\3DWindow.xaml"
            this.canvasOn3D.MouseMove += new System.Windows.Input.MouseEventHandler(this.OnViewportMouseMove);
            
            #line default
            #line hidden
            return;
            case 7:
            this.controlPane = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.comboBoxX = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 9:
            this.comboBoxY = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 10:
            this.comboBoxZ = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 11:
            this.draw3DButton = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\3DWindow.xaml"
            this.draw3DButton.Click += new System.Windows.RoutedEventHandler(this.Draw3DButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

