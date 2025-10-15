using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace Simply_Calc_AppWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Événement de démarrage de l'application.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuration culture (format nombres avec point décimal)
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;


            // Gestionnaire d'exceptions non gérées
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        /// <summary>
        /// Gestionnaire d'exceptions non gérées du domaine d'application.
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogException(exception);

                MessageBox.Show(
                    $"Une erreur critique s'est produite:\n\n{exception.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Gestionnaire d'exceptions non gérées du dispatcher.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);

            MessageBox.Show(
                $"Une erreur s'est produite:\n\n{e.Exception.Message}",
                "Erreur",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            // Marquer comme gérée pour éviter le crash
            e.Handled = true;
        }

        /// <summary>
        /// Enregistre une exception dans un fichier log.
        /// </summary>
        private void LogException(Exception ex)
        {
            try
            {
                string logPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CalculatriceWPF",
                    "errors.log"
                );

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));

                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n\n";

                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // Ignorer erreurs de logging
            }
        }
    }
}

