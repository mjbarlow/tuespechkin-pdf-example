using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TuesPechkin;
using umbraco;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;


//    public static readonly IConverter _pdfConverter =
//           new ThreadSafeConverter(
//             new RemotingToolset<PdfToolset>(
//             new WinAnyCPUEmbeddedDeployment(
//                 new TempFolderDeployment())));


namespace Core.Utilities
{
    public static class PagePdf
    {
        public static void GeneratePdf(IConverter _pdfConverter, IContent content)
        {
            try
            {
                if (content != null)
                {
                    var umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
                    var pdfpage = umbracoHelper.TypedContent(content.Id);
                    
                    if (pdfpage != null)
                    {
                        // decide if to publish or not
                        var generatePDF = content.GetValue<bool>("generatePDF");

                        if (generatePDF)
                        {
                            // now set the properties
                            var pdfTitle = content.GetValue<string>("pDFDocumentTitle");
                            var pdfOutput = content.GetValue<string>("pDFDocumentPath");
                            var pdfUrl = content.GetValue<string>("pDFUrl");

                            var pdfRenderDelay = 10000;
                            LogHelper.Info(typeof(PagePdf), "PDF Render delay: " + pdfRenderDelay);

                            var customPaperSize = new PechkinPaperSize("208mm", "297mm");
                            LogHelper.Info(typeof(PagePdf), "Generating PDF for url: " + pdfUrl);
                            var document = new HtmlToPdfDocument
                            {
                                GlobalSettings =
                                {
                                    ProduceOutline = false,
                                    DocumentTitle = pdfTitle,
                                    //PaperSize = PaperKind.A4,

                                PaperSize = customPaperSize,
                                Margins =
                                    {
                                        All = 0,
                                        Unit = Unit.Millimeters
                                    }
                                },
                                Objects = {
                                    new ObjectSettings {
                                        PageUrl = pdfUrl,
                                        LoadSettings = new LoadSettings
                                        {
                                            RenderDelay = pdfRenderDelay,
                                            DebugJavascript = false,
                                            StopSlowScript = false,
                                            
                                        },
                                    },
                                }
                            };

                            byte[] result = _pdfConverter.Convert(document);
                            System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath(pdfOutput), result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               LogHelper.Error(typeof(CorrespondentPdf), "Error: Unable to generate Correspondents PDF", ex);
            }
            return;
        }
    }
}