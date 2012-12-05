using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using JustAProgrammer.TeamPilgrim.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using NLog;

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

    [ProvideOptionPage(typeof(SettingsPage), "Team Pilgrim", "General", 0, 0, true)]

    //http://stackoverflow.com/questions/4478853/vsx-2010-package-loading-markup-xaml-parsing-cannot-find-assemblies
    [ProvideBindingPath]

    [Guid(GuidList.guidTeamPilgrimPkgString)]
    public sealed class TeamPilgrimPackage : Package, IVsShellPropertyEvents
    {
        private static readonly Logger Logger = TeamPilgrimLogManager.Instance.GetCurrentClassLogger();

        private static TeamPilgrimPackage _singleInstance;

        private static DTE Dte { get; set; }
        private static DTE2 Dte2 { get; set; }

        //public static ExtensionExceptionHandler ExceptionHandler { get; set; }

        private static IVsExtensibility Extensibility { get; set; }

        private static IVsSolution VsSolution;

        //public static ExtensionHost Host { get; set; }

        private static MenuCommandService MenuCommandService { get; set; }

        private static IVsUIShell UIShell { get; set; }

        private static TeamPilgrimVsService _teamPilgrimVsService;

        public static TeamPilgrimServiceModel TeamPilgrimServiceModel { get; private set; }
        
        public static TeamPilgrimSettings TeamPilgrimSettings { get; private set; }

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
            Logger.Debug("Start Constructor");

            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            _teamPilgrimVsService = new TeamPilgrimVsService();
            TeamPilgrimSettings = new TeamPilgrimSettings();

            //http://blogs.msdn.com/b/vsxteam/archive/2008/06/09/dr-ex-why-does-getservice-typeof-envdte-dte-return-null.aspx
            var shellService = GetGlobalService(typeof(SVsShell)) as IVsShell;
            if (shellService != null)
                ErrorHandler.ThrowOnFailure(shellService.AdviseShellPropertyChanges(this, out _shellPropertyChangeCookie));

            //NOTE: Only enable this when you are looking to debug a particular issue
            //Certain Visual Studio dialogs like the "Work Item Query" can be expected to throw binding errors
            //BindingErrorTraceListener.SetTrace();

            Logger.Debug("End Constructor");
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
            Logger.Info("Initialization: " + VersionInfo.Full);
            Logger.Debug("Start First Pass Initialization");

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
                var toolTeamPilgrimExplorer = new MenuCommand(ShowExplorerWindow, new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.toolTeamPilgrimExplorer));
                mcs.AddCommand(toolTeamPilgrimExplorer);

                var cmdTeamPilgrimExplorer = new MenuCommand(ShowExplorerWindow, new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.cmdTeamPilgrimExplorer));
                mcs.AddCommand(cmdTeamPilgrimExplorer);

                var toolTeamPilgrimPendingChanges = new MenuCommand(ShowPendingChangesWindow, new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.toolTeamPilgrimPendingChanges));
                mcs.AddCommand(toolTeamPilgrimPendingChanges);

                var cmdTeamPilgrimPendingChanges = new MenuCommand(ShowPendingChangesWindow, new CommandID(GuidList.guidTeamPilgrimCmdSet, (int)PkgCmdIDList.cmdTeamPilgrimPendingChanges));
                mcs.AddCommand(cmdTeamPilgrimPendingChanges);
            }

            TeamPilgrimPackage.UIShell = (IVsUIShell)base.GetService(typeof(SVsUIShell));
            TeamPilgrimPackage.MenuCommandService = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            TeamPilgrimPackage._teamPilgrimVsService.Initialize(_singleInstance, UIShell);

            CompletePackageInitialization();

            Logger.Debug("End First Pass Initialization");
        }

        public int OnShellPropertyChange(int propid, object var)
        {
            // when zombie state changes to false, finish package initialization

            if ((int)__VSSPROPID.VSSPROPID_Zombie == propid)
            {
                if ((bool)var == false)
                {

                    // zombie state dependent code
                    CompletePackageInitialization();

                    // eventlistener no longer needed

                    var shellService = GetGlobalService(typeof(SVsShell)) as IVsShell;
                    if (shellService != null)
                        ErrorHandler.ThrowOnFailure(shellService.UnadviseShellPropertyChanges(_shellPropertyChangeCookie));

                    _shellPropertyChangeCookie = 0;
                }
            }

            return VSConstants.S_OK;
        }

        private void CompletePackageInitialization()
        {
            Logger.Debug("Start Second Pass Initialization");

            if (Dte != null)
            {
                Logger.Warn("Package Previously Initialized"); 
                return;
            }

            Dte = GetService(typeof(SDTE)) as DTE;

            if (Dte == null)
            {
                Logger.Warn("DTE not found");
                return;
            }

            Extensibility = (IVsExtensibility)GetGlobalService(typeof(IVsExtensibility));
            VsSolution = (IVsSolution)GetService(typeof(SVsSolution));
            Dte2 = (DTE2)Extensibility.GetGlobalsObject(null).DTE;

            _teamPilgrimVsService.InitializeGlobals(Dte2, VsSolution);
            TeamPilgrimServiceModel = new TeamPilgrimServiceModel(new TeamPilgrimServiceModelProvider(), _teamPilgrimVsService);

            Logger.Debug("End Second Pass Initialization");
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

            return (T)_singleInstance.GetService(typeof(T));
        }
    }
}
