using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace CommandLineOcr
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

            int minSize = 40;
            int maxSize = 2600;

            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = null;

            if (!string.IsNullOrEmpty(e.Arguments))
            {
                try
                {

                    var ocrEngine = new OcrEngine(OcrLanguage.English);

                    file = await folder.GetFileAsync(e.Arguments);


                    ImageProperties imgProp = await file.Properties.GetImagePropertiesAsync();

                    if (imgProp.Height < minSize || imgProp.Height > maxSize || imgProp.Width < minSize || imgProp.Width > maxSize)
                    {
                        //write invalid image to output
                        await WriteToFile(folder, file.Name + ".txt", "Image size must be > 40 and < 2600 pixel");

                    }
                    else
                    {
                        WriteableBitmap bitmap;
                        var imgStream = await file.OpenAsync(FileAccessMode.Read);
                        bitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                        bitmap.SetSource(imgStream);


                        //using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
                        //{
                        //    bitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                        //    bitmap.SetSource(imgStream);
                        //}

                        // This main API call to extract text from image.
                        var ocrResult = await ocrEngine.RecognizeAsync((uint)bitmap.PixelHeight, (uint)bitmap.PixelWidth, bitmap.PixelBuffer.ToArray());

                        // OCR result does not contain any lines, no text was recognized. 
                        if (ocrResult.Lines != null)
                        {
                            string extractedText = "";

                            // Iterate over recognized lines of text.
                            foreach (var line in ocrResult.Lines)
                            {
                                // Iterate over words in line.
                                foreach (var word in line.Words)
                                {
                                    extractedText += word.Text + " ";
                                }
                                extractedText += Environment.NewLine;
                            }
                            await WriteToFile(folder, file.Name + ".txt", extractedText);
                        }
                        else
                        {
                            await WriteToFile(folder, file.Name + ".txt", "No Text");
                            //WriteToFile(folder, "failed.txt", "No Text");
                        }

                    }
                   
                }
                catch (Exception ex)
                {
                    await WriteToFile(folder, file.Name + ".txt", ex.Message + "\r\n" + ex.StackTrace);
                }
                App.Current.Exit();
            }
        }
        private async Task WriteToFile(StorageFolder folder, string fileName, string extractedText)
        {
            // Get the text data from the textbox. 
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(extractedText.ToCharArray());

            // Create a new file named DataFile.txt.
            var file = await folder.CreateFileAsync(fileName,
            CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}