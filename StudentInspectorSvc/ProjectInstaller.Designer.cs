namespace StudentInspectorSvc
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.StudentInspectorSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.StudentInspectorSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // StudentInspectorSvcProcessInstaller
            // 
            this.StudentInspectorSvcProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.StudentInspectorSvcProcessInstaller.Password = null;
            this.StudentInspectorSvcProcessInstaller.Username = null;
            // 
            // StudentInspectorSvcInstaller
            // 
            this.StudentInspectorSvcInstaller.ServiceName = "StudentInspectorSvc";
            this.StudentInspectorSvcInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.StudentInspectorSvcProcessInstaller,
            this.StudentInspectorSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller StudentInspectorSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller StudentInspectorSvcInstaller;
    }
}