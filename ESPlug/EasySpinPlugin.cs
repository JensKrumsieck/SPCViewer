namespace ESPlug
{
    public class EasySpinPlugin
    {
        public MLApp.MLApp matlab;

        public EasySpinPlugin()
        {
            this.matlab = new MLApp.MLApp();
        }
        public void MLExec(string cmd)
        {
            this.matlab.Execute(cmd);
        }

        public object MLGet(string var)
        {
            this.matlab.GetWorkspaceData(var, "base", out object data);
            return data;
        }

        public void MLPut(string var, object data)
        {
            this.matlab.PutWorkspaceData(var, "base", data);
        }

        public void setPath(string path)
        {
            this.MLExec("addpath(genpath('"+path+"'));");
        }

        public void silencium()
        {
            this.matlab.Visible = 0;
        }
    }
}
