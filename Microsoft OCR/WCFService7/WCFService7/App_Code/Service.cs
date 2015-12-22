using AntsCode.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{

    public string Hello()
    {
        return "Hello, I'm Binh. Skype me if you need help: MyPossibles";
    }

    //public string GetData(string id)
    //{
    //    return string.Format("You entered: " + id);
    //}

    //public CompositeType GetDataUsingDataContract(CompositeType composite)
    //{
    //    if (composite == null)
    //    {
    //        throw new ArgumentNullException("composite");
    //    }
    //    if (composite.BoolValue)
    //    {
    //        composite.StringValue += "Suffix";
    //    }
    //    return composite;
    //}

    public string ExtractText(Stream file)
    {
        Guid guid = Guid.NewGuid();
        String fName = guid.ToString();
        //FileInfo fi = new FileInfo(fName);
        string folderPath = @"C:\Users\Huynh Binh PC\AppData\Local\Packages\2ca0072b-e230-42c2-a5f2-6ee47ccce84d_yekwsnrkhg0pr\LocalState\";
        //string saveAsPath = @"C:\Users\Huynh Binh PC\AppData\Local\Packages\2ca0072b-e230-42c2-a5f2-6ee47ccce84d_yekwsnrkhg0pr\LocalState\" + fi.Name;

        string fileType = ".png";
        string imageName = fName + fileType;
        string imagePath = folderPath + imageName;
        string txtResultPath = imagePath + ".txt";

        MultipartParser parser = new MultipartParser(file);

        Image image;

        int minSize = 40;
        int maxSize = 2600;

        if (parser.Success)
        {
            //SaveFile(parser.Filename, parser.ContentType, parser.FileContents);     
            MemoryStream ms = new MemoryStream(parser.FileContents);
            image = Image.FromStream(ms);

            if (image.Height < minSize || image.Height > maxSize || image.Width < minSize || image.Width > maxSize)
            {
                return "Image size must be between 40 - 2600 pixel";
            }

            try
            {
                image.Save(imagePath);
            }
            catch (Exception ex)
            {
                return "Get image failed. Try another image PNG 40 - 2600 px";
            }

        }
        else
        {
            return "Rest service failed. Try another image PNG 40 - 2600 px";
        }

        try
        {
            OcrCommandLineCaller.StartOcr(imageName);
        }
        catch (Exception ex)
        {
            return "OCR Failed. Try another image PNG 40 - 2600 px";
        }


        if (File.Exists(txtResultPath))
        {
            File.Delete(imagePath);
            string result = File.ReadAllText(txtResultPath);
            File.Delete(txtResultPath);
            return result;
        }
        else
        {
            return "Failed to extract text";
        }
    }

}
