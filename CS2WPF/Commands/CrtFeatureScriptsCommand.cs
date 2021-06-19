using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CS2WPF.Helpers;
using CS2WPF.View;
using CS2WPF.ViewModel;
using EnvDTE80;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Task = System.Threading.Tasks.Task;

namespace CS2WPF.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CrtFeatureScriptsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0500;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("fe28a662-94fa-4268-8e9d-44b2a21d52ff");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrtFeatureScriptsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CrtFeatureScriptsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CrtFeatureScriptsCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        IVsUIShell uiShell;
        DTE2 dTE2;
        ITextTemplating TextTemplating;
        IVsThreadedWaitDialogFactory DialogFactory;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package, DTE2 dTE2, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory)
        {
            // Switch to the main thread - the call to AddCommand in CrtFeatureScriptsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CrtFeatureScriptsCommand(package, commandService);

            Instance.uiShell = (IVsUIShell)(await package.GetServiceAsync(typeof(SVsUIShell)));
            Instance.dTE2 = dTE2;
            Instance.TextTemplating = textTemplating;
            Instance.DialogFactory = dialogFactory;

        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            MainWindowFeatureScripts dataContext = new MainWindowFeatureScripts(new PrismModuleModifier(dTE2), dTE2, TextTemplating, DialogFactory);
            WindowCS2WPF mainWin = new WindowCS2WPF(dataContext); //uiShell, dTE2, TextTemplating);
            //get the owner of this dialog
            IntPtr hwnd;
            uiShell.GetDialogOwnerHwnd(out hwnd);
            mainWin.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            uiShell.EnableModeless(0);
            try
            {
                WindowHelper.ShowModal(mainWin, hwnd);
            }
            finally
            {
                // This will take place after the window is closed.
                uiShell.EnableModeless(1);
            }
        }
    }
}
