using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CallAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            string createText = "Hello and Welcome" + Environment.NewLine;
            //File.WriteAllText(@"C:\Users\lenovo\Documents\VisualStudio 2015\Projects\WebApplication11\foo.txt", createText);
            Task.Run(() => CallHttp());
            Task.WaitAll();
            Task.WaitAny();
            Thread.Sleep(1000);
        }
        // Simple async function returning a string...
        // this is how you call a third party API from C#
        static public async void CallHttp()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.crowdin.com/");
            HttpResponseMessage response = await client.GetAsync("api/project/referville/download/all.zip?key=f23e2c9181ef6630804761d33c0de737"); // see here we have project name and the secret key
            // which is needed to make the call.
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (FileStream fs = new FileStream(@"C:\temp\alpha\test.zip", FileMode.Create))
                await stream.CopyToAsync(fs);
            System.IO.Compression.ZipFile.ExtractToDirectory(@"C:\temp\alpha\test.zip", @"C:\temp\beta");
        }
    }
}
