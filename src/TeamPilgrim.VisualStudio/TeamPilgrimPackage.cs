using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.Build;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(ExplorerWindow))]
    [ProvideToolWindow(typeof(PendingChangesWindow))]
    [Guid(GuidList.guidTeamPilgrimPkgString)]
    public sealed class TeamPilgrimPackage : Package, IVsShellPropertyEvents
    {
        private static TeamPilgrimPackage _singleInstance;

        private static DTE Dte { get; set; }
        private static DTE2 Dte2 { get; set; }

        //public static ExtensionExceptionHandler ExceptionHandler { get; set; }

        private static IVsExtensibility Extensibility { get; set; }

        //public static ExtensionHost Host { get; set; }

        private static MenuCommandService MenuCommandService { get; set; }

        private static IVsUIShell UIShell { get; set; }

        private static TeamPilgrimVsService _teamPilgrimVsService;
        public static ITeamPilgrimVsService TeamPilgrimVsService
        {
            get { return _teamPilgrimVsService; }
        }

        private uint _shellPropertyChangeCookie;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public TeamPilgrimPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            _teamPilgrimVsService = new TeamPilgrimVsService();

            //http://blogs.msdn.com/b/vsxteam/archive/2008/06/09/dr-ex-why-does-getservice-typeof-envdte-dte-return-null.aspx
            var shellService = GetGlobalService(typeof(SVsShell)) as IVsShell;
            if (shellService != null)
                ErrorHandler.ThrowOnFailure(shellService.AdviseShellPropertyChanges(this, out _shellPropertyChangeCookie));
        }

        public int OnShellPropertyChange(int propid, object var)
        {
            // when zombie state changes to false, finish package initialization

            if ((int)__VSSPROPID.VSSPROPID_Zombie == propid)
            {
                if ((bool)var == false)
                {

                    // zombie state dependent code

                    Dte = GetService(typeof(SDTE)) as DTE;

                    Extensibility = (IVsExtensibility)GetGlobalService(typeof(IVsExtensibility));
                    Dte2 = (DTE2)Extensibility.GetGlobalsObject(null).DTE;

                    _teamPilgrimVsService.InitializeGlobals(Dte2);

                    // eventlistener no longer needed

                    var shellService = GetGlobalService(typeof(SVsShell)) as IVsShell;
                    if (shellService != null)
                        ErrorHandler.ThrowOnFailure(shellService.UnadviseShellPropertyChanges(_shellPropertyChangeCookie));

                    _shellPropertyChangeCookie = 0;
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowExplorerWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(ExplorerWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
        
        private void ShowPendingChangesWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(PendingChangesWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            TeamPilgrimPackage._singleInstance = this;
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                //                CommandID menuCommandID = new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.cmdTeamPilgrimExplorer);
                //                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                //                mcs.AddCommand( menuItem );

                // Create the command for the tool window
                var toolwndPilgrimExplorerCommandID = new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.toolTeamPilgrimExplorer);
                var menuToolWinPilgrimExplorer = new MenuCommand(ShowExplorerWindow, toolwndPilgrimExplorerCommandID);
                mcs.AddCommand(menuToolWinPilgrimExplorer);

                var toolwndPilgrimPendingChangesCommandID = new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.toolTeamPilgrimPendingChanges);
                var menuToolWinPilgrimPendingChanges = new MenuCommand(ShowPendingChangesWindow, toolwndPilgrimPendingChangesCommandID);
                mcs.AddCommand(menuToolWinPilgrimPendingChanges);
            }

            TeamPilgrimPackage.UIShell = (IVsUIShell)base.GetService(typeof(SVsUIShell));
            TeamPilgrimPackage.MenuCommandService = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            TeamPilgrimPackage._teamPilgrimVsService.InitializePackage(_singleInstance, UIShell);
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "TeamPilgrim",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }

        public T GetPackageService<T>()
        {
            if (_singleInstance == null)
            {
                return default(T);
            }
            
            return (T) _singleInstance.GetService(typeof(T));
        }
    }
}
