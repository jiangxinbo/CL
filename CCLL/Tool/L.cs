using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tool
{
    public class L
    {
        private static ILog file;
        private static ILog console;
        private static ILog email;

        public static ILog File
        {
            get
            {
                return file;
            }


        }

        public static ILog Console
        {
            get
            {
                return console;
            }


        }

        public static ILog Email
        {
            get
            {
                return email;
            }


        }

        static L()
        {
            initConsole();
        }



        public static void initConsole()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var repository = LogManager.CreateRepository("NETCoreRepository");
            string config_path = Path.Combine(Environment.CurrentDirectory, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(repository, new FileInfo(config_path));
            file = LogManager.GetLogger(repository.Name, "file");
            console = LogManager.GetLogger(repository.Name, "con");
            //email = LogManager.GetLogger(repository.Name, "email");
        }



    }
}
