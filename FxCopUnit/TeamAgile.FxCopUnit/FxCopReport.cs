using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml;
using Microsoft.FxCop.Common;

namespace TeamAgile.FxCopUnit
{
    public partial class FxCopReport
    {
        public static  FxCopReport MakeReport()
        {
            string fileName = "";
            XmlDocument document = SaveAndGetReportXml(ref fileName);
            return new FxCopReport(document,fileName);
        }

        private string fileName;

        internal string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private XmlDocument document;
        private AssertFxCop assertThat;

        public XmlDocument Document
        {
            get { return document; }
        }

        public AssertFxCop Assert
        {
            get { return assertThat; }
            set { assertThat = value; }
        }

        private FxCopReport(XmlDocument document,string docfilename)
        {
            this.fileName=docfilename;
            assertThat= new AssertFxCop(this);
            this.document = document;
            processXmlReport(document);
        }

        private static XmlDocument SaveAndGetReportXml(ref string fileName)
        {
            string reportFileName = string.Format(@"FxCopReport_{0}.xml", Guid.NewGuid());
            IsolatedStorageFile file = getStore();
            fileName=reportFileName;
            IsolatedStorageFileStream fs = new IsolatedStorageFileStream(reportFileName, FileMode.CreateNew, file);
            FxCopOM.Project.SaveReport(fs, string.Empty, false, Encoding.ASCII);
            XmlDocument reportXml = new XmlDocument();
            fs.Position = 0;
            reportXml.Load(fs);
            fs.Close();
            return reportXml;
        }

        private static IsolatedStorageFile getStore()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.Assembly | IsolatedStorageScope.User, null, null);
        }

        private void processXmlReport(XmlDocument reportXml)
        {
            XmlNodeList messageNodes = reportXml.SelectNodes("//Message");
            XmlNodeList issueNodes = reportXml.SelectNodes("//Issue");


            int numMessages = messageNodes.Count;
            int numExceptions = reportXml.SelectNodes("//Exception").Count;
            if (numMessages > 0)
            {
                Console.WriteLine(numMessages + " messages");
            }
            if (numExceptions > 0)
            {
                Console.WriteLine(numExceptions + " exceptions");
            }

            if ((FxCopOM.Project != null) && FxCopOM.Project.ContainsBuildBreakingMessage)
            {
                Console.WriteLine("Build breaking message encountered");
            }
        }

        public void Delete()
        {
            IsolatedStorageFile storageFile = getStore();
            try
            {
                storageFile.DeleteFile(fileName);
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
