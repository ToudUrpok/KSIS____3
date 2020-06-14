using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

namespace FilesStoringService
{
    class HTTPServer
    {
        HttpListener HttpListener;

        static private int Port = 8007;
        static private string HttpServerDomain = "localhost";

        FilesStorage FilesStorage;

        delegate void HttpRequestHandler(HttpListenerContext context);
        event HttpRequestHandler eReceivedHttpRequest;

        public HTTPServer()
        {
            FilesStorage = new FilesStorage();
            HttpListener = new HttpListener();
            eReceivedHttpRequest += HandleHttpRequest;
            
        }
        public void CreateListeningThread()
        {
            Thread listeningThread = new Thread(ListenToHttpRequests);
            listeningThread.Start();
        }

        private void ListenToHttpRequests()
        {
            HttpListener.Prefixes.Add("http://" + HttpServerDomain + ":" + Port.ToString() + "/");  //"http://*:" + Port.ToString() + "/"
            HttpListener.Start();
            while (true)
            {
                HttpListenerContext context = HttpListener.GetContext();
                eReceivedHttpRequest(context); 
            }
        }

        private void HandleHttpRequest(HttpListenerContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "POST":
                    HandlePostRequest(context);
                    break;
                case "GET":
                    HandleGetRequest(context);
                    break;
                case "HEAD":
                    HandleHeadRequest(context);
                    break;
                case "DELETE":
                    HandleDeleteRequest(context);
                    break;
            }
        }

        private void HandlePostRequest(HttpListenerContext context)
        {
            StreamReader reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            byte[] buffer = context.Request.ContentEncoding.GetBytes(reader.ReadToEnd());
            string fileName = Path.GetFileName(context.Request.Url.LocalPath);
            Console.WriteLine("Post request, file name: " + fileName);
            int fileID = FilesStorage.SaveFile(fileName, buffer);
            if (fileID == -1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            context.Response.OutputStream.Write(Encoding.ASCII.GetBytes(fileID.ToString()), 0,
                    Encoding.ASCII.GetBytes(fileID.ToString()).Length);
            context.Response.OutputStream.Close(); 
        }

        private void HandleGetRequest(HttpListenerContext context)
        {
            int fileID = int.Parse(Path.GetFileName(context.Request.Url.LocalPath));
            try
            {
                byte[] buffer = FilesStorage.ReadFile(fileID);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch(FileNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }

        private void HandleHeadRequest(HttpListenerContext context)
        {
            int fileID = int.Parse(Path.GetFileName(context.Request.Url.LocalPath));
            if (FilesStorage.FilesDictionary.ContainsKey(fileID))
            {
                context.Response.AddHeader("Name", FilesStorage.FilesDictionary[fileID].Name);
                context.Response.AddHeader("Size", FilesStorage.FilesDictionary[fileID].Size);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }

        private void HandleDeleteRequest(HttpListenerContext context)
        {
            int fileID = int.Parse(Path.GetFileName(context.Request.Url.LocalPath));
            try
            {
                FilesStorage.DeleteFile(fileID);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (FileNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }
    }
}
