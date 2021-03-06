﻿using System.Collections.Generic;
using System.IO;
using Com.Microsoft.Projectoxford.Vision;
using Com.Microsoft.Projectoxford.Vision.Contract;
using GoogleGson;
using BookSearch.Model;
using Newtonsoft.Json;

namespace BookSearch
{
    class OCRHelpers
    {
        private VisionServiceRestClient visionServiceClient;
        
        //строка подключение к API Microsoft OCR
        public OCRHelpers()
        {
            visionServiceClient = new VisionServiceRestClient("bb82f299d1884cefa3ac2ce98a2a88b2", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
        }

        public  List<string> RecognizeText(Stream bitmap)
        {
            List<string> list = new List<string>();
            try
            {
                OCR ocr = visionServiceClient.RecognizeText(bitmap, LanguageCodes.AutoDetect, true);
                string result = new Gson().ToJson(ocr);

                OCRModel ocrModel = JsonConvert.DeserializeObject<OCRModel>(result);

                foreach (var region in ocrModel.regions)
                {
                    foreach (var line in region.lines)
                    {
                        foreach (var word in line.words)
                            list.Add(word.text);
                    }
                }

            }
            catch (Java.Lang.Exception ex)
            {
                ex.PrintStackTrace();
                string b = "";
            }            
            return list;

        }

    }
}