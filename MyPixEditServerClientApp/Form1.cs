using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MyPixEditServerClientApp.PixEditWebServiceReference;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MyPixEditServerClientApp
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class PSClientApp : Form, IPixEditWebServiceCallback
    {
        public PSClientApp()
        {
            InitializeComponent();
        }

        PixEditWebServiceClient _server;
        Guid _userID;
        Guid _jobStatusSubscriberID = Guid.Empty;

        const int TICKINTERVAL_MS = 1000;
        System.Threading.Timer _timerCheckConnection;

        private void PSClientApp_Load(object sender, EventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            _server = new PixEditWebServiceClient(context, "NetTcpBinding_IPixEditWebService", "net.tcp://localhost:8080/PixEditWebService");
            _userID = _server.ValidateUser("admin", "admin");

            // Subscribe to notifications from the server about changes in job status (only available with net.tcp binding)
            if (_server != null)
                _jobStatusSubscriberID = _server.SubscribeNotifications(_userID);

            _timerCheckConnection = new System.Threading.Timer(TimerCheckConnection, null, TICKINTERVAL_MS, 0);

            btnCollectFinishedJob.Enabled = false;
        }
        // Set up a timer to keep our connection with the service alive. This timer just calls 'GetServerStatus' avery other second to keeps things from timing out
        // Another possibility would be to increase the timeout values in the config files, but then you might end up with a very long time out if something fails.
        private void TimerCheckConnection(object state)
        {
            try
            {
                string status = _server.GetServerStatus(_userID);
            }
            catch (Exception) // No connection = Disconnect
            {
            }

            // Finally, restart the timer to get back here in a few moments...
            _timerCheckConnection = new System.Threading.Timer(TimerCheckConnection, null, TICKINTERVAL_MS, 0);
        }
        private void btnCreateJob_Click(object sender, EventArgs e)
        {
            // This is the job ticket.
            // It specifies all the imaging functions to apply to the document. 
            // Convertproperties
            // Remove blank pages
            // Remove black borders
            // Erase borders
            // Resize to standard page size
            // Save properties
            // Each command is specified with its parameters. The paramaters may of course be altered and extracted to variables for custom application behaviour.

            string documentFile = "ScannedDoc1.pdf";
            string xmlJobTicket = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <ProcessSubmission xmlns:xsd =""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
               <Title>Convert job for " + documentFile + @"</Title>
               <Filenames>
                   <string>" + documentFile + @"</string>
               </Filenames>
               <ProcessProfile>
                  <commands>
                  <ProcessBase xsi:type=""ConvertProperties"">
                    <CmdName>Open/Convert Document</CmdName>
                    <CmdInfo>Open/Convert document and prepare for further processing</CmdInfo>
                    <TrackWordchanges>Ignore</TrackWordchanges>
                    <Showmarkup>Allow</Showmarkup>
                    <ExcelPageorientation>Landscape</ExcelPageorientation>
                    <SaveOutlookAttachments>true</SaveOutlookAttachments>
                    <ReplaceLinksWithText>false</ReplaceLinksWithText>
                    <AutomaticMsOffTimeout>true</AutomaticMsOffTimeout>
                    <OfficeAppTimeout>120</OfficeAppTimeout>
                    <ThrowOutlookAttachmentError>true</ThrowOutlookAttachmentError>
                    <ProgUsedHtmlconversion>Word</ProgUsedHtmlconversion>
                    <ThrowZipDocumentError>true</ThrowZipDocumentError>
                    <AutoCADuninitLayout>Ignore</AutoCADuninitLayout>
                    <UseAcadCustomSettings>false</UseAcadCustomSettings>
                    <CustomAcadsettings>
                      <Papersize>ISO_A3</Papersize>
                      <Orientation>Landscape</Orientation>
                      <PlotArea>Extents</PlotArea>
                      <Scale>s1_1</Scale>
                      <FitToPaper>true</FitToPaper>
                      <ScaleLineWeights>false</ScaleLineWeights>
                      <CenterPlot>true</CenterPlot>
                      <PlotLineWeights>true</PlotLineWeights>
                      <PaperspaceLast>true</PaperspaceLast>
                    </CustomAcadsettings>
                    <PDFRenderingDpi>300</PDFRenderingDpi>
                    <PDFRetainDpi>true</PDFRetainDpi>
                    <PDFSmoothText>false</PDFSmoothText>
                    <PDFTimeoutSeconds>1200</PDFTimeoutSeconds>
                    <VerifyPDFA1b>false</VerifyPDFA1b>
                    <VerifyPDFA2b>false</VerifyPDFA2b>
                    <VerifyPDFA3b>false</VerifyPDFA3b>
                    <ZipArchiveMergeContent>true</ZipArchiveMergeContent>
                    <XPSDigitalRender>true</XPSDigitalRender>
                  </ProcessBase>
                  <ProcessBase xsi:type= ""RemoveBlankPages"" >
                    <CmdName>Remove Blank Pages</CmdName>
                    <CmdInfo>Remove blank pages.This can be useful when scanning in duplex mode</CmdInfo>
                    <Graphics>0.06</Graphics>
                    <LeftTopMargins>
                      <Width>5</Width>
                      <Height>5</Height>
                    </LeftTopMargins>
                    <RightBottomMargins>
                      <Width>5</Width>
                      <Height>5</Height>
                    </RightBottomMargins>
                  </ProcessBase>
                  <ProcessBase xsi:type= ""RemoveBlackBorders"" >
                    <CmdName>Remove Black Borders</CmdName>
                    <CmdInfo>Removes black borders from a scanned document</CmdInfo>
                    <Crop>true</Crop>
                    <MaxDeskewAngle>4</MaxDeskewAngle>
                  </ProcessBase>
                  <ProcessBase xsi:type= ""EraseBorders"" >
                    <CmdName>Erase Borders</CmdName>
                    <CmdInfo>Erase Borders may be used to erase any remaining graphics noise near the document edges</CmdInfo>
                    <LeftBorder>3</LeftBorder>
                    <TopBorder>3</TopBorder>
                    <RightBorder>3</RightBorder>
                    <BottomBorder>3</BottomBorder>
                  </ProcessBase>
                  <ProcessBase xsi:type= ""ResizeToStandardPageSize"" >
                    <CmdName>Resize to standard page sizes</CmdName>
                    <CmdInfo>All document pages will be adapted to standard sizes to create a uniform document</CmdInfo>
                    <MaximumDeviation>5</MaximumDeviation>
                    <ResizeImage>false</ResizeImage>
                  </ProcessBase>
                  <ProcessBase xsi:type= ""OCR"" >
                    <CmdName>Text Recognition(Ocr)</CmdName>
                    <CmdInfo>Apply text recognition to make your documents searchable</CmdInfo>
                    <LargeObjectsFilter>30</LargeObjectsFilter>
                    <ExportOcrTextFile>false</ExportOcrTextFile>
                    <ExportRemoveLinefeed>false</ExportRemoveLinefeed>
                    <ExportPageSeparator>- Page { 0} -</ExportPageSeparator>
                    <ExportOnly>false</ExportOnly>
                    <OCRTimeout>0</OCRTimeout>
                  </ProcessBase>
                  <ProcessBase xsi:type=""SaveProperties"">
                    <CmdName>Save to Output Directory</CmdName>
                    <CmdInfo>Save the document in the output directory</CmdInfo>
                    <FileFormat>PDF_A1b</FileFormat>
                    <ImageQuality>85</ImageQuality>
                    <AlternativeSavePath />
                    <DocnameFormat>OrgFilename</DocnameFormat>
                    <AddOriginalFileExtention>false</AddOriginalFileExtention>
                    <Prefix />
                    <Suffix />
                    <IncrementStartNumber>0</IncrementStartNumber>
                    <Digits>4</Digits>
                    <Recompress>false</Recompress>
                    <MovePdfAVerifiedFiles>true</MovePdfAVerifiedFiles>
                  </ProcessBase>
                </commands>
                <ID>428cdbfa-8fc6-4ead-915d-fd43fd6518b0</ID>
                <Name>jobProfile</Name>
                <DocumentBackup>false</DocumentBackup>
                <Icon>BlackAndWhite.png</Icon>
               </ProcessProfile>
            </ProcessSubmission>";

            //Create processing job using our specified ticket
            Guid jobID = _server.CreateJob(_userID, xmlJobTicket);

            string jobDocFolder = _server.GetJobDocumentsFolder(_userID, jobID);

            // We have embedded a scanned test document in this project. Get it !
            string filePathname = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "Documents", documentFile);

            // Upload the document by using PixEdit Server chunk uploader.
            // This is a better way instead of copying the file to the job's working folder
            labelStatus.Text = "Uploading document...";
            StreamUploadDocumentToServer(jobID, filePathname);

            //Submit for processing. The Core service will add it to its queue and handle it when its ready.
            // When the job is submitted the core will invoke callbacks and usefull information can be retrieved on the OnJobStatusChanged handler
            _server.SubmitJob(_userID, jobID);
        }

        void StreamUploadDocumentToServer(Guid jobID, string filePathname)
        {
            // Stream the file to the server
            string filename = Path.GetFileName(filePathname);
            _server.BeginUploadJobDocument(_userID, jobID, Path.GetFileName(filePathname)); // Initializes a stream to the server

            FileInfo fileInfo = new FileInfo(filePathname);
            JobDocumentChunk chunk = new JobDocumentChunk();

            // Determines the size of the chunks and how many we need to stream the whole file
            int chunkSize = ((NetTcpBinding)_server.ChannelFactory.Endpoint.Binding).ReaderQuotas.MaxArrayLength;
            int chunkCount = (int)(fileInfo.Length / chunkSize);
            int lastChunkSize = (int)(fileInfo.Length % chunkSize);

            chunk.Data = new byte[chunkSize];
            chunk.ByteCount = chunkSize;

            // Streams data to PixEdit Server Core
            using (FileStream stream = new FileStream(filePathname, FileMode.Open))
            {
                for (int i = 0; i < chunkCount; i++)
                {
                    stream.Read(chunk.Data, 0, chunkSize);
                    _server.UploadJobDocumentChunk(_userID, jobID, filename, chunk);
                }

                if (lastChunkSize > 0)
                {
                    stream.Read(chunk.Data, 0, lastChunkSize);
                    chunk.ByteCount = (int)lastChunkSize;
                    _server.UploadJobDocumentChunk(_userID, jobID, filename, chunk);
                }
            }
        }

        public void OnServerStatus(string status)
        {

        }

        // This is the required implementation of the service callback for PixEditServiceReference.
        // It is invoked by the server to give the client information about changes in the job status.
        public void OnJobStatusChanged(string status)
        {
            // Simple decoding of the status xml sent from the server.
            // We will only fetch the job status here to provide a very simple progress and to know when the document is finished and ready for our client to collect

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(status);

            string jobstatus = "";
            // Get status from XML
            try
            {
                jobstatus = xmlDoc.SelectSingleNode("DocProcessStatus/Status").InnerText;
            }
            catch (Exception)
            {
            }
            // Updates progress label
            BeginInvoke(new Action(delegate
            {
                labelStatus.Text = jobstatus;
            }));

            // Job finished get document(s)
            if (jobstatus == "Delivered")
            {
                // Get Job ID
                Guid jobID = new Guid(xmlDoc.SelectSingleNode("DocProcessStatus/JobID").InnerText);

                // Copy document(s) to our client folder
                string jobFolder = _server.GetJobDocumentsFolder(_userID, jobID);

                // Get output filenames. In case of a document separation job there will multiple output filenames. 
                // Make sure we are able to collect them all
                XmlNodeList outputfileNodes = xmlDoc.SelectSingleNode("DocProcessStatus/OutputFilenames").ChildNodes;
                foreach (XmlNode outputfileNode in outputfileNodes)
                {
                    // Copy each file from the internal output to client specified folder
                    if (!string.IsNullOrEmpty(outputfileNode.InnerText))
                    {
                        string srcFilePathname = Path.Combine(jobFolder, outputfileNode.InnerText);
                        string dstFilePathanme = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "Processed Docs", outputfileNode.InnerText);
                        File.Copy(srcFilePathname, dstFilePathanme);
                    }
                }
                // Purge Job and remove it from PixEdit Server internal folders
                _server.PurgeJob(_userID, jobID);
            }
        }

        public void OnJobProfilesChanged()
        {
        }

        private void UnSubscribeFromPixEditServer()
        {
            if (_server != null && _server.State == CommunicationState.Opened)
            {
                _server.UnsubscribeNotifications(_jobStatusSubscriberID);
                _server.Close();
            }
        }

        private void PSClientApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnSubscribeFromPixEditServer();
        }

        private void btnCollectFinishedJob_Click(object sender, EventArgs e)
        {
        }
    }
}
