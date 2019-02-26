using System;
using System.Reflection;
using BluEPRint2.EasySpin.Types;

namespace BluEPRint2.EasySpin
{
    public class EasySpinHelper
    {
        MethodInfo MLExec;
        MethodInfo MLPut;
        MethodInfo MLGet;
        MethodInfo MLSetPath;
        object matlab;

        public EasySpinHelper(string location)
        {
            //invoking methods from easyspin plugin
            var ESPlug = Assembly.LoadFrom(location);
            var EasySpinPlugin = ESPlug.GetType("ESPlug.EasySpinPlugin");
            matlab = Activator.CreateInstance(EasySpinPlugin);
            //set up methods
            MLExec = EasySpinPlugin.GetMethod("MLExec");
            MLGet = EasySpinPlugin.GetMethod("MLGet");
            MLPut = EasySpinPlugin.GetMethod("MLPut");
            MLSetPath = EasySpinPlugin.GetMethod("setPath");
            var MLSilent = EasySpinPlugin.GetMethod("silencium");
#if !DEBUG
            MLSilent.Invoke(matlab, new object[] { });
#endif
        }

        public void Execute(string cmd)
        {
            MLExec.Invoke(matlab, new object[] { cmd });
        }

        public object Get(string var)
        {
            return MLGet.Invoke(matlab, new object[] { var });
        }

        public void Put(string var, object value)
        {
            MLPut.Invoke(matlab, new object[] { var, value });
        }

        public void SetPath(string path)
        {
            MLSetPath.Invoke(matlab, new object[] { path });
        }

        public void garlic(Sys sys, Exp exp, string B = "B", string spc = "spc")
        {
            sys.Setup(this);
            exp.Setup(this);
            string vars = "[" + B + "," + spc + "]";
            this.Execute(vars + " = garlic(Sys,Exp);");
        }

        public void pepper(Sys sys, Exp exp, string B = "B", string spc = "spc")
        {
            sys.Setup(this);
            exp.Setup(this);
            string vars = "[" + B + "," + spc + "]";
            this.Execute(vars + " = pepper(Sys,Exp);");
        }
        
    }
}
